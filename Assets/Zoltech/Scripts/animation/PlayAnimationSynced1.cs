using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;



public class PlayAnimationSynced1 : UdonSharpBehaviour
{
    public Animator animator;

    [UdonSynced(UdonSyncMode.None)] // Sync variable without interpolation
    private bool isAnimationPlaying;

    private void Start()
    {
        // Ensure the correct animation state on join
        if (isAnimationPlaying)
        {
            animator.SetTrigger("PlayAnimation1");
        }
    }

    public override void Interact()
    {
        TriggerAnimation();
    }

    private void TriggerAnimation()
    {

        isAnimationPlaying = true; // Set synced variable to true
        RequestSerialization(); // Sync the variable across network

        // Trigger the animation for all players
        AnimationPlay();


    }

    public void AnimationPlay()
    {
        animator.SetTrigger("PlayAnimation1");
    }

    public override void OnDeserialization()
    {
        // Play animation if the synced variable indicates it's already playing
        if (isAnimationPlaying)
        {
            animator.SetTrigger("PlayAnimation1");
        }
    }
}
