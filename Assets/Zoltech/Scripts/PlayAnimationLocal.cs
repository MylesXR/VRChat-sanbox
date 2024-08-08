
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayAnimationLocal : UdonSharpBehaviour
{
    public Animator animator;
    public override void Interact()
    {
        animator.SetTrigger("PlayAnimation");
    }
}
