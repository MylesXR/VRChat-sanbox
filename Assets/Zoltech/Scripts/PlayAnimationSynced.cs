
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayAnimationSynced : UdonSharpBehaviour
{
    public Animator animator;
    public string keyObject;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the collider is the key object
        if (other.gameObject.name == keyObject)
        {
            // Trigger the animation for all players
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "AnimationPlay");
        }
    }
    public void AnimationPlay()
    {
        animator.SetTrigger("PlayAnimation");
    }
}
