using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Fire_Trap_Manager : UdonSharpBehaviour
{
    public GameObject[] FireTraps; // Array of fire traps

    [UdonSynced] // Synced variable to track if traps are disabled across all players
    private bool areTrapsDisabled = false;

    private void Start()
    {
        // Ensure traps are in the correct state at the start
        UpdateFireTrapStates();
    }

    public override void Interact()
    {
        // Ensure we are the owner of the object before making changes
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        // Toggle the fire traps and sync the state across the network
        ToggleFireTraps();

        // Request to sync the updated state across all clients
        RequestSerialization();

        // Call the update function on all clients
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UpdateFireTrapStates");
    }

    // Method to toggle fire traps
    private void ToggleFireTraps()
    {
        // Flip the boolean state to enable/disable the traps
        areTrapsDisabled = !areTrapsDisabled;
    }

    // Update the state of fire traps for all clients
    public void UpdateFireTrapStates()
    {
        for (int i = 0; i < FireTraps.Length; i++)
        {
            if (FireTraps[i] != null)
            {
                // Set each fire trap active or inactive based on the synced state
                FireTraps[i].SetActive(!areTrapsDisabled);
            }
        }
    }

    public override void OnDeserialization()
    {
        // When the object state is deserialized, update the traps for all players
        UpdateFireTrapStates();
    }
}
