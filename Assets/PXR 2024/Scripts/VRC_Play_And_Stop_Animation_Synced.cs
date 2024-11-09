using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class VRC_Play_And_Stop_Animation_Synced : UdonSharpBehaviour
{
    public Animator[] Animators;
    public string AnimationPlayTrigger = "PlayAnimation";
    public string AnimationStopTrigger = "StopAnimation";
    [UdonSynced] public bool isAnimationPlaying = false;

    private VRCPlayerApi localPlayer;
    private bool pendingInteraction = false;

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        ApplyAnimationState();
    }

    public override void Interact()
    {
        if (!Networking.IsOwner(gameObject))
        {
            // Request ownership and mark interaction as pending
            pendingInteraction = true;
            Networking.SetOwner(localPlayer, gameObject);
        }
        else
        {
            // If already the owner, proceed with toggling the state
            ToggleAnimationState();
        }
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        if (player.isLocal && pendingInteraction)
        {
            // Ownership has been transferred to the local player
            pendingInteraction = false;
            ToggleAnimationState();
        }
    }

    private void ToggleAnimationState()
    {
        // Toggle the animation state
        isAnimationPlaying = !isAnimationPlaying;

        // Apply the new animation state
        ApplyAnimationState();

        // Request serialization to synchronize the state across clients
        RequestSerialization();
    }

    public void ApplyAnimationState()
    {
        if (isAnimationPlaying)
        {
            PlayAnimation();
        }
        else
        {
            StopAnimation();
        }
    }

    private void PlayAnimation()
    {
        foreach (Animator animator in Animators)
        {
            animator.SetTrigger(AnimationPlayTrigger);
        }
    }

    private void StopAnimation()
    {
        foreach (Animator animator in Animators)
        {
            animator.SetTrigger(AnimationStopTrigger);
        }
    }

    public override void OnDeserialization()
    {
        ApplyAnimationState();
    }
}
