using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class AxeToggle : UdonSharpBehaviour
{
    public KeyCode toggleKey = KeyCode.T; // Key to press for toggling

    // Reference to the child GameObject to toggle
    public GameObject axeObject;

    public PlayerManager playerManager;

    // Synced variable to track visibility state across network
    [UdonSynced, FieldChangeCallback(nameof(IsVisible))]
    private bool isVisible = false;

    private bool hasBeenEnabled = false;

    private VRCPlayerApi localPlayer;

    private int ownershipTransferCount = 0;

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

        // Start the enum for custom update cycle
        CustomUpdateSeconds();
    }

    private void OnEnable()
    {
        if(!hasBeenEnabled)
        {
            axeObject.SetActive(false);
            hasBeenEnabled = true;
        }
    }

    void Update()
    {
        if (Networking.IsOwner(gameObject)) // Check if the local player owns this object
        {

            if (Input.GetKeyDown(toggleKey))
            {
                
                // Check if the local player is a Barbarian
                VRCPlayerApi localPlayer = Networking.LocalPlayer;
                if (localPlayer != null)
                {
                    string playerClass = playerManager.GetPlayerClass(localPlayer);
                    if (playerClass == "Barbarian")
                    {
                        Debug.Log("[AxeToggle] Player is a Barbarian. Toggling visibility.");

                        // Toggle visibility locally and sync across network
                        IsVisible = !IsVisible;

                        // Request serialization to sync with other players
                        RequestSerialization();
                    }
                    else
                    {
                        Debug.Log("[AxeToggle] Player is not a Barbarian. Visibility not changed.");
                    }
                }
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
        // Ensure the new player receives the correct visibility state
        //if (Networking.IsOwner(gameObject))
        //{
            axeObject.SetActive(false);
            RequestSerialization();
        //}
    }
    public override void OnOwnershipTransferred(VRCPlayerApi newOwner)
    {
        ownershipTransferCount++;
        Debug.Log($"[AxeToggle] Ownership transferred to: {newOwner.displayName}. Transfer count: {ownershipTransferCount}");

        // Check if the transfer count exceeds 1
        if (ownershipTransferCount > 0)
        {
            Debug.Log("[AxeToggle] Ownership transferred more than once. Destroying the object.");
            Destroy(gameObject); // Destroy the game object
            return;
        }
    }
    
}
