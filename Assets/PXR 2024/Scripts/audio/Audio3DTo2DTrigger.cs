
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Audio3DTo2DTrigger : UdonSharpBehaviour
{
    public AudioSource audioSource;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal && audioSource != null)
        {
            // Switch to 2D (spatialBlend = 0) when player enters the trigger
            audioSource.spatialBlend = 0.0f;
            Debug.Log("Player entered trigger, switching audio to 2D.");
        }
    }
}