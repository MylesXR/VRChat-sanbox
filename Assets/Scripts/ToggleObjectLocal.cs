using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleObjectLocal : UdonSharpBehaviour
{
    public GameObject targetObject; // The GameObject to be toggled

    private bool isObjectActive; // Local variable to keep track of object's active state

    private void Start()
    {
        // Initialize the variable and set the object's initial state
        isObjectActive = targetObject.activeSelf;
    }

    public override void Interact()
    {
        // Toggle the object locally
        ToggleObject();
    }

    public void ToggleObject()
    {
        // Toggle the state and update the local variable
        isObjectActive = !isObjectActive;

        // Apply the new state to the target object
        if (targetObject != null)
        {
            targetObject.SetActive(isObjectActive);
        }
    }
}
