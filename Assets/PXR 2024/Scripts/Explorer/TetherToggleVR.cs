using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TetherToggleVR : UdonSharpBehaviour
{
    // Reference to the child GameObject to toggle
    public GameObject TetherObjectVR;
    public PlayerManager playerManager;

    private bool isPlayerInVR = false;
    private bool tetherIsVisibleVR = false;

    void Start()
    {
        // Ensure the TetherObjectVR is set
        if (TetherObjectVR == null)
        {
            Debug.LogError("Tether object reference is not set!");
            enabled = false; // Disable the script if TetherObjectVR is not assigned
            return;
        }

        TetherObjectVR.SetActive(false);

        // Set VR status once at start
        isPlayerInVR = Networking.LocalPlayer != null && Networking.LocalPlayer.IsUserInVR();

        // Start the custom update cycle
        CustomUpdateSeconds();
    }

    public void CustomUpdateSeconds()
    {
        // Check the player's class every 1.5 seconds and update visibility if both conditions are met
        if (isPlayerInVR && playerManager.GetPlayerClass() == "Explorer")
        {
            tetherIsVisibleVR = true;
        }
        else
        {
            tetherIsVisibleVR = false;
        }

        UpdateVisibility();

        // Schedule the next call to CustomUpdateSeconds in 1.5 seconds
        SendCustomEventDelayedSeconds(nameof(CustomUpdateSeconds), 1.5f);
    }

    public void UpdateVisibility()
    {
        SetVisibility(tetherIsVisibleVR);
    }

    private void SetVisibility(bool visible)
    {
        // Set the visibility of the TetherObjectVR
        TetherObjectVR.SetActive(visible);
    }
}
