using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)] // Manual sync for controlled networking
public class Fire_Trap_Button_Manager : UdonSharpBehaviour
{
    [Header("Arrays")]
    public GameObject[] correctPattern; // Correct order of buttons
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

    private GameObject[] playerPattern; // Store player's input pattern
    private int currentIndex = 0; // Tracks player's progress in the pattern
    private bool puzzleComplete = false; // Puzzle state

    [UdonSynced] private int syncedMaterialIndex = 0; // Sync material changes across network
    private const int BASE_MATERIAL = 0;
    private const int GUESS_MATERIAL = 1;
    private const int INCORRECT_MATERIAL = 2;
    private const int CORRECT_MATERIAL = 3;

    [UdonSynced] private bool isObjectActive = true; // Objects start active
    [UdonSynced] private bool isFlashingSynced; // Sync flashing state across players
    private bool flashToggle = false; // Tracks the flashing state
    private int remainingFlashes; // Remaining flashes for the current incorrect pattern
    private VRCPlayerApi localPlayer;

    void Start()
    {
        localPlayer = Networking.LocalPlayer;
        playerPattern = new GameObject[maxGuesses]; // Initialize player's pattern array

        if (backgroundMesh != null)
        {
            baseMaterial = backgroundMesh.material; // Save the base material
        }

        ApplyMaterialFromIndex(); // Ensure material is applied properly for all players
        UpdateObjectStates(); // Sync object states (starts as active)
    }

    public void RegisterButtonPress(GameObject button)
    {
        if (puzzleComplete || currentIndex >= maxGuesses) return;

        playerPattern[currentIndex] = button;
        currentIndex++;

        ShowGuessMaterial();

        if (currentIndex == maxGuesses)
        {
            CheckPattern();
        }
    }

    private void CheckPattern()
    {
        bool isCorrect = true;

        for (int i = 0; i < correctPattern.Length; i++)
        {
            if (playerPattern[i] == null || playerPattern[i] != correctPattern[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            HandleCorrectPattern();
        }
        else
        {
            HandleIncorrectPattern();
        }
    }

    private void HandleCorrectPattern()
    {
        puzzleComplete = true;
        syncedMaterialIndex = CORRECT_MATERIAL; // Sync the correct material
        RequestSerialization(); // Sync across all players
        ApplyMaterialFromIndex();

        DeactivateVFX(); // Deactivate the objects (they were active)

        SendCustomEventDelayedSeconds(nameof(ReactivateVFX), vfxActiveTime); // Reactivate after the timed amount
        SendCustomEventDelayedSeconds(nameof(ResetPuzzle), resetDelay); // Reset puzzle after a delay
    }

    private void HandleIncorrectPattern()
    {
        remainingFlashes = flashCount;
        isFlashingSynced = true;
        RequestSerialization();
        FlashDuringReset(); // Start flashing
        SendCustomEventDelayedSeconds(nameof(ResetPuzzle), resetDelay); // Reset puzzle after flashing
    }

    public void ShowGuessMaterial()
    {
        if (Networking.IsOwner(localPlayer, gameObject))
        {
            syncedMaterialIndex = GUESS_MATERIAL;
            RequestSerialization(); // Sync the material index
            ApplyMaterialFromIndex();
        }

        SendCustomEventDelayedSeconds(nameof(RevertToBaseMaterial), guessMaterialDuration);
    }

    public void ApplyMaterialFromIndex()
    {
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
        }
    }

    public void DeactivateVFX()
    {
        if (Networking.IsOwner(localPlayer, gameObject))
        {
            isObjectActive = false; // Deactivate the objects (they start as active)
            RequestSerialization(); // Sync deactivation across all players
            UpdateObjectStates(); // Deactivate the objects (VFX)
        }
    }

    public void ReactivateVFX()
    {
        if (Networking.IsOwner(localPlayer, gameObject))
        {
            isObjectActive = true; // Reactivate the objects after the correct solution
            RequestSerialization(); // Sync reactivation across all players
            UpdateObjectStates(); // Reactivate the objects (VFX)
        }
    }

    public void UpdateObjectStates()
    {
        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                obj.SetActive(isObjectActive); // Sync active/inactive state across all players
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
            return;
        }

        // Alternate between incorrect material and base material
        flashToggle = !flashToggle;
        syncedMaterialIndex = flashToggle ? INCORRECT_MATERIAL : BASE_MATERIAL;
        RequestSerialization();
        ApplyMaterialFromIndex();

        remainingFlashes--; // Decrease remaining flashes
        SendCustomEventDelayedSeconds(nameof(FlashDuringReset), flashInterval); // Continue flashing
    }

    public void ChangeMeshMaterial(Material newMaterial)
    {
        if (backgroundMesh != null)
        {
            backgroundMesh.material = newMaterial;
        }
    }

    public void ResetPuzzle()
    {
        if (Networking.IsOwner(localPlayer, gameObject))
        {
            ResetPattern();
            UpdateObjectStates(); // Ensure objects are reactivated as needed
            syncedMaterialIndex = BASE_MATERIAL;
            RequestSerialization(); // Sync reset across all players
            ApplyMaterialFromIndex();
        }
    }

    public void ResetPattern()
    {
        currentIndex = 0;
        for (int i = 0; i < playerPattern.Length; i++)
        {
            playerPattern[i] = null;
        }
        puzzleComplete = false; // Reset the puzzle state
    }

    public override void OnDeserialization()
    {
        ApplyMaterialFromIndex();
        UpdateObjectStates();
    }
}
