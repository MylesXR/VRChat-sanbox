using UdonSharp;
using UnityEngine;

public class AnimationEvent_PlaySound : UdonSharpBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudioClip()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}