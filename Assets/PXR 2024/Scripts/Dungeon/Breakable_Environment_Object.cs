using UdonSharp;
using UnityEngine;
using VRC.Udon.Common.Interfaces;

public class Breakable_Environment_Object : UdonSharpBehaviour
{
    #region Variables 

    public Rigidbody[] rbs;
    public Collider[] objectColliders; 
    public Collider animationCollider; 
    public Animator animator;
    public float deactivationTime = 5.0f; 
    public float animationDuration = 2.0f; 
    [UdonSynced] public bool isBroken = false;

    #endregion

    #region On Start

    void Start()
    {
        ResetObject();
    }

    public void ResetObject()
    {
        // Reset the animator to idle and disable physics
        isBroken = false;
        animator.enabled = false;
        animator.SetTrigger("Idle");

        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true; // Reset to kinematic (no physics interactions)
            rb.useGravity = false; // Disable gravity initially
        }

        // Re-enable colliders
        foreach (Collider col in objectColliders)
        {
            col.enabled = true;
            col.gameObject.SetActive(true);
        }

        // Ensure the animation collider is enabled
        if (animationCollider != null)
        {
            animationCollider.enabled = true;
        }
    }

    #endregion

    #region Play Animation To Break Object 

    public void BreakObject()
    {
        if (!isBroken)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "NetworkedBreakObject");
        }
    }

    public void NetworkedBreakObject()
    {
        if (!isBroken) 
        {
            isBroken = true;
            animator.enabled = true;
            animator.SetTrigger("PlayAnimation");
            SendCustomEventDelayedSeconds("EnablePhysicsAndBreak", animationDuration);
        }
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