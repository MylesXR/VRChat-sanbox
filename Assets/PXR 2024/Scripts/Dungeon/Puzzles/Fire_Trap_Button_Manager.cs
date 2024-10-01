﻿using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Fire_Trap_Button_Manager : UdonSharpBehaviour
{
    public GameObject[] correctPattern; // Array to define the correct button order
    public GameObject[] objectsToDeactivate; // Array of objects to deactivate/reactivate upon pattern check

    public MeshRenderer backgroundMesh; // Mesh to change material on correct/incorrect pattern
    public int maxGuesses = 4; // Maximum number of guesses
    public Material incorrectMaterial; // Material to apply when the pattern is wrong
    public Material correctMaterial; // Material to apply when the pattern is correct (green)
    public Material guessMaterial; // Material to show feedback on each guess
    public Material baseMaterial; // Base material to revert to after showing guess
    public float guessMaterialDuration = 0.25f; // Duration to show guess material before reverting (configurable in Inspector)
    public float resetDelay = 5f; // Delay in seconds before resetting the puzzle (configurable in Inspector)

    public int flashCount = 3; // Number of flashes when the pattern is wrong
    public float flashInterval = 0.2f; // Interval between flashes

    private GameObject[] playerPattern; // Array to store the player's input pattern
    private int currentIndex; // Tracks the current position in the player's pattern
    private bool puzzleComplete = false; // Tracks if the puzzle is complete

    private bool isFlashing = false; // Tracks if flashing is in progress
    private int flashDuringResetCount; // Local flash count for the reset period
    private int remainingFlashes; // Local flash count to track remaining flashes during reset
    private bool flashToggle; // Toggles between base and incorrect material





    void Start()
    {
        playerPattern = new GameObject[maxGuesses]; // Initialize the player's pattern array
        if (backgroundMesh != null)
        {
            baseMaterial = backgroundMesh.material; // Store the base material of the background mesh
        }
        ResetPattern(); // Reset the pattern at the start
    }

    public void RegisterButtonPress(GameObject button)
    {
        if (puzzleComplete) // If the puzzle is already complete, ignore inputs
        {
            Debug.LogWarning("Puzzle is complete, cannot register button press.");
            return;
        }

        if (currentIndex >= maxGuesses) // If already reached max guesses, do nothing
        {
            Debug.LogWarning("Maximum guesses reached. Pattern check will now occur.");
            return;
        }

        // Log the button press
        Debug.LogWarning("Registering button press: " + button.name);

        // Add button press to the playerPattern array
        playerPattern[currentIndex] = button;
        currentIndex++;

        Debug.LogWarning("Button " + button.name + " registered as guess " + currentIndex);
        Debug.LogWarning("Current guess " + currentIndex + " out of " + maxGuesses);

        // Show the guess material for a short period, then revert back to base material
        ShowGuessMaterial();
        if (currentIndex == maxGuesses)
        {
            Debug.LogWarning("All guesses used. Checking pattern now.");
            CheckPattern();
        }
    }

    private void CheckPattern()
    {
        Debug.LogWarning("Checking player pattern against correct pattern.");

        bool isCorrect = true;

        // Compare each guess in the playerPattern array to the correctPattern array
        for (int i = 0; i < correctPattern.Length; i++)
        {
            if (playerPattern[i] == null)
            {
                Debug.LogWarning("Guess at index " + i + " is null.");
                isCorrect = false;
                break;
            }

            if (playerPattern[i] != correctPattern[i])
            {
                Debug.LogWarning("Pattern mismatch at guess " + (i + 1) + ": " + playerPattern[i].name + " != " + correctPattern[i].name);
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            Debug.LogWarning("Correct Pattern! Player has successfully matched the sequence.");
            HandleCorrectPattern();
        }
        else
        {
            Debug.LogWarning("Incorrect Pattern. Resetting the puzzle.");
            HandleIncorrectPattern();
        }
    }

    private void HandleCorrectPattern()
    {
        puzzleComplete = true; // Mark the puzzle as complete
        ChangeMeshMaterial(correctMaterial); // Immediately change to green for correct pattern
        DeactivateObjects(); // Deactivate the specified objects
        Debug.LogWarning("Correct pattern processed. Material changed to green and objects deactivated.");

        // Reset the puzzle after the reset delay
        SendCustomEventDelayedSeconds(nameof(ResetPuzzle), resetDelay);
    }

    private void HandleIncorrectPattern()
    {
        Debug.LogWarning("Incorrect pattern detected. Flashing will start during reset.");

        // Initialize remaining flashes with the value set in Inspector
        remainingFlashes = flashCount;

        // Ensure the flashing effect starts
        StartFlashingEffect();

        // Reset the puzzle after the flashing and reset delay
        SendCustomEventDelayedSeconds(nameof(ResetPuzzle), resetDelay);
    }

    public void ShowGuessMaterial()
    {
        // Change to guess material for the configured duration
        ChangeMeshMaterial(guessMaterial);
        Debug.LogWarning("Guess material set. Will revert after " + guessMaterialDuration + " seconds.");

        // Revert back to base material after guess duration
        SendCustomEventDelayedSeconds(nameof(RevertToBaseMaterial), guessMaterialDuration);
    }

    public void RevertToBaseMaterial()
    {
        // Only revert to base material if the puzzle is not complete (if it's complete, keep it green)
        if (!puzzleComplete)
        {
            ChangeMeshMaterial(baseMaterial); // Revert to the base material after showing guess material
        }
    }

    public void StartFlashingEffect()
    {
        if (!isFlashing)
        {
            Debug.LogWarning("Starting flashing effect.");
            isFlashing = true; // Set flashing state
            flashToggle = true; // Initialize the toggle for flashing materials
            FlashDuringReset(); // Start the flashing loop
        }
    }

    public void FlashDuringReset()
    {
        if (remainingFlashes <= 0)
        {
            // Stop flashing and revert to base material when the reset is done
            isFlashing = false;
            ChangeMeshMaterial(baseMaterial);
            Debug.LogWarning("Flashing complete. Puzzle is ready for reset.");
            return;
        }

        // Alternate between incorrect and base material using the flashToggle variable
        if (flashToggle)
        {
            ChangeMeshMaterial(incorrectMaterial); // Set to incorrect material
        }
        else
        {
            ChangeMeshMaterial(baseMaterial); // Set to base material
        }

        // Toggle the flashToggle for the next iteration
        flashToggle = !flashToggle;

        // Log the remaining flashes
        Debug.LogWarning("Flashing material. Remaining flashes: " + remainingFlashes);

        // Decrement the flash count
        remainingFlashes--;

        // Continue flashing after the interval
        SendCustomEventDelayedSeconds(nameof(FlashDuringReset), flashInterval);
    }


    public void FlashMaterial()
    {
        // Alternate between incorrect material and base material
        if (backgroundMesh.material == incorrectMaterial)
        {
            ChangeMeshMaterial(baseMaterial);
        }
        else
        {
            ChangeMeshMaterial(incorrectMaterial);
        }

        // Log the flash action
        Debug.LogWarning("Flashing material. Remaining flashes: " + flashCount);

        flashCount--;

        // Continue flashing until flashCount is zero
        if (flashCount > 0)
        {
            SendCustomEventDelayedSeconds(nameof(FlashMaterial), flashInterval);
        }
        else
        {
            // Stop flashing and revert to base material at the end
            isFlashing = false;
            ChangeMeshMaterial(baseMaterial);
            Debug.LogWarning("Flashing complete, reverting to base material.");
        }
    }

    public void ChangeMeshMaterial(Material newMaterial)
    {
        if (backgroundMesh != null)
        {
            backgroundMesh.material = newMaterial; // Change the material immediately
            backgroundMesh.material = backgroundMesh.material;
        }
        else
        {
            Debug.LogWarning("Background mesh is not set.");
        }
    }

    private void DeactivateObjects()
    {
        if (objectsToDeactivate != null && objectsToDeactivate.Length > 0)
        {
            foreach (GameObject obj in objectsToDeactivate)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    Debug.LogWarning("Deactivated object: " + obj.name);
                }
            }
        }
        else
        {
            Debug.LogWarning("No objects to deactivate or array is empty.");
        }
    }

    private void ReactivateObjects()
    {
        if (objectsToDeactivate != null && objectsToDeactivate.Length > 0)
        {
            foreach (GameObject obj in objectsToDeactivate)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                    Debug.LogWarning("Reactivated object: " + obj.name);
                }
            }
        }
        else
        {
            Debug.LogWarning("No objects to reactivate or array is empty.");
        }
    }

    private void ResetPattern()
    {
        currentIndex = 0; // Reset the index to start checking from the beginning
        for (int i = 0; i < playerPattern.Length; i++)
        {
            playerPattern[i] = null; // Initialize the pattern with nulls
        }
        puzzleComplete = false; // Reset puzzle completion status
    }

    public void ResetPuzzle()
    {
        Debug.LogWarning("Puzzle is resetting...");

        // Reset the pattern for a new attempt
        ResetPattern();

        // Reactivate any objects that were deactivated
        ReactivateObjects();

        // Ensure the base material is shown after reset
        ChangeMeshMaterial(baseMaterial);
    }
}
