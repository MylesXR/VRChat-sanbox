
using UdonSharp;
using UnityEngine;

public class VRC_PlayAnimationOnStart : UdonSharpBehaviour
{
    [SerializeField] Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("PlayAnimation");
    }
}