using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Puzzle_II_Explorer_TriggerZone : UdonSharpBehaviour
{
    public string correctObjectName; // The name of the correct object that needs to be placed on this pressure plate
    public GameObject vfxObject; // VFX that will play when the correct object is placed
    public Puzzle_II_Explorer_Manager puzzleManager; // Reference to the manager
    private VRCPlayerApi localPlayer;

    private bool isCorrectObjectOnPlate = false; // To keep track if the correct object is on the plate

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;

        // Ensure the VFX is disabled at the start
        if (vfxObject != null)
        {
            vfxObject.SetActive(false); // Start inactive
            Debug.LogWarning("VFX disabled at start.");
        }
        else
        {
            Debug.LogWarning("VFX object is not assigned.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Networking.IsOwner(gameObject)) // Ensure the owner controls network updates
        {
            Networking.SetOwner(localPlayer, gameObject);
        }

        // Make sure the other collider is not null
        if (other == null || other.gameObject == null)
        {
            Debug.LogWarning("The collider or game object is null.");
            return;
        }

        // Check if the object entering the trigger has the correct name
        if (other.gameObject.name == correctObjectName)
        {
            Debug.LogWarning("Correct object placed on pressure plate: " + other.gameObject.name);
            isCorrectObjectOnPlate = true;
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayVFX"); // Trigger the VFX for all clients
            puzzleManager.UpdatePlateStatus(); // Notify the manager to check puzzle status
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Networking.IsOwner(gameObject)) // Ensure the owner controls network updates
        {
            Networking.SetOwner(localPlayer, gameObject);
        }

        // Make sure the other collider is not null
        if (other == null || other.gameObject == null)
        {
            Debug.LogWarning("The collider or game object is null.");
            return;
        }

        // Handle logic if the correct object is removed from the pressure plate
        if (other.gameObject.name == correctObjectName)
        {
            Debug.LogWarning("Object removed from pressure plate: " + other.gameObject.name);
            isCorrectObjectOnPlate = false;
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "StopVFX"); // Stop the VFX for all clients
            puzzleManager.UpdatePlateStatus(); // Notify the manager to check puzzle status
        }
    }

    // Method to trigger VFX when correct object is placed
    public void PlayVFX()
    {
        if (vfxObject != null)
        {
            vfxObject.SetActive(true); // Play the VFX
            Debug.LogWarning("VFX triggered for " + correctObjectName);
        }
        else
        {
            Debug.LogError("VFX object is not assigned.");
        }
    }

    // Method to stop VFX when correct object leaves
    public void StopVFX()
    {
        if (vfxObject != null)
        {
            vfxObject.SetActive(false); // Stop the VFX
            Debug.LogWarning("VFX stopped for " + correctObjectName);
        }
        else
        {
            Debug.LogError("VFX object is not assigned.");
        }
    }

    // Method to check if the correct object is on the plate
    public bool IsCorrectObjectOnPlate()
    {
        return isCorrectObjectOnPlate;
    }
}
