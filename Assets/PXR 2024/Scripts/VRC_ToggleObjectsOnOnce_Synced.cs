
using UdonSharp;
using UnityEngine;

public class VRC_ToggleObjectsOnOnce_Synced : UdonSharpBehaviour
{
    public GameObject[] targetObjects;

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ToggleObject");
    }

    public void ToggleObject()
    {
        if (targetObjects != null)
        {
            foreach (GameObject targetObject in targetObjects)
            {
                targetObject.GetComponent<BoxCollider>().enabled = true;
            }
        }          
    }
}