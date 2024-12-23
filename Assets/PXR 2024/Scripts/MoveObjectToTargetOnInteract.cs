﻿using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class MoveObjectToTargetOnInteract: UdonSharpBehaviour
{
    [Header("Target Settings")]
    public GameObject targetObject;    // The object to move
    public Transform targetPosition;   // The position to move the object to

    public override void Interact()
    {

        // Check if targetObject and targetPosition are set
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(ObjectMove));
    }
    public void ObjectMove()
    {
        if (targetObject != null && targetPosition != null)
        {
            // Move the target object to the target position
            targetObject.transform.position = targetPosition.position;
        }
        else
        {
            Debug.LogWarning("Target object or target position is not set!");
        }
    }
}
