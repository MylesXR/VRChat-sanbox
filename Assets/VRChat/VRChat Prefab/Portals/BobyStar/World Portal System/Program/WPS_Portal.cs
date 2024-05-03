
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class WPS_Portal : UdonSharpBehaviour
{
    public Bobys_WorldPortalSystem PortalSys;
    [Header("Portal Settings")]
    [UdonSynced] public Vector3 PortalPos;
    [UdonSynced] public Vector3 PortalUp;
    [UdonSynced] public int LocationID;
    [UdonSynced] public int TargertPlayerID;
    [Space]
    [UdonSynced] public bool UsePlayerID;
    [Space]
    [UdonSynced] public float LifeTimer = -1; int LastLifeTimeSync = -1;
    [Space]
    public Transform PortalBase;
    public CapsuleCollider PortalTrigger;
    [Header("Portal UI")]
    public Text PortalText;

    void Start()
    {
        if (PortalSys == null)
        { enabled = false; gameObject.SetActive(false); return; }
    }

    void Update()
    {
        transform.position = PortalPos; transform.up = PortalUp.normalized;        
        PortalBase.gameObject.SetActive(LifeTimer > 0);
        PortalTrigger.enabled = LifeTimer > 0;

        if (Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position != Vector3.zero)
        { PortalText.transform.eulerAngles = Vector3.up * (Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation.eulerAngles.y); }
        else
        {
            PortalText.transform.forward =
                ((PortalText.transform.position - Vector3.up * PortalText.transform.position.y) -
                (Networking.LocalPlayer.GetPosition() - Vector3.up * Networking.LocalPlayer.GetPosition().y)).normalized;
        }

        if (!Networking.IsOwner(gameObject))
        { 
            if (!UsePlayerID)
            {
                if (LocationID >= PortalSys.Locations.Length)
                { return; }

                PortalText.text =
                    "<b>Portal To:\n" +
                    $"<i>{PortalSys.Locations[LocationID].name}</i></b>\n" +
                    $"{Mathf.Ceil(LifeTimer)}";
            }
            else
            {
                if (VRCPlayerApi.GetPlayerById(TargertPlayerID) == null)
                { return; }

                PortalText.text =
                    "<b>Portal To Player:\n" +
                    $"<i>{VRCPlayerApi.GetPlayerById(TargertPlayerID).displayName}</i></b>\n" +
                    $"{Mathf.Ceil(LifeTimer)}";
            }
            return; 
        }

        LifeTimer = LifeTimer > 0 ? LifeTimer - Time.deltaTime : 0;
        if (Mathf.CeilToInt(LifeTimer) != LastLifeTimeSync)
        {
            LastLifeTimeSync = Mathf.CeilToInt(LifeTimer);
            if (!Networking.IsClogged)
            { RequestSerialization(); }
        }

        if (LifeTimer <= 0)
        { LifeTimer = -1f; return; }

        #region Location
        if (!UsePlayerID)
        {
            if (LocationID >= PortalSys.Locations.Length)
            { LocationID = 0; LifeTimer = 0; return; }

            PortalText.text = 
                "<b>Portal To:\n" +
                $"<i>{PortalSys.Locations[LocationID].name}</i></b>\n" +
                $"{Mathf.Ceil(LifeTimer)}";
        }
        #endregion
        #region Player
        else
        {
            if (VRCPlayerApi.GetPlayerById(TargertPlayerID) == null)
            { TargertPlayerID = 0; LifeTimer = 0; return; }

            PortalText.text =
                "<b>Portal To Player:\n" +
                $"<i>{VRCPlayerApi.GetPlayerById(TargertPlayerID).displayName}</i></b>\n" +
                $"{Mathf.Ceil(LifeTimer)}";
        }
        #endregion
    }

    public void ResetLifeTime()
    {
        LifeTimer = 30;
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player != Networking.LocalPlayer)
        { return; }

        bool BlockTeleport = false;
        if (player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position != Vector3.zero)
        {
            BlockTeleport |= Vector3.Dot(
                ((transform.position - Vector3.up * transform.position.y) - 
                (player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position - Vector3.up * player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position.y)).normalized, 
                player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation * Vector3.forward - Vector3.up * (player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation * Vector3.forward).y) < .5f;
        }
        else
        {
            BlockTeleport |= Vector3.Dot(
                (transform.position - player.GetPosition()).normalized,
                player.GetRotation() * Vector3.forward) < .5f;
        }

        if (!BlockTeleport)
        {
            if (!UsePlayerID)
            { Networking.LocalPlayer.TeleportTo(PortalSys.Locations[LocationID].position, PortalSys.Locations[LocationID].rotation); }
            else
            { Networking.LocalPlayer.TeleportTo(VRCPlayerApi.GetPlayerById(TargertPlayerID).GetPosition(), VRCPlayerApi.GetPlayerById(TargertPlayerID).GetRotation()); }

            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "ResetLifeTime");
        }
    }
}