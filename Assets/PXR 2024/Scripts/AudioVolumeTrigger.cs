using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AudioVolumeTrigger : UdonSharpBehaviour
{
    // Public field to adjust the volume value when the player enters the trigger
    public float newVolume = 1.0f; // Set the desired volume (between 0.0f and 1.0f)

    public AudioSource audioSource;


    // This method is triggered when the player enters the collider
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            if (audioSource != null)
            {
                // Set the volume of the AudioSource to the new volume value
                audioSource.volume = newVolume;
                Debug.Log("Player entered trigger, changing volume to: " + newVolume);
            }
        }
        
    }
}
