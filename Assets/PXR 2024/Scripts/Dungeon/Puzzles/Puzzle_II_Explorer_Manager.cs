using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Puzzle_II_Explorer_Manager : UdonSharpBehaviour
{
    public Puzzle_II_Explorer_TriggerZone[] pressurePlates; // Array of all pressure plates
    public Animator[] animators; // Array of animators to trigger animations

    // This method is called whenever a plate status changes
    public void UpdatePlateStatus()
    {
        bool puzzleComplete = true;

        // Check if all pressure plates have the correct objects
        foreach (Puzzle_II_Explorer_TriggerZone plate in pressurePlates)
        {
            if (!plate.IsCorrectObjectOnPlate())
            {
                puzzleComplete = false;
                break;
            }
        }

        if (puzzleComplete)
        {
            Debug.Log("Puzzle is complete! Triggering the animations.");
            TriggerPuzzleComplete(); // Trigger the puzzle completion logic
        }
    }

    // Method to handle puzzle completion
    public void TriggerPuzzleComplete()
    {
        // Broadcast the event to all clients to play the animations
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayCompletionAnimations");
    }

    // This method will be called across all clients to play all animations
    public void PlayCompletionAnimations()
    {
        if (animators != null && animators.Length > 0)
        {
            foreach (Animator anim in animators)
            {
                if (anim != null)
                {
                    anim.SetTrigger("PlayAnimation"); // Make sure each Animator has a trigger called "PlayAnimation"
                    Debug.Log("Animation triggered for: " + anim.gameObject.name);
                }
                else
                {
                    Debug.LogError("One of the animators in the array is not assigned.");
                }
            }
        }
        else
        {
            Debug.LogError("Animators array is empty or not assigned.");
        }
    }
}
