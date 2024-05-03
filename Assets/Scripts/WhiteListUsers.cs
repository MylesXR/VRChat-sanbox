using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class WhiteListUsers : UdonSharpBehaviour
{
    public Bobys_WorldPortalSystem Bobys_WorldPortalSystem;
    //public GameObject PortalUI;
    private string[] whitelistedUsers = { "MylesXR", "JustineKat", "The Threadman", "ColePaskuski", "sammievu", "helllomandy", "JakeRuneckles", "singlethread, the_majesty, zeNita, Zzoltan" };

    void Start()
    {
        // Initially disable the other script and UI
        Bobys_WorldPortalSystem.enabled = false;
        //PortalUI.SetActive(false);
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        player = Networking.LocalPlayer;
        // When a player joins, check if they are on the whitelist
        foreach (string user in whitelistedUsers)
        {
            if (user == player.displayName)
            {
                // If they are, enable the other script and UI
                Bobys_WorldPortalSystem.enabled = true;
                //PortalUI.SetActive(true);
                break;
            }
        }
    }

    //public override void OnPlayerLeft(VRCPlayerApi player)
    //{
    //    player = Networking.LocalPlayer;
    //    // When a player leaves, if they were on the whitelist, disable the other script and UI
    //    foreach (string user in whitelistedUsers)
    //    {
    //        if (user == player.displayName)
    //        {
    //            Bobys_WorldPortalSystem.enabled = false;
    //            //PortalUI.SetActive(false);
    //            break;
    //        }
    //    }
    //}
}
