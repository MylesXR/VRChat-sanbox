using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Breakable_Environment_Object : UdonSharpBehaviour
{
    public Rigidbody[] rbs;
    public Collider[] objectColliders; // Colliders of the objects that will break
    public Collider animationCollider; // Collider used for the animation, set in Inspector
    public Animator animator;
    public float deactivationTime = 5.0f; // Time in seconds before objects get deactivated
    public float animationDuration = 2.0f; // Duration of the break animation

    void Start()
    {
        // Disable animator and all rigidbodies' physics until triggered
        animator.enabled = false; // Disable animation on start
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true; // Disable physics interactions
            rb.useGravity = false; // Disable gravity initially
        }

        // Ensure all object colliders are enabled at the start
        foreach (Collider col in objectColliders)
        {
            col.enabled = true;
        }

        // Ensure the animation collider is enabled at the start
        if (animationCollider != null)
        {
            animationCollider.enabled = true;
        }
    }

    // Call this function to start the breaking process
    public void BreakObject()
    {
        // Start the animation first and wait for it to finish before enabling physics
        animator.enabled = true;
        PlayStalactiteAnimation();
        SendCustomEventDelayedSeconds("PlayBridgeBreakingAnimation", 5f);
        SendCustomEventDelayedSeconds("EnablePhysicsAndBreak", animationDuration); // Wait for the animation to finish
    }


    public void PlayStalactiteAnimation()
    {
        animator.SetTrigger("PlayStalactiteAnimation");
        SendCustomEventDelayedSeconds("Idle", 5f);
    }

    public void PlayBridgeBreakingAnimation()
    {
        animator.SetTrigger("PlayAnimation");
        SendCustomEventDelayedSeconds("Idle", 5f);
    }

    public void PlayIdleAnimation()
    {
        animator.SetTrigger("Idle");
    }


    public void EnablePhysicsAndBreak()
    {
        // Only now, after the animation has finished, enable physics on all the rigidbodies
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false; // Enable physics
            rb.useGravity = true;   // Re-enable gravity so the pieces can fall
        }

        // Set objects to inactive after a delay
        SendCustomEventDelayedSeconds("DeactivateObjects", deactivationTime);
    }



    public void DeactivateObjects()
    {
        foreach (Collider col in objectColliders)
        {
            col.gameObject.SetActive(false); // Set the objects inactive after breaking
        }

        Debug.LogWarning("Exploded objects have been set inactive.");
    }
}
