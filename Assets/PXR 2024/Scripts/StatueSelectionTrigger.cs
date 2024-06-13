using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class StatueSelectionTrigger : UdonSharpBehaviour
{
    public GameObject explorer;
    public GameObject barbarian;
    public GameObject alchemist;
    public Bobys_WorldPortalSystem Bobys_WorldPortalSystem;

    public int thisObjectValue;

    private void Start()
    {
        explorer.SetActive(false);
        barbarian.SetActive(false);
        alchemist.SetActive(false);
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            ToggleObject(player);
        }
    }

    public void ToggleObject(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            if (thisObjectValue == 1)
            {
                SetClass("Explorer", explorer);
            }
            else if (thisObjectValue == 2)
            {
                SetClass("Barbarian", barbarian);
            }
            else if (thisObjectValue == 3)
            {
                SetClass("Alchemist", alchemist);
            }
        }
    }

    private void SetClass(string className, GameObject classObject)
    {
        explorer.SetActive(false);
        barbarian.SetActive(false);
        alchemist.SetActive(false);

        classObject.SetActive(true);
        Bobys_WorldPortalSystem.ClassType = className;

        // Update class objects for everyone
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UpdateClassObjects");
    }

    public void UpdateClassObjects()
    {
        explorer.SetActive(explorer.activeSelf);
        barbarian.SetActive(barbarian.activeSelf);
        alchemist.SetActive(alchemist.activeSelf);
    }
}
