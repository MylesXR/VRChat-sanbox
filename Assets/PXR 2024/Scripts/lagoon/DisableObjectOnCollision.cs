using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DisableObjectOnCollision : UdonSharpBehaviour
{
    public GameObject targetObject; // Reference to the GameObject that should be disabled
    public string LockObject; // Name of the object that triggers the disable action

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the collider is the key object
        if (other.gameObject.name == LockObject)
        {
            // Trigger the DisableObject method for all players
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DisableObject");
        }
    }

    public void DisableObject()
    {
        // Disable the GameObject
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }
}
