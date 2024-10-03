
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BreakableObject : UdonSharpBehaviour
{
    public Rigidbody[] rbs;
    public Collider sphereCollider; // Reference to the animated sphere's collider
    public int breakerLayer; // The layer to identify the breaker
    public Animator animator;

    void Start()
    {
        // Disable animator and all rigidbodies' physics until triggered
        animator.enabled = false; // Disable animation on start
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true; // Disable physics interactions
            rb.useGravity = false; // Disable gravity initially
        }
        sphereCollider.enabled = true; // Ensure the collider is enabled at the start
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is on the correct layer
        if (other.gameObject.layer == breakerLayer)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayBreakAnimation");
        }
    }

    public void PlayBreakAnimation()
    {
        // Enable animation and break the object
        animator.enabled = true;
        animator.SetTrigger("PlayAnimation");

        // Delay physics activation for stability in VRChat builds
        SendCustomEventDelayedSeconds("EnablePhysics", 0.5f); // Adjust delay as needed
    }

    public void EnablePhysics()
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false; // Enable physics
            rb.useGravity = true;   // Re-enable gravity
        }

        // Disable the collider after the animation is complete
        SendCustomEventDelayedSeconds("DisableCollider", 2.0f); // Adjust delay based on animation length
    }

    public void DisableCollider()
    {
        sphereCollider.enabled = false; // Disable the collider
        Debug.LogWarning("Collider disabled after explosion.");
    }
}
