using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AxeAssigner : UdonSharpBehaviour
{
    public GameObject[] axes; // Array of existing axe GameObjects to assign to players
    private bool[] axeAssigned; // Array to track which axe has been assigned

    void Start()
    {
        axeAssigned = new bool[axes.Length]; // Initialize the array to track assignment status

        // Ensure all axes start as inactive
        foreach (GameObject axe in axes)
        {
            axe.SetActive(false);
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        base.OnPlayerJoined(player);

        // Sync axeAssigned array across network
        RequestSerialization();

        int axeIndex = GetNextAvailableAxeIndex();

        if (axeIndex != -1)
        {
            GameObject axeToAssign = axes[axeIndex];

            // Check if this axe is already assigned
            if (axeAssigned[axeIndex])
            {
                Debug.LogWarning($"Axe at index {axeIndex} is already assigned.");
                return;
            }

            // Activate the axe object
            axeToAssign.SetActive(true);

            // Optionally, assign ownership to the player
            Networking.SetOwner(player, axeToAssign);

            // Mark axe as assigned locally and sync across network
            axeAssigned[axeIndex] = true;
            RequestSerialization(); // Request serialization after assignment

            Debug.Log($"Assigned axe {axeToAssign.name} to player {player.displayName}");
        }
        else
        {
            Debug.LogWarning("Not enough axes to assign to all players.");
        }
    }

    public override void OnDeserialization()
    {
        // Ensure axes are correctly activated/deactivated based on axeAssigned array
        for (int i = 0; i < axes.Length; i++)
        {
            axes[i].SetActive(axeAssigned[i]);
        }
    }

    int GetNextAvailableAxeIndex()
    {
        for (int i = 0; i < axeAssigned.Length; i++)
        {
            if (!axeAssigned[i])
            {
                return i;
            }
        }
        return -1; // Return -1 if all axes are assigned
    }
}
