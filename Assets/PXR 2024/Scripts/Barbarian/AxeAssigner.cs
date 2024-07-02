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
                //the PC axe in the head tracker prefab
                GameObject pcAxeChild = axeToAssign.transform.GetChild(0).gameObject;
                pcAxeChild.SetActive(false);
                Log($"Deactivated child game object: {pcAxeChild.name}");



                //The Return point for the VRaxe in the head tracker prefab
                GameObject returnPointChild = axeToAssign.transform.GetChild(1).gameObject;
                returnPointChild.SetActive(true);
                Log($"activated child game object: {returnPointChild.name}");



                //The parent of the pc tether objects in the head tracker prefab
                GameObject pcTetherPartent = axeToAssign.transform.GetChild(3).gameObject;
                pcTetherPartent.SetActive(false);
                Log($"activated child game object: {pcTetherPartent.name}");
                //Set ownership of all the pc tether objects children
                foreach (Transform pcTetherLeftHandChild in pcTetherPartent.transform)
                {
                    Networking.SetOwner(player, pcTetherLeftHandChild.gameObject);
                    Log($"Set ownership of child {pcTetherLeftHandChild.gameObject.name} to player {player.displayName}");
                    pcTetherLeftHandChild.gameObject.SetActive(true);
                }
                //Set ownership of the pc tether objects left hand grandchildren
                GameObject pcTetherLeftHandParent = pcTetherPartent.transform.GetChild(0).gameObject;
                GameObject pcLeftTetherRingParent = pcTetherLeftHandParent.transform.GetChild(2).gameObject;
                foreach (Transform pcLeftTetherRingGrandChild in pcLeftTetherRingParent.transform)
                {
                    Networking.SetOwner(player, pcLeftTetherRingGrandChild.gameObject);
                    Log($"Set ownership of grandchild {pcLeftTetherRingGrandChild.gameObject.name} to player {player.displayName}");
                    pcLeftTetherRingGrandChild.gameObject.SetActive(true);
                }
                //Set ownership of the pc tether object right hand grandchildren
                GameObject pcTetherRightHandParent = pcTetherPartent.transform.GetChild(1).gameObject;
                GameObject pcTetherRightRingParent = pcTetherRightHandParent.transform.GetChild(2).gameObject;
                foreach (Transform pcTetherRightRingGrandChild in pcTetherRightRingParent.transform)
                {
                    Networking.SetOwner(player, pcTetherRightRingGrandChild.gameObject);
                    Log($"Set ownership of grandchild {pcTetherRightRingGrandChild.gameObject.name} to player {player.displayName}");
                    pcTetherRightRingGrandChild.gameObject.SetActive(true);
                }

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
