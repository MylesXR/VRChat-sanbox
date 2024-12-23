﻿using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleSecondObjectSynced : UdonSharpBehaviour
{
    public GameObject targetObject; // The GameObject to be toggled
    public GameObject targetObject2;

    [UdonSynced]
    private bool isObjectActive; // Synchronized variable to keep track of object's active state

    private void Start()
    {
        // Initialize the variable and set the object's initial state
        isObjectActive = targetObject.activeSelf;
    }

    public override void Interact()
    {
        // Send the event to all players to toggle the object
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ToggleObject");
    }

    public override void OnDeserialization()
    {
        // Called when the UdonSynced variable is updated
        // Update the object's state based on the synced variable
        if (targetObject != null)
        {
            targetObject.SetActive(isObjectActive);
        }
    }

    public void ToggleObject()
    {
        if (Networking.LocalPlayer.isMaster)
        {
            // Toggle the state and update the synced variable
            isObjectActive = !isObjectActive;

            // Apply the new state to the target object
            if (targetObject != null)
            {
                targetObject.SetActive(isObjectActive);
                targetObject2.SetActive(isObjectActive);
            }
        }
    }
}
