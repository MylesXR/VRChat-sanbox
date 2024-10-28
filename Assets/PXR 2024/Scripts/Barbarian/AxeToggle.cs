using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Components;

public class AxeToggle : UdonSharpBehaviour
{
    public KeyCode toggleKey = KeyCode.T; // Key to press for toggling

    public VRCObjectPool axePool;
    // Reference to the child GameObject to toggle
    public GameObject axeObject;

    public PlayerManager playerManager;

    private bool hasBeenEnabled = false;

    private bool isOwner = false;
    private VRCPlayerApi localPlayer;

    private int ownershipTransferCount = 0;

    [UdonSynced, FieldChangeCallback(nameof(IsVisible))]
    private bool isVisible = false;
    private bool IsVisible
    {
        get => isVisible;
        set
        {
            isVisible = value;
            SetVisibility(isVisible);
        }
    }

    void Start()
    {
        // Ensure the axeObject is set
        if (axeObject == null)
        {
            Debug.LogError("Axe object reference is not set!");
            enabled = false; // Disable the script if the axeObject is not assigned
            return;
        }
        axeObject.SetActive(false);

        localPlayer = Networking.GetOwner(gameObject);
        // Start the enum for custom update cycle
        localPlayer = Networking.LocalPlayer;
        UpdateOwner();
        CustomUpdateSeconds();
    }
    public override void OnOwnershipTransferred(VRCPlayerApi newOwner)
    {
        UpdateOwner();
    }

    private void UpdateOwner()
    {
        isOwner = Networking.IsOwner(gameObject);
    }
    void Update()
    {
        // Early exit if the player is not the owner
        if (!isOwner) return;

        // Early exit if the toggle key hasn't been pressed
        if (!Input.GetKeyDown(toggleKey)) return;

        // If both conditions are met, then proceed to check the player class and toggle visibility
        ToggleAxeVisibility();
    }
    private void ToggleAxeVisibility()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer != null)
        {
            string playerClass = playerManager.GetPlayerClass(localPlayer);
            if (playerClass == "Barbarian")
            {
                IsVisible = !IsVisible;
                RequestSerialization();
            }
        }
    }

    public void CustomUpdateSeconds()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer != null)
        {
            string playerClass = playerManager.GetPlayerClass(localPlayer);
            if (playerClass != "Barbarian")
            {
                isVisible = false;

            }
        }

        UpdateVisibility();

        // Schedule the next call
        SendCustomEventDelayedSeconds(nameof(CustomUpdateSeconds), 1.5f);
    }

    public void UpdateVisibility()
    {
        SetVisibility(isVisible);
    }
    public override void OnDeserialization()
    {
        // Sync the visibility state when receiving network updates
        //Debug.Log("[AxeToggle] OnDeserialization called. Setting visibility to: " + isVisible);
        SetVisibility(isVisible);
    }

    private void SetVisibility(bool visible)
    {
        // Set the visibility of the child object
        axeObject.SetActive(visible);
        //Debug.Log("[AxeToggle] Axe visibility set to: " + visible);
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        RequestSerialization();
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if(player == localPlayer)
        {
            axePool.Return(gameObject);
            
            foreach(Transform child in (gameObject.transform))
            {
                axePool.Return(gameObject);
            }
        }
    }

}
