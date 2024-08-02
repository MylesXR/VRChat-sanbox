using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerSpeedZone : UdonSharpBehaviour
{
    [Header("Original Speeds")]
    private float originalWalkSpeed = 4.5f;
    private float originalRunSpeed = 7f;
    private float originalStrafeSpeed = 3f;
    private float originalJumpImpulse = 3f;
    private float originalGravityStrength = 1f;

    [Header("Slowed Speeds")]
    private float slowedWalkSpeed = 3.0f;
    private float slowedRunSpeed = 5.0f;
    private float slowedStrafeSpeed = 2.0f;
    private float slowedJumpImpulse = 0.5f;
    private float increasedGravityStrength = 3f;

    [Header("Debug Menu")]
    public DebugMenu debugMenu;

    private VRCPlayerApi localPlayer;

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        if (localPlayer != null)
        {
            // Log initial speeds
            LogInitialSpeeds();
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            ApplySlowedSpeeds();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ApplySlowedSpeeds));
            debugMenu.Log("Player entered speed zone. Speeds slowed.");
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            RestoreOriginalSpeeds();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(RestoreOriginalSpeeds));
            debugMenu.Log("Player exited speed zone. Speeds restored.");
        }
    }

    public void ApplySlowedSpeeds()
    {
        if (localPlayer != null)
        {
            localPlayer.SetWalkSpeed(slowedWalkSpeed);
            localPlayer.SetRunSpeed(slowedRunSpeed);
            localPlayer.SetStrafeSpeed(slowedStrafeSpeed);
            localPlayer.SetJumpImpulse(slowedJumpImpulse);
            localPlayer.SetGravityStrength(increasedGravityStrength);
        }
    }

    public void RestoreOriginalSpeeds()
    {
        if (localPlayer != null)
        {
            localPlayer.SetWalkSpeed(originalWalkSpeed);
            localPlayer.SetRunSpeed(originalRunSpeed);
            localPlayer.SetStrafeSpeed(originalStrafeSpeed);
            localPlayer.SetJumpImpulse(originalJumpImpulse);
            localPlayer.SetGravityStrength(originalGravityStrength);
        }
    }

    private void LogInitialSpeeds()
    {
        debugMenu.Log($"Initial Walk Speed: {originalWalkSpeed}");
        debugMenu.Log($"Initial Run Speed: {originalRunSpeed}");
        debugMenu.Log($"Initial Strafe Speed: {originalStrafeSpeed}");
        debugMenu.Log($"Initial Jump Impulse: {originalJumpImpulse}");
        debugMenu.Log($"Initial Gravity Strength: {originalGravityStrength}");
    }
}
