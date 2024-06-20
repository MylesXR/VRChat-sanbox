using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

public class AxeAssigner : UdonSharpBehaviour
{
    public VRCObjectPool axePool; // Reference to the VRCObjectPool component
    private int currentIndex = 0; // To keep track of the axe index


    void Start()
    {
        
        if (axePool == null)
        {
            Debug.LogError("Axe pool is not assigned.");
            return;
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        Debug.Log($"[AxeAssigner] Player joined: {player.displayName}");

        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            Debug.LogWarning("Non-owner attempted to spawn object from AxeManager");
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
            Debug.Log($"[AxeAssigner] Set ownership of axe {axeToAssign.name} to player {player.displayName}");

            // Ensure the child objects also have the correct ownership
            foreach (Transform child in axeToAssign.transform)
            {
                Networking.SetOwner(player, child.gameObject);
                Debug.Log($"[AxeAssigner] Set ownership of child {child.gameObject.name} to player {player.displayName}");
                child.gameObject.SetActive(false);
            }

            // Assign axe index to the BarbarianThrowAxe component
            BarbarianThrowAxe throwAxeScript = axeToAssign.GetComponent<BarbarianThrowAxe>();
            if (throwAxeScript != null)
            {
                throwAxeScript.axeIndex = currentIndex; // Use the current index
                throwAxeScript.axeManager = this; // Set reference to AxeManager
                throwAxeScript.ownerPlayer = player; // Set the owner player
                Debug.Log($"[AxeAssigner] Assigned axe index {currentIndex} to {player.displayName}");
            }
             
            // Increment the index for the next axe
            currentIndex++;

            // Activate the axe object but ensure its children remain inactive
            axeToAssign.SetActive(true);

            if (axeToAssign.transform.childCount > -1)
            {
                GameObject childGameObject = axeToAssign.transform.GetChild(0).gameObject;
                childGameObject.SetActive(false);
                Debug.Log($"[AxeAssigner] Deactivated child game object: {childGameObject.name}");
            }
        }
        else
        {
            Debug.LogWarning("No available axes in the pool.");
        }
    }

    
    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        // Handle player leaving logic if needed
    }
}
