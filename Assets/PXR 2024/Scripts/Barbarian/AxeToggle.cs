using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AxeToggle : UdonSharpBehaviour
{
    public KeyCode toggleKey = KeyCode.T; // Key to press for toggling
    public GameObject axeObject; // Reference to the child GameObject to toggle
    public PlayerManager playerManager;

    private bool isVisible = false;

    void Start()
    {
        if (axeObject == null)
        {
            Debug.LogError("Axe object reference is not set!");
            enabled = false; // Disable the script if the axeObject is not assigned
            return;
        }

        // Start with the axeObject disabled
        axeObject.SetActive(false);

        // Begin the custom update cycle
        CustomUpdateSeconds();
    }

    void Update()
    {
        // Toggle visibility if the toggle key is pressed
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleAxeVisibility();
        }
    }

    private void ToggleAxeVisibility()
    {
        // Toggle visibility only if the player has the "Barbarian" class
        if (playerManager.GetPlayerClass() == "Barbarian")
        {
            isVisible = !isVisible;
            SetVisibility(isVisible);
        }
    }

    public void CustomUpdateSeconds()
    {
        // If the player is not "Barbarian" class, automatically hide the axe
        if (playerManager.GetPlayerClass() != "Barbarian")
        {
            isVisible = false;
            SetVisibility(isVisible);
        }

        // Schedule the next call to CustomUpdateSeconds in 1.5 seconds
        SendCustomEventDelayedSeconds(nameof(CustomUpdateSeconds), 1.5f);
    }

    private void SetVisibility(bool visible)
    {
        // Set the visibility of the axeObject
        axeObject.SetActive(visible);
    }
}
