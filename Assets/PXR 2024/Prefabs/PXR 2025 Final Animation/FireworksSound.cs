using UdonSharp;
using UnityEngine;

public class FireworksSound : UdonSharpBehaviour
{
    public ParticleSystem[] particleSystems;
    public AudioSource[] audioSources;

    [Range(0.5f, 2.0f)] public float minPitch = 0.8f;
    [Range(0.5f, 2.0f)] public float maxPitch = 1.2f;
    [Range(0.5f, 1.0f)] public float minVolume = 0.7f;
    [Range(0.5f, 1.0f)] public float maxVolume = 1.0f;

    void Update()
    {
        for (int i = 0; i < particleSystems.Length; i++)
        {
            if (particleSystems[i].particleCount > 0 && !audioSources[i].isPlaying)
            {
                audioSources[i].pitch = Random.Range(minPitch, maxPitch);
                audioSources[i].volume = Random.Range(minVolume, maxVolume);
                audioSources[i].Play();
            }
        }
    }
}
