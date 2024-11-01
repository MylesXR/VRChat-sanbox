using UdonSharp;
using UnityEngine;

public class VRC_ToggleObject_Synced : UdonSharpBehaviour
{
    public GameObject[] targetObjects; 
    [UdonSynced] private bool isObjectActive; 

    private void Start()
    {     
        if (targetObjects != null && targetObjects.Length > 0) // Initialize the variable and set the objects' initial state
        {
            isObjectActive = targetObjects[0].activeSelf; // Assume all objects share the same initial state
            SetObjectsActive(isObjectActive);
        }
    }

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ToggleObjects");
    }

    public void ToggleObjects()
    {     
            isObjectActive = !isObjectActive; // Toggle the state and update the synced variable
            SetObjectsActive(isObjectActive); // Apply the new state to all target objects      
    }

    private void SetObjectsActive(bool state)
    {
        if (targetObjects != null)
        {
            foreach (GameObject obj in targetObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(state);
                }
            }
        }
    }
}