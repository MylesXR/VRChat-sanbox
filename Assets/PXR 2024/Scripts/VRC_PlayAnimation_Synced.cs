
using UdonSharp;
using UnityEngine;

public class VRC_PlayAnimation_Synced : UdonSharpBehaviour
{
    public Animator Animator;
    public string AnimationTrigger = "PlayAnimation";

    public override void Interact()
    {
        if (Animator != null)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayAnimation");
        }      
    }

    public void PlayAnimation()
    {
        Animator.SetTrigger(AnimationTrigger);
    }
}