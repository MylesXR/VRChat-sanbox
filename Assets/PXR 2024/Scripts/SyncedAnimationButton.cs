using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SyncedAnimationButton : UdonSharpBehaviour
{
    public Animator targetAnimation;

    public override void Interact()
    {
        if (targetAnimation != null)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayAnimation");
        }
    }

    public void PlayAnimation()
    {
        targetAnimation.SetTrigger("PlayAnimation");
    }
}
