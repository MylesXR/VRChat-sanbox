using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AudioPlay_OnStart : UdonSharpBehaviour
{
    public float minPitch = 0.8f;   // Minimum pitch range
    public float maxPitch = 1.2f;   // Maximum pitch range

    void Start()
    {
        // Get the AudioSource component on this GameObject
        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            // Set a random pitch within the defined range
            audioSource.pitch = Random.Range(minPitch, maxPitch);

            // Play the audio at the randomized pitch
            audioSource.Play();
        }
        else
        {
            Debug.LogError("No AudioSource component found. Please add an AudioSource component to this GameObject.");
        }
    }
}