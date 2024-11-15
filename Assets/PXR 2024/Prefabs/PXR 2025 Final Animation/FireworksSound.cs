
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FireworksSound : UdonSharpBehaviour
{
    public ParticleSystem mainParticleSystem; // Assign the root particle system here
    public AudioSource audioSource;           // Assign the AudioSource here

    private ParticleSystem.EmissionModule emissionModule;

    void Start()
    {
        if (mainParticleSystem != null)
        {
            // Set up emission module for tracking particle count if needed
            emissionModule = mainParticleSystem.emission;
        }

        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource is not assigned.");
        }
    }

    void Update()
    {
        // Check if particles are actively being emitted
        if (mainParticleSystem != null && audioSource != null)
        {
            // If particles are emitting, play the audio if not already playing
            if (mainParticleSystem.particleCount > 0 && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}
