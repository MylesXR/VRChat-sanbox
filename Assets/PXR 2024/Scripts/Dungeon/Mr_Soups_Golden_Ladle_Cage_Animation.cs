using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Mr_Soups_Golden_Ladle_Cage_Animation : UdonSharpBehaviour
{
    public Animator animator;
    public Transform cageTransform;  // Reference to the cage's Transform
    public Vector3 finalCagePosition; // Final position for the cage
    public Quaternion finalCageRotation; // Final rotation for the cage

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator not assigned!");
            return;
        }

        animator.enabled = false;  // Disable animator to prevent automatic play
    }

    public void StartAnimation()
    {
        animator.enabled = true;  // Enable animator
        animator.Play("Mr-Soups-Golden-Ladle-Cage"); // Start the cage animation

        // Optionally, you can directly apply the final position after animation ends
        SendCustomEventDelayedSeconds(nameof(SetFinalCageTransform), GetAnimationClipLength("Mr-Soups-Golden-Ladle-Cage-Door"));
    }

    public void SetFinalCageTransform()
    {
        // Explicitly set the final position and rotation
        cageTransform.position = finalCagePosition;
        cageTransform.rotation = finalCageRotation;

        // Ensure the animator is no longer driving the transform
        animator.enabled = false;
    }

    private float GetAnimationClipLength(string animationName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }
        Debug.LogError("Animation clip not found: " + animationName);
        return 0f;
    }
}
