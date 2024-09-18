
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayAnimationSynced1 : UdonSharpBehaviour
{
    public Animator animator;

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "AnimationPlay");
    }

    public void AnimationPlay()
    {
        animator.SetTrigger("PlayAnimation1");
    }
}
