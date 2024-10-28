using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Components;

public class TetherToggle : UdonSharpBehaviour
{
    public KeyCode toggleKey = KeyCode.T; // Key to press for toggling

    public VRCObjectPool ThetherPool;
    // Reference to the child GameObject to toggle
    public GameObject TetherObjectPC;

    public PlayerManager playerManager;

    private bool hasBeenEnabled = false;

    private VRCPlayerApi localPlayer;

    private int ownershipTransferCount = 0;

    private bool isOwner;
    [UdonSynced, FieldChangeCallback(nameof(IsVisibleTether))]
    private bool isVisibleTether = false;
    private bool IsVisibleTether
    {
        get => isVisibleTether;
        set
        {
            isVisibleTether = value;
            SetVisibility(isVisibleTether);
        }
    }

    void Start()
    {
        // Ensure the axeObject is set
        if (TetherObjectPC == null)
        {
            Debug.LogError("Axe object reference is not set!");
            enabled = false; // Disable the script if the axeObject is not assigned
            return;
        }
        TetherObjectPC.SetActive(false);

        localPlayer = Networking.GetOwner(gameObject);
        localPlayer = Networking.LocalPlayer;
        UpdateOwner();
        // Start the enum for custom update cycle
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

        ToggleTetherVisibility();
    }

    private void ToggleTetherVisibility()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer != null)
        {
            string playerClass = playerManager.GetPlayerClass(localPlayer);
            if (playerClass == "Explorer")
            {
                IsVisibleTether = !IsVisibleTether;
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
            if (playerClass != "Explorer")
            {
                isVisibleTether = false;
            }
        }

        UpdateVisibility();

        // Schedule the next call
        SendCustomEventDelayedSeconds(nameof(CustomUpdateSeconds), 1.5f);
    }

    public void UpdateVisibility()
    {
        SetVisibility(isVisibleTether);
    }
    public override void OnDeserialization()
    {
        // Sync the visibility state when receiving network updates
        //Debug.Log("[AxeToggle] OnDeserialization called. Setting visibility to: " + isVisible);
        SetVisibility(isVisibleTether);
    }

    private void SetVisibility(bool Visible)
    {
        // Set the visibility of the child object
        TetherObjectPC.SetActive(Visible);
        //Debug.Log("[AxeToggle] Axe visibility set to: " + visible);
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        RequestSerialization();
    }


    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (player == localPlayer)
        {
            ThetherPool.Return(gameObject);

            foreach (Transform child in (gameObject.transform))
            {
                ThetherPool.Return(gameObject);
            }
        }
    }

}
