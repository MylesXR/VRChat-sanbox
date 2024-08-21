
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayAnimationTest : UdonSharpBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string animationClip;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play(animationClip); // Replace with your animation clip name
    }
}
