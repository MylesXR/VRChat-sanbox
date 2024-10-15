
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Breakable_Environment_Object_Manager : UdonSharpBehaviour
{
    public Breakable_Environment_Object[] breakableObjects; // Array of all breakable objects (bridges, etc.)
    public Animator[] otherAnimators; // Array of additional animators for other non-breakable animations
    public float animationDuration = 2.0f; // Duration of each bridge break animation

    // Function to trigger breaking for all breakable objects
    public void BreakAllObjects()
    {
        // Trigger the break animation for each breakable object
        for (int i = 0; i < breakableObjects.Length; i++)
        {
            if (breakableObjects[i] != null)
            {
                breakableObjects[i].BreakObject(); // Call BreakObject on each breakable object
            }
        }
    }

    // Function to play other animations
    public void PlayOtherAnimations()
    {
        for (int i = 0; i < otherAnimators.Length; i++)
        {
            if (otherAnimators[i] != null)
            {
                otherAnimators[i].enabled = true;
                otherAnimators[i].SetTrigger("PlayAnimation"); // Trigger other animations
            }
        }
    }

    // Reset all breakable objects and other animations back to their original state
    public void ResetAll()
    {
        // Reset all breakable objects
        for (int i = 0; i < breakableObjects.Length; i++)
        {
            if (breakableObjects[i] != null)
            {
                //breakableObjects[i].ResetObject(); // Custom function to reset each object
            }
        }

        // Reset all other animators to idle
        for (int i = 0; i < otherAnimators.Length; i++)
        {
            if (otherAnimators[i] != null)
            {
                otherAnimators[i].SetTrigger("Idle"); // Reset other animations to Idle state
            }
        }
    }


    /*
    public void PlayStalactiteAnimation()
    {
        //animator.SetTrigger("PlayStalactiteAnimation");
        SendCustomEventDelayedSeconds("Idle", 5f);
    }

    public void PlayBridgeBreakingAnimation()
    {
        //animator.SetTrigger("PlayAnimation");
        SendCustomEventDelayedSeconds("Idle", 5f);
    }

    public void PlayIdleAnimation()
    {
        //animator.SetTrigger("Idle");
    }
    */
}
