using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BarbarianAxeCollision : UdonSharpBehaviour
{
    public Animator targetAnimation;

    private void OnTriggerEnter(Collider other)
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
