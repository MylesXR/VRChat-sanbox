
using UdonSharp;
using UnityEngine;

public class VRC_PlayAnimationOnStart : UdonSharpBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string animationClip;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play(animationClip); 
    }
}
