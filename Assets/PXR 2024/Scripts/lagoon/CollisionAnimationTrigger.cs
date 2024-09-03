using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CollisionAnimationTrigger : UdonSharpBehaviour
{
    public Animator animator; // Reference to the Animator component
    public int colliderTag; // Public integer to identify specific colliders
    public string animationTriggerName; // The name of the trigger parameter in the Animator

    [UdonSynced] private bool isAnimationPlaying = false; // Synced variable to track animation state

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has a ColliderTag script with a matching colliderTag
        ColliderTag otherTag = collision.gameObject.GetComponent<ColliderTag>();
        if (otherTag != null && otherTag.colliderTag == colliderTag && !isAnimationPlaying)
        {
            // Ensure the local player is the owner before triggering the animation
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            PlayAnimation();
        }
    }

    private void PlayAnimation()
    {
        isAnimationPlaying = true;
        animator.SetTrigger(animationTriggerName); // Play the animation by setting the trigger
        RequestSerialization(); // Sync the state across the network
    }

    public override void OnDeserialization()
    {
        // When the synced data changes, this method is called, and we sync the animation state
        if (isAnimationPlaying)
        {
            animator.SetTrigger(animationTriggerName); // Ensure the animation is triggered for all players
        }
    }
}
