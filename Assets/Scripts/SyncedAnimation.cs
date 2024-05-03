
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SyncedAnimation : UdonSharpBehaviour
{
    public Animator animator;
    void Start()
    {
        
    }

    public override void Interact()
    {
        AnimationNetwork();
    }

    public void AnimationNetwork()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "AnimationToggle");
    }
    public void AnimationToggle()
    {
        animator.SetTrigger("Wall Crack");
    }
}
