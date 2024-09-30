
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Puzzle_II_Bridge_Animations : UdonSharpBehaviour
{
    public Animator animator;

    public void PlayAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("PlayAnimation"); // Ensure your Animator has a trigger named "PlayAnimation"
            Debug.Log("PlayAnimation trigger activated.");
        }
        else
        {
            Debug.LogError("Animator is not assigned.");
        }
    }
}
