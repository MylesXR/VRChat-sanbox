using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TetherToggle : UdonSharpBehaviour
{
    public KeyCode toggleKey = KeyCode.T; // Key to press for toggling
    public GameObject TetherObjectPC; // Reference to the child GameObject to toggle
    public PlayerManager playerManager;

    private bool isVisibleTether = false;

    void Start()
    {
        if (TetherObjectPC == null)
        {
            Debug.LogError("Tether object reference is not set!");
            enabled = false; // Disable the script if the TetherObjectPC is not assigned
            return;
        }

        // Start with the TetherObjectPC disabled
        TetherObjectPC.SetActive(false);

        // Begin the custom update cycle
        CustomUpdateSeconds();
    }

    void Update()
    {
        // Toggle visibility if the toggle key is pressed
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleTetherVisibility();
        }
    }

    private void ToggleTetherVisibility()
    {
        // Toggle visibility only if the player has the "Explorer" class
        if (playerManager.GetPlayerClass() == "Explorer")
        {
            isVisibleTether = !isVisibleTether;
            SetVisibility(isVisibleTether);
        }
    }

    public void CustomUpdateSeconds()
    {
        // If the player is not "Explorer" class, automatically hide the tether
        if (playerManager.GetPlayerClass() != "Explorer")
        {
            isVisibleTether = false;
        }

        // Update visibility based on the current isVisibleTether state
        SetVisibility(isVisibleTether);

        // Schedule the next call to CustomUpdateSeconds in 1.5 seconds
        SendCustomEventDelayedSeconds(nameof(CustomUpdateSeconds), 1.5f);
    }

    private void SetVisibility(bool visible)
    {
        // Set the visibility of the TetherObjectPC
        TetherObjectPC.SetActive(visible);
    }
}
