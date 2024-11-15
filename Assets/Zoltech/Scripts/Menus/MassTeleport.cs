using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class MassTeleport : UdonSharpBehaviour
{
    [UdonSynced] private Vector3 targetPosition;

    void Update()
    {
        // Only the instance owner should update the target position to their head position
        if (Networking.IsMaster)
        {
            // Update the target position to the instance owner's head position (locally only)
            targetPosition = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        }
    }

    public override void Interact()
    {
        // Ensure the instance owner has the latest head position before requesting serialization
        if (Networking.IsMaster)
        {
            RequestSerialization();
        }

        // Send the teleport event to all players
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(TeleportAllPlayers));
    }

    public void TeleportAllPlayers()
    {
        if (Networking.LocalPlayer != null)
        {
            // Teleport each player to the synchronized target position
            Networking.LocalPlayer.TeleportTo(targetPosition, Networking.LocalPlayer.GetRotation());
        }
    }

    //public override void OnDeserialization()
    //{
    //    // Ensure all clients update their position upon receiving the new target position
    //    TeleportAllPlayers();
    //}
}
