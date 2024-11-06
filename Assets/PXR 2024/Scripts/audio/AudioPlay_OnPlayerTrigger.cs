using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class AudioPlay_OnPlayerTrigger : UdonSharpBehaviour
{
    private float minPitch = 0.75f;   // Minimum pitch range
    private float maxPitch = 1.25f;   // Maximum pitch range
    private float minVolume = 0.40f;  // Minimum volume range
    private float maxVolume = 0.80f;  // Maximum volume range
    private float minSpatialBlend = 0.75f;  // Minimum spatial blend range for 3D effect
    private float maxSpatialBlend = 1.0f;  // Maximum spatial blend range for 3D effect

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.volume = Random.Range(minVolume, maxVolume);
            audioSource.spatialBlend = Random.Range(minSpatialBlend, maxSpatialBlend);
            audioSource.Play();
        }
        else
        {
            //Debug.LogError("No AudioSource component found. Please add an AudioSource component to this GameObject.");
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.Stop();
        }
        else
        {
            //Debug.LogError("No AudioSource component found. Please add an AudioSource component to this GameObject.");
        }
    }
}
