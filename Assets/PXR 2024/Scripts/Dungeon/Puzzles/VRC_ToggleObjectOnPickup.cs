using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRC_ToggleObjectOnPickup : UdonSharpBehaviour
{
    public GameObject targetObject;  // The object that will be toggled on and off
    public Collider objectCollider1; // First collider to be disabled/enabled
    public Collider objectCollider2; // Second collider to be disabled/enabled 

    public override void OnPickup()
    {
        if (targetObject != null)
        {
            // Disable the entire object and its colliders when picked up
            targetObject.SetActive(false);
            if (objectCollider1 != null)
            {
                objectCollider1.enabled = false;
            }
            if (objectCollider2 != null)
            {
                objectCollider2.enabled = false;
            }
        } 
    }

    public override void OnDrop()
    {
        if (targetObject != null)
        {
            // Re-enable the entire object and its colliders when dropped
            targetObject.SetActive(true);
            if (objectCollider1 != null)
            {
                objectCollider1.enabled = true;
            }
            if (objectCollider2 != null)
            {
                objectCollider2.enabled = true;
            }
        }
    } 
}
