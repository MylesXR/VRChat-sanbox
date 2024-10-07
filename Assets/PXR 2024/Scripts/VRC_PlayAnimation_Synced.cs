
using UdonSharp;
using UnityEngine;

public class VRC_PlayAnimation_Synced : UdonSharpBehaviour
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
