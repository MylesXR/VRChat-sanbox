using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

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
                Debug.Log("Alchemist Entered Restricted Area");
                Block.SetActive(false);
            }
            else
            {
                Debug.Log("Not Alchemist");
                Block.SetActive(true);
            }
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            if (BWPS.ClassType == "Alchemist")
            {
                Debug.Log("Alchemist Entered Restricted Area");
                Block.SetActive(true);
            }
            else
            {
                Debug.Log("Not Alchemist");
                Block.SetActive(true);
            }
        }
    }
}
