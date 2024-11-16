using UdonSharp;
using UnityEngine;

public class AudioPlay_OnStart : UdonSharpBehaviour
{
    [Header("Audio Settings")][Space(5)]
    [SerializeField] private float minPitch = 0.75f;
    [SerializeField] private float maxPitch = 1.25f;
    [SerializeField] private float minVolume = 0.50f;
    [SerializeField] private float maxVolume = 1f;

    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.volume = Random.Range(minVolume, maxVolume);
            audioSource.Play();
        }
        else
        {
            Debug.LogError("No AudioSource component found. Please add an AudioSource component to this GameObject.");
        }
    }
}