
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class WPS_NetworkManager1 : UdonSharpBehaviour
{
    public Bobbys_WorldPortalSystem1 WPS;
    public WPS_PlayerData[] PlayerData;
    public int OwnerOfID = -1;

    void Start()
    {
        if (WPS == null || PlayerData.Length == 0)
        { enabled = false; return; }

        _FindNewPlayerData();
    }

    public void _FindNewPlayerData()
    {
        bool SkipFirstMaster = false;
        for (int i = 0; i < PlayerData.Length; i++)
        {
            #region Skip Null
            if (PlayerData[i] == null)
            { continue; }
            #endregion

            #region If Owned By Master
            if (Networking.GetOwner(PlayerData[i].gameObject).isMaster)
            {
                #region Check Master Skip
                if (!SkipFirstMaster & !Networking.LocalPlayer.isMaster)
                { SkipFirstMaster = true; continue; }
                #endregion

                #region Attempt to take Player Data
                OwnerOfID = i;
                Networking.SetOwner(Networking.LocalPlayer, PlayerData[i].gameObject);
                SendCustomEventDelayedSeconds("_CheckIfStillOwner", Random.Range(1, 5f));
                return;
                #endregion
            }
            #endregion
        }

        Debug.LogError("Couldn't Find Open Player Data. Trying again in 5s...");
        SendCustomEventDelayedSeconds("_FindNewPlayerData", 5);
    }

    public void _CheckIfStillOwner()
    {
        #region Find New Player Data if Lost or Taken Away
        if (OwnerOfID == -1)
        { _FindNewPlayerData(); return; }

        if (!Networking.IsOwner(PlayerData[OwnerOfID].gameObject))
        { OwnerOfID = -1; _FindNewPlayerData(); return; }
        #endregion

        WPS.PlayerData = PlayerData[OwnerOfID];
        // Check Again
        SendCustomEventDelayedSeconds("_CheckIfStillOwner", 5);
    }
}