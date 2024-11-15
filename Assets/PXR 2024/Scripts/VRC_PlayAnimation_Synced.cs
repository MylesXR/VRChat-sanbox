using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRC_PlayAnimation_Synced : UdonSharpBehaviour
{
    public Animator Animator;
    public string AnimationTrigger = "PlayAnimation";
    [UdonSynced] private bool isAnimationPlaying;
    [UdonSynced] private int animationPlayCount;

    private int lastAnimationPlayCount;

    void Start()
    {
        if (animationPlayCount > 0)
        {
            int timesToPlay = animationPlayCount - lastAnimationPlayCount;
            for (int i = 0; i < timesToPlay; i++)
            {
                PlayAnimation();
            }
            lastAnimationPlayCount = animationPlayCount;
        }
    }

    public override void Interact()
    {
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        isAnimationPlaying = true;
        animationPlayCount++;
        RequestSerialization();

        PlayAnimation();
        ScheduleReset();
    }

    public override void OnDeserialization()
    {
        if (animationPlayCount != lastAnimationPlayCount)
        {
            int timesToPlay = animationPlayCount - lastAnimationPlayCount;
            for (int i = 0; i < timesToPlay; i++)
            {
                PlayAnimation();
                ScheduleReset();
            }
            lastAnimationPlayCount = animationPlayCount;
        }
    }

    private void PlayAnimation()
    {
        if (Animator != null)
        {
            Animator.SetTrigger(AnimationTrigger);
        }
    }

    private void ScheduleReset()
    {
        SendCustomEventDelayedSeconds(nameof(ResetAnimationState), Animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public void ResetAnimationState()
    {
        if (!isAnimationPlaying)
            return;

        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        isAnimationPlaying = false;
        RequestSerialization();
    }
}
