using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class AudioPlay_OnPlayerTrigger : UdonSharpBehaviour
{
    [SerializeField] private float minPitch = 0.75f;   // Minimum pitch range
    [SerializeField] private float maxPitch = 1.25f;   // Maximum pitch range
    [SerializeField] private float minVolume = 0.50f;  // Minimum volume range
    [SerializeField] private float maxVolume = 0.75f;  // Maximum volume range
    [SerializeField] private float minSpatialBlend = 0.75f;  // Minimum spatial blend range for 3D effect
    [SerializeField] private float maxSpatialBlend = 1.0f;  // Maximum spatial blend range for 3D effect   

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        // Get all AudioSource components on the GameObject
        AudioSource[] audioSources = GetComponents<AudioSource>();

        // Check if there are any AudioSources on the GameObject
        if (audioSources.Length > 0)
        {
            foreach (AudioSource audioSource in audioSources)
            {
                // Apply random settings to each AudioSource
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.volume = Random.Range(minVolume, maxVolume);
                audioSource.spatialBlend = Random.Range(minSpatialBlend, maxSpatialBlend);
                audioSource.Play();
            }
        }
        else
        {
            // Debug.LogError("No AudioSource components found. Please add AudioSource components to this GameObject.");
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        // Stop all AudioSources on the GameObject
        AudioSource[] audioSources = GetComponents<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Stop();
        }
    }
}
