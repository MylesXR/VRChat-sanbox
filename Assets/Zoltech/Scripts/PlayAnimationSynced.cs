using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]

public class PlayAnimationSynced : UdonSharpBehaviour
{
    public Animator animator;
    public string keyObject;



    [UdonSynced(UdonSyncMode.None)] // Sync variable without interpolation
    private bool isAnimationPlaying;

    private void Start()
    {
        // Ensure the correct animation state on join
        if (isAnimationPlaying)
        {
            animator.SetTrigger("PlayAnimation");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the collider is the key object
        if (other.gameObject.name == keyObject)
        {
            // Set the animation state and sync it
            TriggerAnimation();
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
        animator.SetTrigger("PlayAnimation");
    }

    public override void OnDeserialization()
    {
        // Play animation if the synced variable indicates it's already playing
        if (isAnimationPlaying)
        {
            animator.SetTrigger("PlayAnimation");
        }
    }
}
