
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Puzzle_II_Bridge_Animations : UdonSharpBehaviour
{
    public Animator[] animators;

    public void PlayAnimation()
    {
        if (animators != null && animators.Length > 0)
        {
            foreach (Animator animator in animators)
            {
                if (animator != null)
                {
                    animator.SetTrigger("PlayAnimation"); // Ensure each Animator has a trigger named "PlayAnimation"
                    Debug.LogWarning("PlayAnimation trigger activated on animator: " + animator.gameObject.name);
                }
                else
                {
                    Debug.LogWarning("Animator in array is not assigned.");
                }
            }
        }
        else
        {
            Debug.LogError("Animator array is not assigned or empty.");
        }
    }
}
