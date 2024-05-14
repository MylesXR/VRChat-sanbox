
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
            targetAnimation.SetTrigger("PlayAnimation");
        }
    }
    
}
