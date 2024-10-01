using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Fire_Trap_Manager : UdonSharpBehaviour
{
    public GameObject[] FireTraps;

    public override void Interact()
    {
        // Send the network event to all players to disable the fire traps
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DisableFireTraps");
    }

    // This method will be called across all clients
    public void DisableFireTraps()
    {
        if (FireTraps == null || FireTraps.Length == 0)
        {
            Debug.LogError("FireTraps array is not set or is empty.");
            return;
        }

        // Loop through the FireTraps array and disable each fire trap
        for (int i = 0; i < FireTraps.Length; i++)
        {
            if (FireTraps[i] != null)
            {
                FireTraps[i].SetActive(false);
                Debug.LogWarning($"FireTrap at index {i} disabled.");
            }
            else
            {
                Debug.LogWarning($"FireTrap at index {i} is null.");
            }
        }
    }
}
