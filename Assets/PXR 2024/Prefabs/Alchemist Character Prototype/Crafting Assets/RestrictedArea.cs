
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RestrictedArea : UdonSharpBehaviour
{
    Bobys_WorldPortalSystem BWPS;
    [SerializeField] Collider RestrictedCollider;


    private void OnCollisionEnter(Collision collision)
    {
        if (BWPS.ClassType == "Alchemist")
        {
            RestrictedCollider.gameObject.SetActive(false);
        }
        else if (BWPS.ClassType != "Alchemist")
        {
            RestrictedCollider.gameObject.SetActive(true);
        }
    }

}
