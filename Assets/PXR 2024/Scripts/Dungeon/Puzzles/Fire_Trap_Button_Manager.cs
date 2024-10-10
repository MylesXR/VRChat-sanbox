using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)] // Manual sync for controlled networking
public class Fire_Trap_Button_Manager : UdonSharpBehaviour
{
    [Header("Arrays")]
    public int[] correctPattern; // Correct order of button IDs
    public GameObject[] objectsToDeactivate; // Objects/VFX to activate or deactivate
    public MeshRenderer backgroundMesh; // For visual feedback

    [Header("Settings")]
    public int maxGuesses = 4; // Maximum number of button presses allowed
    public Material incorrectMaterial;
    public Material correctMaterial;
    public Material guessMaterial;
    public Material baseMaterial;
    public float guessMaterialDuration = 0.25f; // Time to show guess material
    public float resetDelay = 5f; // Delay before resetting puzzle after solving
    public float vfxActiveTime = 3f; // Time the objects remain off after correct solution
    public int flashCount = 3; // Number of flashes for incorrect pattern
    public float flashInterval = 0.2f; // Flashing interval for incorrect pattern

    private int[] playerPattern; // Store player's input as integers
    private bool puzzleComplete = false; // Puzzle state

    [UdonSynced] private int syncedMaterialIndex = 0; // Sync material changes across network
    private const int BASE_MATERIAL = 0;
    private const int GUESS_MATERIAL = 1;
    private const int INCORRECT_MATERIAL = 2;
    private const int CORRECT_MATERIAL = 3;

    [UdonSynced] private bool isObjectActive = true; // Objects start active
    [UdonSynced] private bool isFlashingSynced; // Sync flashing state across players
    [UdonSynced] private bool flashToggle = false; // Tracks the flashing state
    [UdonSynced] private int remainingFlashes; // Remaining flashes for the current incorrect pattern

    [UdonSynced] private int currentGlobalGuessIndex = 0; // Synced guess index across all players
    [UdonSynced] private int[] syncedGuessIDs; // Synced pattern of guesses as integer values

    private VRCPlayerApi localPlayer;

    void Start()
    {
        localPlayer = Networking.LocalPlayer;
        playerPattern = new int[maxGuesses]; // Initialize the pattern array
        syncedGuessIDs = new int[maxGuesses]; // Initialize synced guess IDs array

        if (backgroundMesh != null)
        {
            baseMaterial = backgroundMesh.material; // Save the base material
        }

        // Apply material and object states when joining
        ApplyMaterialFromIndex();
        UpdateObjectStates();
        Debug.LogWarning("[Fire_Trap_Button_Manager] Start method initialized.");
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        Debug.LogWarning("[Fire_Trap_Button_Manager] Player joined: " + player.displayName);

        if (Networking.IsOwner(gameObject)) // Ensure the owner sends the current state
        {
            RequestSerialization(); // Force sync for new players
            Debug.LogWarning("[Fire_Trap_Button_Manager] Requesting serialization on player join.");
        }
    }

    public void RegisterButtonPress(int buttonID)
    {
        Debug.LogWarning("[Fire_Trap_Button_Manager] Button with ID: " + buttonID + " was pressed.");

        if (puzzleComplete || currentGlobalGuessIndex >= maxGuesses)
        {
            Debug.LogWarning("[Fire_Trap_Button_Manager] Button press ignored. Puzzle complete or max guesses reached.");
            return;
        }

        TakeOwnership(); // Ensure the player can sync their interactions

        playerPattern[currentGlobalGuessIndex] = buttonID; // Store the guess ID
        syncedGuessIDs[currentGlobalGuessIndex] = buttonID; // Store the guess ID in the synced array
        currentGlobalGuessIndex++; // Increment the global guess index

        Debug.LogWarning("[Fire_Trap_Button_Manager] Guess registered. Current guess count: " + currentGlobalGuessIndex);

        RequestSerialization(); // Sync the guess across all players
        ShowGuessMaterial();

        if (currentGlobalGuessIndex == maxGuesses)
        {
            Debug.LogWarning("[Fire_Trap_Button_Manager] Max guesses reached. Checking pattern.");
            CheckPattern();
        }
    }

    private void CheckPattern()
    {
        bool isCorrect = true;

        // Compare the synced guess IDs to the correct pattern
        for (int i = 0; i < correctPattern.Length; i++)
        {
            if (syncedGuessIDs[i] != correctPattern[i])
            {
                isCorrect = false;
                Debug.LogWarning("[Fire_Trap_Button_Manager] Pattern mismatch at index: " + i);
                break;
            }
        }

        if (isCorrect)
        {
            Debug.LogWarning("[Fire_Trap_Button_Manager] Pattern is correct.");
            HandleCorrectPattern();
        }
        else
        {
            Debug.LogWarning("[Fire_Trap_Button_Manager] Pattern is incorrect.");
            HandleIncorrectPattern();
        }
    }

    private void HandleCorrectPattern()
    {
        puzzleComplete = true;
        syncedMaterialIndex = CORRECT_MATERIAL; // Sync the correct material
        RequestSerialization(); // Sync across all players
        ApplyMaterialFromIndex();
        Debug.LogWarning("[Fire_Trap_Button_Manager] Correct pattern handled. VFX deactivated.");

        DeactivateVFX(); // Deactivate the objects (they were active)

        SendCustomEventDelayedSeconds(nameof(ReactivateVFX), vfxActiveTime); // Reactivate after the timed amount
        SendCustomEventDelayedSeconds(nameof(ResetPuzzle), resetDelay); // Reset puzzle after a delay
    }

    private void HandleIncorrectPattern()
    {
        remainingFlashes = flashCount;
        isFlashingSynced = true;
        RequestSerialization();
        Debug.LogWarning("[Fire_Trap_Button_Manager] Incorrect pattern. Flashing VFX.");
        FlashDuringReset(); // Start flashing
        SendCustomEventDelayedSeconds(nameof(ResetPuzzle), resetDelay); // Reset puzzle after flashing
    }

    public void ShowGuessMaterial()
    {
        TakeOwnership(); // Ensure the player owns the object to sync changes

        syncedMaterialIndex = GUESS_MATERIAL;
        RequestSerialization(); // Sync the material index
        ApplyMaterialFromIndex();
        Debug.LogWarning("[Fire_Trap_Button_Manager] Guess material applied.");

        SendCustomEventDelayedSeconds(nameof(RevertToBaseMaterial), guessMaterialDuration);
    }

    public void ApplyMaterialFromIndex()
    {
        Debug.LogWarning("[Fire_Trap_Button_Manager] Applying material with index: " + syncedMaterialIndex);

        switch (syncedMaterialIndex)
        {
            case BASE_MATERIAL:
                ChangeMeshMaterial(baseMaterial);
                break;
            case GUESS_MATERIAL:
                ChangeMeshMaterial(guessMaterial);
                break;
            case INCORRECT_MATERIAL:
                ChangeMeshMaterial(incorrectMaterial);
                break;
            case CORRECT_MATERIAL:
                ChangeMeshMaterial(correctMaterial);
                break;
        }
    }

    public void RevertToBaseMaterial()
    {
        if (!puzzleComplete)
        {
            syncedMaterialIndex = BASE_MATERIAL;
            RequestSerialization(); // Sync the change across the network
            ApplyMaterialFromIndex();
            Debug.LogWarning("[Fire_Trap_Button_Manager] Reverting to base material.");
        }
    }

    public void DeactivateVFX()
    {
        TakeOwnership(); // Ensure the player owns the object to sync changes

        isObjectActive = false; // Deactivate the objects (they start as active)
        RequestSerialization(); // Sync deactivation across all players
        UpdateObjectStates(); // Deactivate the objects (VFX)
        Debug.LogWarning("[Fire_Trap_Button_Manager] VFX deactivated.");
    }

    public void ReactivateVFX()
    {
        TakeOwnership(); // Ensure the player owns the object to sync changes

        isObjectActive = true; // Reactivate the objects after the correct solution
        RequestSerialization(); // Sync reactivation across all players
        UpdateObjectStates(); // Reactivate the objects (VFX)
        Debug.LogWarning("[Fire_Trap_Button_Manager] VFX reactivated.");
    }

    public void UpdateObjectStates()
    {
        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                obj.SetActive(isObjectActive); // Sync active/inactive state across all players
                Debug.LogWarning("[Fire_Trap_Button_Manager] Object state updated: " + obj.name + " Active: " + isObjectActive);
            }
        }
    }

    public void FlashDuringReset()
    {
        if (remainingFlashes <= 0)
        {
            isFlashingSynced = false;
            RequestSerialization();
            syncedMaterialIndex = BASE_MATERIAL;
            ApplyMaterialFromIndex();
            Debug.LogWarning("[Fire_Trap_Button_Manager] Flashing ended. Resetting material.");
            return;
        }

        // Alternate between incorrect material and base material
        flashToggle = !flashToggle;
        syncedMaterialIndex = flashToggle ? INCORRECT_MATERIAL : BASE_MATERIAL;
        RequestSerialization();
        ApplyMaterialFromIndex();

        remainingFlashes--; // Decrease remaining flashes
        Debug.LogWarning("[Fire_Trap_Button_Manager] Flashing during reset. Remaining flashes: " + remainingFlashes);
        SendCustomEventDelayedSeconds(nameof(FlashDuringReset), flashInterval); // Continue flashing
    }

    public void ChangeMeshMaterial(Material newMaterial)
    {
        if (backgroundMesh != null)
        {
            backgroundMesh.material = newMaterial;
            Debug.LogWarning("[Fire_Trap_Button_Manager] Mesh material changed to: " + newMaterial.name);
        }
    }

    public void ResetPuzzle()
    {
        TakeOwnership(); // Ensure the player owns the object to sync changes

        ResetPattern();
        UpdateObjectStates(); // Ensure objects are reactivated as needed
        syncedMaterialIndex = BASE_MATERIAL;
        RequestSerialization(); // Sync reset across all players
        ApplyMaterialFromIndex();
        Debug.LogWarning("[Fire_Trap_Button_Manager] Puzzle reset.");
    }

    public void ResetPattern()
    {
        currentGlobalGuessIndex = 0; // Reset the global guess index
        for (int i = 0; i < syncedGuessIDs.Length; i++)
        {
            syncedGuessIDs[i] = -1; // Clear the global pattern (using -1 for empty spots)
            playerPattern[i] = -1; // Clear the player pattern
        }
        puzzleComplete = false; // Reset the puzzle state
        Debug.LogWarning("[Fire_Trap_Button_Manager] Pattern reset.");
    }

    public override void OnDeserialization()
    {
        ApplyMaterialFromIndex();
        UpdateObjectStates();
        Debug.LogWarning("[Fire_Trap_Button_Manager] OnDeserialization called. State updated.");
    }

    private void TakeOwnership()
    {
        if (!Networking.IsOwner(localPlayer, gameObject))
        {
            Networking.SetOwner(localPlayer, gameObject); // Transfer ownership to the interacting player
            Debug.LogWarning("[Fire_Trap_Button_Manager] Ownership transferred to: " + localPlayer.displayName);
        }
    }
}
