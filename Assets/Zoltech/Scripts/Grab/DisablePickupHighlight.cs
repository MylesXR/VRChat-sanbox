using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DisablePickupHighlight : UdonSharpBehaviour
{
    public GameObject targetObject;  // The object that is being picked up

    private Collider pickupCollider;

    void Start()
    {
        if (targetObject != null)
        {
            // Get the VRC_Pickup component from the target object
            pickupCollider = targetObject.GetComponent<Collider>();
        }
    }

    public override void OnPickup()
    {
        if (pickupCollider != null)
        {
            // Disable the pickup component to remove the highlight
            pickupCollider.enabled = false;
        }
    }

    public override void OnDrop()
    {
        if (pickupCollider != null)
        {
            // Re-enable the pickup component when dropped
            pickupCollider.enabled = true;
        }
    }
}
