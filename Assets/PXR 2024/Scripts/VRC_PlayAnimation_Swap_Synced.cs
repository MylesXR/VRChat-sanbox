using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRC_PlayAnimation_Swap_Synced : UdonSharpBehaviour
{
    public Animator targetAnimation;

    // Triggers for the animations, set these in the Inspector
    public string firstAnimationTrigger = "PlayFirstAnimation";
    public string secondAnimationTrigger = "PlaySecondAnimation";

    // Boolean to track which animation to play next
    private bool playFirstAnimation = true;

    // This method decides which animation to play on interaction
    public override void Interact()
    {
        if (targetAnimation != null)
        {
            if (playFirstAnimation)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayFirstAnimation");
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlaySecondAnimation");
            }
        }
    }

    // Plays the first animation and switches to the second one for the next interaction
    public void PlayFirstAnimation()
    {
        if (targetAnimation != null)
        {
            targetAnimation.SetTrigger(firstAnimationTrigger); // Trigger for opening animation (set in Inspector)
            Debug.LogWarning($"Playing first animation: {firstAnimationTrigger}");
            playFirstAnimation = false;  // Set flag for second animation next time
        }
    }

    // Plays the second animation and switches back to the first one for the next interaction
    public void PlaySecondAnimation()
    {
        if (targetAnimation != null)
        {
            targetAnimation.SetTrigger(secondAnimationTrigger); // Trigger for closing animation (set in Inspector)
            Debug.LogWarning($"Playing second animation: {secondAnimationTrigger}");
            playFirstAnimation = true;  // Set flag for first animation next time
        }
    }
}
