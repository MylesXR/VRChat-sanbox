using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayAudioAnimationEvent : UdonSharpBehaviour
{
    public AudioSource audioSource; // Assign your AudioSource in the Inspector

    public void PlaySound()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
