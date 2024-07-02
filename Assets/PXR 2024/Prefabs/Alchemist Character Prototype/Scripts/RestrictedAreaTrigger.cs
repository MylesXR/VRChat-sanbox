using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RestrictedAreaTrigger : UdonSharpBehaviour
{
    public Bobys_WorldPortalSystem BWPS;
    [SerializeField] private GameObject Block;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            if (BWPS.ClassType == "Alchemist")
            {
                Debug.Log("Alchemist Entered");
                // Destroy the collider
                Block.SetActive(false);
            }
            else
            {
                Debug.Log("Not Alchemist");
                Block.SetActive(true);
            }
        }
    }
}
