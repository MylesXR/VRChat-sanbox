using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BreakableObject : UdonSharpBehaviour
{

    #region Variables 

    public Rigidbody[] rbs; // Reference to the rigid bodies of the objects that will break
    public Collider[] objectColliders; // Reference to the colliders of the objects that will break
    public Collider animationCollider; // Reference to the collider used for the animation, set in Inspector
    public int breakerLayer; // The layer to identify the breaker
    public int afterBreakLayer; // The new layer after the object breaks, set in Inspector
    public Animator animator;
    public float deactivationTime = 5.0f; // Time in seconds before objects get deactivated
    public float layerChangeDelay = 1.0f; // Delay in seconds before changing to the after-break layer, adjustable in Inspector

    #endregion

    void Start()
    {
        // Disable animator and all rigidbodies' physics until triggered
        animator.enabled = false; // Disable animation on start
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true; // Disable physics interactions
            rb.useGravity = false; // Disable gravity initially
            rb.gameObject.isStatic = true; // Make the objects static to reduce batch count

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

        // Disable the animation collider once the explosion happens
        if (animationCollider != null)
        {
            animationCollider.enabled = false;
        }

        // Delay physics activation for stability in VRChat builds
        SendCustomEventDelayedSeconds("EnablePhysics", 0.5f); // Adjust delay as needed
    }

    public void EnablePhysics()
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false; // Enable physics
            rb.useGravity = true;   // Re-enable gravity
            rb.gameObject.isStatic = false; // Switch objects to dynamic, allowing them to interact with physics

        }

        // Change the colliders' layers after a delay
        SendCustomEventDelayedSeconds("ChangeLayerAfterDelay", layerChangeDelay);

        // Set objects to inactive after a delay
        SendCustomEventDelayedSeconds("DeactivateObjects", deactivationTime);
    }

    public void ChangeLayerAfterDelay()
    {
        // Change the colliders to the after-break layer
        foreach (Collider col in objectColliders)
        {
            col.gameObject.layer = afterBreakLayer; // Change to the new layer after breaking
        }

        Debug.LogWarning("Colliders' layers changed to after-break layer after delay.");
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