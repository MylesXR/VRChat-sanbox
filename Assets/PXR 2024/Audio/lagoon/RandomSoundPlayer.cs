using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RandomSoundPlayer : UdonSharpBehaviour
{
    public AudioSource audioSource;    // The AudioSource component to play sounds
    public AudioClip[] sounds;         // Array of sounds to play
    public float minInterval = 30f;    // Minimum interval between sounds
    public float maxInterval = 45f;    // Maximum interval between sounds

    private void Start()
    {
        // Schedule the first play event
        ScheduleNextPlay();
    }

    public void PlayRandomSound()
    {
        if (sounds.Length > 0)
        {
            // Choose a random sound and play it
            AudioClip clip = sounds[Random.Range(0, sounds.Length)];
            audioSource.PlayOneShot(clip);
        }

        // Schedule the next play after playing a sound
        ScheduleNextPlay();
    }

    private void ScheduleNextPlay()
    {
        // Calculate a random delay between minInterval and maxInterval
        float randomDelay = Random.Range(minInterval, maxInterval);

        // Schedule the PlayRandomSound event with the random delay
        SendCustomEventDelayedSeconds(nameof(PlayRandomSound), randomDelay);
    }
}
