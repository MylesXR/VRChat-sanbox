
using UdonSharp;
using UnityEngine;

public class Script_Spinning_Animation : UdonSharpBehaviour
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