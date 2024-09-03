
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DisableMeshOnCollision : UdonSharpBehaviour
{
    public MeshRenderer meshRenderer;
    public string LockObject;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the collider is the key object
        if (other.gameObject.name == LockObject)
        {
            // Trigger the animation for all players
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DisableMesh");
        }
    }
    public void DisableMesh()
    {
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
    }
}
