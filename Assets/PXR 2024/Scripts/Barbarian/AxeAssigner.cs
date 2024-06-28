using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using UnityEngine.UI;

public class AxeAssigner : UdonSharpBehaviour
{
    public VRCObjectPool axePool; // Reference to the VRCObjectPool component
    private int currentIndex = 0; // To keep track of the axe index
    public Text debugText; // Reference to the UI Text component for debug logs

    void Start()
    {
        if (axePool == null)
        {
            LogError("Axe pool is not assigned.");
            return;
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        Log($"Player joined: {player.displayName}");

        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            LogWarning("Non-owner attempted to spawn object from AxeManager");
            return;
        }

        GameObject axeToAssign = axePool.TryToSpawn();
        if (axeToAssign != null)
        {
            // Ensure the axe and its children are inactive before assignment
            axeToAssign.SetActive(false);
            foreach (Transform child in axeToAssign.transform)
            {
                child.gameObject.SetActive(false);
            }

            // Assign ownership to the player
            Networking.SetOwner(player, axeToAssign);
            Log($"Set ownership of axe {axeToAssign.name} to player {player.displayName}");

            // Ensure the child objects also have the correct ownership
            foreach (Transform child in axeToAssign.transform)
            {
                Networking.SetOwner(player, child.gameObject);
                Log($"Set ownership of child {child.gameObject.name} to player {player.displayName}");
                child.gameObject.SetActive(false);
            }

            // Assign axe index to the BarbarianThrowAxe component
            BarbarianThrowAxe throwAxeScript = axeToAssign.GetComponent<BarbarianThrowAxe>();
            if (throwAxeScript != null)
            {
                throwAxeScript.axeIndex = currentIndex; // Use the current index
                throwAxeScript.axeManager = this; // Set reference to AxeManager
                throwAxeScript.ownerPlayer = player; // Set the owner player
                Log($"Assigned axe index {currentIndex} to {player.displayName}");
            }

            // Increment the index for the next axe
            currentIndex++;

            // Activate the axe object but ensure its children remain inactive
            axeToAssign.SetActive(true);

            if (axeToAssign.transform.childCount > -1)
            {
                GameObject childGameObject = axeToAssign.transform.GetChild(0).gameObject;
                childGameObject.SetActive(false);
                Log($"Deactivated child game object: {childGameObject.name}");
            }
            if (axeToAssign.transform.childCount > -1)
            {
                GameObject childGameObject = axeToAssign.transform.GetChild(1).gameObject;
                childGameObject.SetActive(true);
                Log($"activated child game object: {childGameObject.name}");
            }
        }
        else
        {
            LogWarning("No available axes in the pool.");
        }
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        // Handle player leaving logic if needed
    }

    private void Log(string message)
    {
        Debug.Log(message);
        if (debugText != null)
        {
            debugText.text += "\n" + message;
        }
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning(message);
        if (debugText != null)
        {
            debugText.text += "\nWARNING: " + message;
        }
    }

    private void LogError(string message)
    {
        Debug.LogError(message);
        if (debugText != null)
        {
            debugText.text += "\nERROR: " + message;
        }
    }
}
