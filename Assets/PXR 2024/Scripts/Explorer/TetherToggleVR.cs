using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class TetherToggleVR : UdonSharpBehaviour
{
    public KeyCode toggleKey = KeyCode.T; // Key to press for toggling

    // Reference to the child GameObject to toggle
    public GameObject TetherObjectVR;

    public PlayerManager playerManager;

    private bool hasBeenEnabled = false;

    private VRCPlayerApi localPlayer;

    private int ownershipTransferCount = 0;

    private bool isPlayerInVR = false;

    [UdonSynced, FieldChangeCallback(nameof(TetherIsVisibleVR))]
    private bool tetherIsVisibleVR = false;

    private bool TetherIsVisibleVR
    {
        get => tetherIsVisibleVR;
        set
        {
            tetherIsVisibleVR = value;
            SetVisibility(tetherIsVisibleVR);
        }
    }

    void Start()
    {
        // Ensure the axeObject is set
        if (TetherObjectVR == null)
        {
            Debug.LogError("Tether object reference is not set!");
            enabled = false; // Disable the script if the axeObject is not assigned
            return;
        }
        TetherObjectVR.SetActive(false);

        // Start the enum for custom update cycle
        CustomUpdateSeconds();
    }

    //void Update()
    //{
    //    if (Networking.IsOwner(gameObject)) // Check if the local player owns this object
    //    {

    //        if (Input.GetKeyDown(toggleKey))
    //        {

    //            // Check if the local player is a Barbarian
    //            VRCPlayerApi localPlayer = Networking.LocalPlayer;
    //            if (localPlayer != null)
    //            {
    //                string playerClass = playerManager.GetPlayerClass(localPlayer);
    //                if (playerClass == "Barbarian")
    //                {
    //                    Debug.Log("[AxeToggle] Player is a Barbarian. Toggling visibility.");

    //                    // Toggle visibility locally and sync across network
    //                    IsVisibleVR = !IsVisibleVR;

    //                    // Request serialization to sync with other players
    //                    RequestSerialization();
    //                }
    //                else
    //                {
    //                    Debug.Log("[AxeToggle] Player is not a Barbarian. Visibility not changed.");
    //                }
    //            }
    //        }

    //    }
    //}

    public void CustomUpdateSeconds()
    {
        if (Networking.IsOwner(gameObject))
        {
            VRCPlayerApi localPlayer = Networking.LocalPlayer;
            tetherIsVisibleVR = localPlayer.IsUserInVR();
            if (localPlayer != null)
            {
                string playerClass = playerManager.GetPlayerClass(localPlayer);
                if (playerClass != "Explorer")
                {
                    tetherIsVisibleVR = false;

                }
                if (playerClass == "Explorer" && isPlayerInVR)
                {
                    tetherIsVisibleVR = true;
                }
            }

            UpdateVisibility();

            // Schedule the next call
            SendCustomEventDelayedSeconds(nameof(CustomUpdateSeconds), 1.5f);
        }
    }

    public void UpdateVisibility()
    {
        SetVisibility(tetherIsVisibleVR);
    }
    public override void OnDeserialization()
    {
        // Sync the visibility state when receiving network updates
        //Debug.Log("[AxeToggle] OnDeserialization called. Setting visibility to: " + isVisible);
        SetVisibility(tetherIsVisibleVR);
    }

    private void SetVisibility(bool visible)
    {
        // Set the visibility of the child object
        TetherObjectVR.SetActive(visible);
        //Debug.Log("[AxeToggle] Axe visibility set to: " + visible);
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        RequestSerialization();
    }
}
