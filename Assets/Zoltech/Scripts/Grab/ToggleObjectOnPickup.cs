using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleObjectOnPickup : UdonSharpBehaviour
{
    public GameObject targetObject;  // The object that will be toggled on and off

    public override void OnPickup()
    {
        if (targetObject != null)
        {
            // Disable the entire object when picked up
            targetObject.SetActive(false);
        }
    }

    public override void OnDrop()
    {
        if (targetObject != null)
        {
            // Re-enable the entire object when dropped
            targetObject.SetActive(true);
        }
    }
}
