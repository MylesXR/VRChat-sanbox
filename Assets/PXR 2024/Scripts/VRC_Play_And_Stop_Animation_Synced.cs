using UdonSharp;
using UnityEngine;

public class VRC_Play_And_Stop_Animation_Synced : UdonSharpBehaviour
{
    public Animator[] Animators;
    public string AnimationPlayTrigger = "PlayAnimation";
    public string AnimationStopTrigger = "StopAnimation";
    [UdonSynced] public bool isAnimationPlaying = false;


    private void Start()
    {
        if (isAnimationPlaying)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayAnimation");
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "StopAnimation");
        }

    }

    public override void Interact()
    {
        if (Animators != null && Animators.Length > 0)
        {
            if (!isAnimationPlaying)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayAnimation");
                isAnimationPlaying = true;
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "StopAnimation");
                isAnimationPlaying = false;
            }
        }
    }

    public void PlayAnimation()
    {
        if (Animators != null)
        {
            foreach (Animator animator in Animators)
            {
                if (animator != null)
                {
                    animator.SetTrigger(AnimationPlayTrigger);
                }
            }
        }
    }

    public void StopAnimation()
    {
        if (Animators != null)
        {
            foreach (Animator animator in Animators)
            {
                if (animator != null)
                {
                    animator.SetTrigger(AnimationStopTrigger);
                }
            }
        }
    }
}