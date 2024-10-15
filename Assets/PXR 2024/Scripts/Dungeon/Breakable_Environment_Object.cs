﻿using UdonSharp;
using UnityEngine;

public class Breakable_Environment_Object : UdonSharpBehaviour
{
    #region Variables 

    public Rigidbody[] rbs;
    public Collider[] objectColliders; // Colliders of the objects that will break
    public Collider animationCollider; // Collider used for the animation, set in Inspector
    public Animator animator;
    public float deactivationTime = 5.0f; // Time in seconds before objects get deactivated
    public float animationDuration = 2.0f; // Duration of the break animation

    [UdonSynced] public bool isBroken = false;

    #endregion

    #region On Start

    void Start()
    {
        // Disable animator and all rigidbodies' physics until triggered
        animator.enabled = false; // Disable animation on start
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true; // Disable physics interactions
            rb.useGravity = false; // Disable gravity initially
        }

        // Ensure all object colliders are enabled at the start
        foreach (Collider col in objectColliders)
        {
            col.enabled = true;
        }

        // Ensure the animation collider is enabled at the start
        if (animationCollider != null)
        {
            animationCollider.enabled = true;
        }
    }

    #endregion

    #region Play Animation To Break Object 

    public void BreakObject()
    {
        animator.enabled = true;
        animator.SetTrigger("PlayAnimation");
        SendCustomEventDelayedSeconds("EnablePhysicsAndBreak", animationDuration); 
    }

    #endregion

    #region  Enable Kinematic And Gravity After Delay

    public void EnablePhysicsAndBreak()
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false; 
            rb.useGravity = true;   
        }
        SendCustomEventDelayedSeconds("DeactivateObjects", deactivationTime);
    }

    #endregion

    #region Deactivate Obejsct After Breaking

    public void DeactivateObjects()
    {
        foreach (Collider col in objectColliders)
        {
            col.gameObject.SetActive(false); 
        }

        Debug.LogWarning("Exploded objects have been set inactive.");
    }

    #endregion

}