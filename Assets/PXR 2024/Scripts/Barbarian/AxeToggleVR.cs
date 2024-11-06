using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AxeToggleVR : UdonSharpBehaviour
{
    public GameObject axeObjectVR; // Reference to the VR axe object
    public PlayerManager playerManager;

    private bool isVisibleVR = false;
    private bool isPlayerInVR = false; // Set once at start

    void Start()
    {
        if (axeObjectVR == null)
        {
            Debug.LogError("Axe object reference is not set!");
            enabled = false; // Disable the script if the axeObjectVR is not assigned
            return;
        }

        // Start with the axeObject disabled
        axeObjectVR.SetActive(false);

        // Check if the player is in VR once at start
        isPlayerInVR = Networking.LocalPlayer != null && Networking.LocalPlayer.IsUserInVR();

        // Start the custom update cycle to regularly check the class
        CustomUpdateSeconds();
    }

    public void CustomUpdateSeconds()
    {
        // Check the player's class every 1.5 seconds, update visibility only if both VR and class are correct
        if (isPlayerInVR && playerManager.GetPlayerClass() == "Barbarian")
        {
            isVisibleVR = true;
        }
        else
        {
            isVisibleVR = false;
        }

        UpdateVisibility();

        // Schedule the next call to CustomUpdateSeconds in 1.5 seconds
        SendCustomEventDelayedSeconds(nameof(CustomUpdateSeconds), 1.5f);
    }

    public void UpdateVisibility()
    {
        SetVisibility(isVisibleVR);
    }

    private void SetVisibility(bool visible)
    {
        // Set the visibility of the axeObjectVR
        axeObjectVR.SetActive(visible);
    }
}
