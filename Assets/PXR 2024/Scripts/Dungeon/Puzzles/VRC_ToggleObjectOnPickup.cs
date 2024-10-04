using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRC_ToggleObjectOnPickup : UdonSharpBehaviour
{
    public GameObject pickupObject;  // The object that will be toggled on and off
    public Collider pickupCollider;  // First collider to be disabled/enabled
    public Collider meshCollider;    // Second collider to be disabled/enabled

    public override void OnPickup()
    {
        if (pickupCollider != null)
        {
            // Disable the collider when picked up
            pickupCollider.enabled = false;
        }
        if (meshCollider != null)
        {
            // Disable the second collider when picked up
            meshCollider.enabled = false;
        }
    }

    public override void OnDrop()
    {
        if (pickupCollider != null)
        {
            // Re-enable the collider when dropped
            pickupCollider.enabled = true;
        }
        if (meshCollider != null)
        {
            // Re-enable the second collider when dropped
            meshCollider.enabled = true;
        }
    }
}
