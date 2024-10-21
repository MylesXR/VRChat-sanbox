using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Audio2DTo3DTrigger : UdonSharpBehaviour
{
    public AudioSource audioSource;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal && audioSource != null)
        {
            // Instantly switch from 2D (spatialBlend = 0) to 3D (spatialBlend = 1)
            audioSource.spatialBlend = 1.0f;
            Debug.Log("Player entered trigger, switching audio to 3D.");
        }
    }
}
