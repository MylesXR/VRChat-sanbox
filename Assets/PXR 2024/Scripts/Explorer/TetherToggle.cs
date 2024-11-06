using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class TetherToggle : UdonSharpBehaviour
{
    public KeyCode toggleKey = KeyCode.T; // Key to press for toggling
    public GameObject TetherObjectPC; // Reference to the child GameObject to toggle
    public PlayerManager playerManager;

    private bool isOwner = false;
    private VRCPlayerApi localPlayer;

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
        if (TetherObjectPC == null)
        {
            Debug.LogError("Tether object reference is not set!");
            enabled = false; // Disable the script if the TetherObjectPC is not assigned
            return;
        }

        TetherObjectPC.SetActive(false);
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
        ToggleTetherVisibility();
    }

    private void ToggleTetherVisibility()
    {
        if (localPlayer != null && playerManager.GetPlayerClass(localPlayer) == "Explorer")
        {
            IsVisibleTether = !IsVisibleTether;
            RequestSerialization();
        }
    }

    public void CustomUpdateSeconds()
    {
        if (localPlayer != null && playerManager.GetPlayerClass(localPlayer) != "Explorer")
        {
            isVisibleTether = false;
        }

        UpdateVisibility();
        SendCustomEventDelayedSeconds(nameof(CustomUpdateSeconds), 1.5f);
    }

    public void UpdateVisibility()
    {
        SetVisibility(isVisibleTether);
    }

    public override void OnDeserialization()
    {
        SetVisibility(isVisibleTether);
    }

    private void SetVisibility(bool visible)
    {
        TetherObjectPC.SetActive(visible);
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
            TetherObjectPC.SetActive(false);
        }
    }
}
