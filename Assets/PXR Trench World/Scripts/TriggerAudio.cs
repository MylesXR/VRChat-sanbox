using UnityEngine;

public class TriggerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // Reference to the audio source
    [SerializeField] private float delayBeforePlaying = 2.0f; // Delay in seconds
    [SerializeField] private float fadeOutTime = 1.0f; // Time for the audio to fade out in seconds
    [SerializeField] private float fadeOutStep = 0.1f; // Volume decrease per step

    private bool isInsideTrigger = false; // To track if something is inside the trigger

    private void OnTriggerEnter(Collider other)
    {
        // Start playing the audio loop after a delay
        isInsideTrigger = true;
        Invoke("StartAudioLoop", delayBeforePlaying);
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop the audio loop and fade out
        isInsideTrigger = false;
        CancelInvoke("FadeOutStep"); // Cancel any ongoing fade-out
        InvokeRepeating("FadeOutStep", 0, fadeOutTime / (1 / fadeOutStep));
    }

    private void StartAudioLoop()
    {
        if (isInsideTrigger) // Double-check in case the object left the trigger before the delay
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void FadeOutStep()
    {
        if (audioSource.volume > 0)
        {
            audioSource.volume -= fadeOutStep;
        }
        else
        {
            CancelInvoke("FadeOutStep");
            audioSource.Stop();
            audioSource.volume = 1.0f; // Reset volume for next time
        }
    }
}
