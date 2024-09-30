using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Puzzle_II_Explorer_TriggerZone : UdonSharpBehaviour
{
    public string correctObjectName; // The name of the correct object that needs to be placed on this pressure plate
    public GameObject vfxObject; // VFX that will play when the correct object is placed
    public Puzzle_II_Explorer_Manager puzzleManager; // Reference to the manager

    private bool isCorrectObjectOnPlate = false; // To keep track if the correct object is on the plate

    private void Start()
    {
        // Ensure the VFX is disabled at the start
        if (vfxObject != null)
        {
            vfxObject.SetActive(false); // Start inactive
            Debug.Log("VFX disabled at start.");
        }
        else
        {
            Debug.LogError("VFX object is not assigned.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Make sure the other collider is not null
        if (other == null || other.gameObject == null)
        {
            Debug.LogWarning("The collider or game object is null.");
            return;
        }

        // Check if the object entering the trigger has the correct name
        if (other.gameObject.name == correctObjectName)
        {
            Debug.Log("Correct object placed on pressure plate: " + other.gameObject.name);
            isCorrectObjectOnPlate = true;
            PlayVFX(); // Trigger the VFX when the correct object is placed
            puzzleManager.UpdatePlateStatus(); // Notify the manager to check puzzle status
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Make sure the other collider is not null
        if (other == null || other.gameObject == null)
        {
            Debug.LogWarning("The collider or game object is null.");
            return;
        }

        // Handle logic if the correct object is removed from the pressure plate
        if (other.gameObject.name == correctObjectName)
        {
            Debug.Log("Object removed from pressure plate: " + other.gameObject.name);
            isCorrectObjectOnPlate = false;
            StopVFX(); // Stop the VFX when the correct object leaves
            puzzleManager.UpdatePlateStatus(); // Notify the manager to check puzzle status
        }
    }

    // Method to trigger VFX when correct object is placed
    public void PlayVFX()
    {
        if (vfxObject != null)
        {
            vfxObject.SetActive(true); // Play the VFX
            Debug.Log("VFX triggered for " + correctObjectName);
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
            Debug.Log("VFX stopped for " + correctObjectName);
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
