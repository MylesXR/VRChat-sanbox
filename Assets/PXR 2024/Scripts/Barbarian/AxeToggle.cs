using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class AxeToggle : UdonSharpBehaviour
{
    public KeyCode toggleKey = KeyCode.T; // Key to press for toggling
    public GameObject axeObject; // Reference to the child GameObject to toggle
    public PlayerManager playerManager;

    private bool isOwner = false;
    private VRCPlayerApi localPlayer;

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
        if (axeObject == null)
        {
            Debug.LogError("Axe object reference is not set!");
            enabled = false; // Disable the script if the axeObject is not assigned
            return;
        }

        axeObject.SetActive(false);
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
        if (!isOwner || !Input.GetKeyDown(toggleKey)) return;
        ToggleAxeVisibility();
    }

    private void ToggleAxeVisibility()
    {
        if (localPlayer != null && playerManager.GetPlayerClass(localPlayer) == "Barbarian")
        {
            IsVisible = !IsVisible;
            RequestSerialization();
        }
    }

    public void CustomUpdateSeconds()
    {
        if (localPlayer != null && playerManager.GetPlayerClass(localPlayer) != "Barbarian")
        {
            isVisible = false;
        }

        UpdateVisibility();
        SendCustomEventDelayedSeconds(nameof(CustomUpdateSeconds), 1.5f);
    }

    public void UpdateVisibility()
    {
        SetVisibility(isVisible);
    }

    public override void OnDeserialization()
    {
        SetVisibility(isVisible);
    }

    private void SetVisibility(bool visible)
    {
        axeObject.SetActive(visible);
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        RequestSerialization();
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (player == localPlayer)
        {
            // Optional: handle visibility if the local player leaves
            axeObject.SetActive(false);
        }
    }
}
