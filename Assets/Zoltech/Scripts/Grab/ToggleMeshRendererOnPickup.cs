using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleMeshRendererOnPickup : UdonSharpBehaviour
{
    public GameObject targetObject;  // The object whose MeshRenderer will be toggled

    private MeshRenderer meshRenderer;

    void Start()
    {
        // Get the MeshRenderer component from the target object
        if (targetObject != null)
        {
            meshRenderer = targetObject.GetComponent<MeshRenderer>();
        }
    }

    public override void OnPickup()
    {
        if (meshRenderer != null)
        {
            // Turn off the MeshRenderer when the object is picked up
            meshRenderer.enabled = false;
        }
    }

    public override void OnDrop()
    {
        if (meshRenderer != null)
        {
            // Turn the MeshRenderer back on when the object is dropped
            meshRenderer.enabled = true;
        }
    }
}
