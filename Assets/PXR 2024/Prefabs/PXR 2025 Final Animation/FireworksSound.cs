
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FireworksSound : UdonSharpBehaviour
{
    public AudioSource audioSource;  // Reference to the audio source to play

    private void OnParticleCollision(GameObject other)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
