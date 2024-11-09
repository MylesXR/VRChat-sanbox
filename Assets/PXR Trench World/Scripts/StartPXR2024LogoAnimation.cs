
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class StartPXR2024LogoAnimation : UdonSharpBehaviour
{
    [SerializeField]
    private Animator SpinningAnimator;

    [SerializeField]
    private string SpinningAnimatorStateName;

    void Start()
    {
        SpinningAnimator.Play(SpinningAnimatorStateName);
    }
}
