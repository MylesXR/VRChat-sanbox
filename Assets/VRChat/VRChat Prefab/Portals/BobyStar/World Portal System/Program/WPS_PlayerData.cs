
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class WPS_PlayerData : UdonSharpBehaviour
{
    [UdonSynced] public bool AllowPlayerTeleport = true;
}