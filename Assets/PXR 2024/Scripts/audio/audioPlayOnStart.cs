using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class audioPlayOnStart : UdonSharpBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // Check if the audio source is found before trying to play it
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource component not found on this GameObject.");
        }
    }
}
