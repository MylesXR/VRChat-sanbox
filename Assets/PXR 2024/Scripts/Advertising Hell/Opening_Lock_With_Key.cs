using UdonSharp;
using UnityEngine;

public class Opening_Lock_With_Key : UdonSharpBehaviour
{
    public GameObject keyObject;
    public Animator lockAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == keyObject)
        {
            lockAnimator.SetTrigger("PlayAnimation");
            keyObject.SetActive(false);
        }
    }
}