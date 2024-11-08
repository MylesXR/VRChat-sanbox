using UdonSharp;
using UnityEngine;
using VRC.Udon.Common.Interfaces;

public class VRC_PlayAnimation_Synced : UdonSharpBehaviour
{
    public Animator Animator;
    public string AnimationTrigger = "PlayAnimation";
    [UdonSynced] private bool isAnimationPlaying;

    public override void Interact()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "PlayAnimation");
    }

    public void PlayAnimation()
    {
        if (Animator != null)
        {
            isAnimationPlaying = !isAnimationPlaying; 
            Animator.SetTrigger(AnimationTrigger);
        }
    }
}