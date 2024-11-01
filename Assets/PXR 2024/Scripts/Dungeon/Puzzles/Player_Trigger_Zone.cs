using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class Player_Trigger_Zone : UdonSharpBehaviour
{
    public Player_Trigger_Zone_Manager TriggerZoneManager; // Reference to the manager script
    public GameObject TriggerZoneVFX;
    private bool isPlayerOnPlate = false; // Tracks if a player is on the plate

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!isPlayerOnPlate && player != null) // Detect both local and remote players
        {
            isPlayerOnPlate = true;
            ActivateTriggerZoneVFX();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ActivateTriggerZoneVFX");           
            TriggerZoneManager.OnPlateActivated(); // Notify the manager that a player entered the trigger zone
            //Debug.LogWarning(gameObject.name + " activated by " + player.displayName + " (Local: " + player.isLocal + ").");
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {     
        if (isPlayerOnPlate && player != null) // Detect both local and remote players
        {
            isPlayerOnPlate = false;
            DeactivateTriggerZoneVFX();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DeactivateTriggerZoneVFX");           
            TriggerZoneManager.OnPlateDeactivated(); // Notify the manager that a player left the trigger zone
            //Debug.LogWarning(gameObject.name + " deactivated by " + player.displayName + " (Local: " + player.isLocal + ").");
        }
    }

    public bool IsPlayerOnPlate()
    {
        return isPlayerOnPlate;
    }

    private void DeactivateTriggerZoneVFX()
    {
        TriggerZoneVFX.SetActive(false);
    }

    private void ActivateTriggerZoneVFX()
    {
        TriggerZoneVFX.SetActive(true);
    }
}