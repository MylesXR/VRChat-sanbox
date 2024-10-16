using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

public class Breakable_Environment_Object_Manager : UdonSharpBehaviour
{
    public Breakable_Environment_Object[] breakableObjects; // Array of all breakable objects (bridges, etc.)
    public Animator[] otherAnimators; // Array of additional animators for other non-breakable animations
    public float animationDuration = 2.0f; // Duration of each bridge break animation

    // Function to trigger breaking for all breakable objects across the network
    public void BreakAllObjects()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "BreakAllObjectsNetworked");
    }

    // Networked version of BreakAllObjects
    public void BreakAllObjectsNetworked()
    {
        for (int i = 0; i < breakableObjects.Length; i++)
        {
            if (breakableObjects[i] != null)
            {
                breakableObjects[i].BreakObject(); // Call BreakObject on each breakable object
            }
        }
    }

    // Function to trigger breaking of a single object by index across the network
    public void BreakSingleObject(int index)
    {
        if (index >= 0 && index < breakableObjects.Length)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "BreakSingleObjectNetworked" + index);
        }
        else
        {
            Debug.LogError("Index out of bounds for breakable objects array.");
        }
    }

    // Networked version of BreakSingleObject for each object
    public void BreakSingleObjectNetworked0() { BreakObjectAtIndex(0); }
    public void BreakSingleObjectNetworked1() { BreakObjectAtIndex(1); }
    public void BreakSingleObjectNetworked2() { BreakObjectAtIndex(2); }
    public void BreakSingleObjectNetworked3() { BreakObjectAtIndex(3); }
    // Add more methods if you have more objects in the array

    // Helper method to break the object at a specific index
    private void BreakObjectAtIndex(int index)
    {
        if (index >= 0 && index < breakableObjects.Length && breakableObjects[index] != null)
        {
            breakableObjects[index].BreakObject(); // Break the object at the specified index
        }
    }

    // Function to play other animations across the network
    public void PlayOtherAnimations()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "PlayOtherAnimationsNetworked");
    }

    // Networked version of PlayOtherAnimations
    public void PlayOtherAnimationsNetworked()
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

    // Function to reset all breakable objects and other animations across the network
    public void ResetAll()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "ResetAllNetworked");
    }

    // Networked version of ResetAll
    public void ResetAllNetworked()
    {
        // Reset all breakable objects
        for (int i = 0; i < breakableObjects.Length; i++)
        {
            if (breakableObjects[i] != null)
            {
                breakableObjects[i].ResetObject(); // Custom function to reset each object
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
}