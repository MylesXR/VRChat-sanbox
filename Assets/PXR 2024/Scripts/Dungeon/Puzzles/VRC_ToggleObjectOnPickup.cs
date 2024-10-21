using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class VRC_ToggleObjectOnPickup : UdonSharpBehaviour
{
    public GameObject pickupObject;  // The object that will be toggled on and off
    public Collider pickupCollider;  // First collider to be disabled/enabled
    public Collider meshCollider;    // Second collider to be disabled/enabled
    public VRC.SDK3.Components.VRCObjectSync objectSync; // Reference to VRC Object Sync

    private void Start()
    {
        // Disable VRC Object Sync at the start
        if (objectSync != null)
        {
            objectSync.enabled = false;
            Debug.LogWarning("Object Sync OFF");
        }
    }

    public override void OnPickup()
    {
        // Enable VRC Object Sync on pickup
        if (objectSync != null)
        {
            objectSync.enabled = true;
            Debug.LogWarning("Object Sync ON");
        }

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
        // Disable VRC Object Sync on drop
        if (objectSync != null)
        {
            objectSync.enabled = false;
            Debug.LogWarning("Object Sync OFF");
        }

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
