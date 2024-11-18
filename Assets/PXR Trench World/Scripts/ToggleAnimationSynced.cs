using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleAnimationSynced : UdonSharpBehaviour
{
    public Animator targetAnimator; // Animator component on the GameObject

    [UdonSynced(UdonSyncMode.None)]
    private bool animationTriggered;

    private void Start()
    {
        // Initialize the synchronized variable
        animationTriggered = false;
        targetAnimator.SetBool("isPlaying", false);
    }

    public override void Interact()
    {
        // Only the master client triggers the animation for all clients
        if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "TriggerAnimation");
        }
    }

    public void TriggerAnimation()
    {
        if (targetAnimator != null)
        {
            AnimatorStateInfo stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(0);
            // This will restart the animation from the beginning, regardless of its current state
            targetAnimator.Play("PXR-2024-Logo-State", 0, 0f);
            targetAnimator.SetBool("isPlaying", true); // Ensure the 'isPlaying' boolean is set to true
        }
    }





    public override void OnDeserialization()
    {
        // Called when the UdonSynced variable is updated
        if (animationTriggered)
        {
            // Set the 'isPlaying' boolean parameter if the synced variable is true
            if (targetAnimator != null)
            {
                targetAnimator.SetBool("isPlaying", true);
            }
        }
    }
}
