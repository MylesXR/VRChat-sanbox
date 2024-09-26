
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AnimationEvents_Dungeon : UdonSharpBehaviour
{
    public Animator animator;

    // This method is triggered by the animation event
    public void StartRotation()
    {
        animator.Play("Mr-Soups-Golden-Lure-Cage");
    }
}
