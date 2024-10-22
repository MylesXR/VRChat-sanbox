using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class VRC_ToggleObject_Synced : UdonSharpBehaviour
{
    public GameObject[] targetObjects; // Array of GameObjects to be toggled

    [UdonSynced]
    private bool isObjectActive; // Synchronized variable to keep track of objects' active state

    private void Start()
    {
        // Initialize the variable and set the objects' initial state
        if (targetObjects != null && targetObjects.Length > 0)
        {
            isObjectActive = targetObjects[0].activeSelf; // Assume all objects share the same initial state
            SetObjectsActive(isObjectActive);
        }
    }

    public override void Interact()
    {
        // Send the event to all players to toggle the objects
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ToggleObjects");
    }

    public override void OnDeserialization()
    {
        // Called when the UdonSynced variable is updated
        // Update the objects' state based on the synced variable
        SetObjectsActive(isObjectActive);
    }

    public void ToggleObjects()
    {
        if (Networking.LocalPlayer.isMaster)
        {
            // Toggle the state and update the synced variable
            isObjectActive = !isObjectActive;

            // Apply the new state to all target objects
            SetObjectsActive(isObjectActive);
        }
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
