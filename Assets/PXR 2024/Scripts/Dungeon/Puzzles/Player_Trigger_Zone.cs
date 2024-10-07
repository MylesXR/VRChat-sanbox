using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Player_Trigger_Zone : UdonSharpBehaviour
{
    public Player_Trigger_Zone_Manager manager; // Reference to the manager script
    private bool isPlayerOnPlate = false; // Tracks if a player is on the plate

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        // Detect both local and remote players
        if (!isPlayerOnPlate && player != null)
        {
            isPlayerOnPlate = true;
            Debug.LogWarning(gameObject.name + " activated by " + player.displayName + " (Local: " + player.isLocal + ").");
            manager.OnPlateActivated(); // Notify the manager that a player entered the trigger zone
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        // Detect both local and remote players
        if (isPlayerOnPlate && player != null)
        {
            isPlayerOnPlate = false;
            Debug.LogWarning(gameObject.name + " deactivated by " + player.displayName + " (Local: " + player.isLocal + ").");
            manager.OnPlateDeactivated(); // Notify the manager that a player left the trigger zone
        }
    }

    public bool IsPlayerOnPlate()
    {
        return isPlayerOnPlate;
    }
}
