using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class StatueSelectionTrigger : UdonSharpBehaviour
{
    public GameObject explorer; // explorer
    public GameObject barbarian; // barbarian
    public GameObject alchemist; // alchemist
    public Bobys_WorldPortalSystem Bobys_WorldPortalSystem;

    public int thisObjectValue; // value of this trigger, changes what class the trigger affects

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
            Networking.SetOwner(player, gameObject);
            ToggleObject(player);
        }
    }

    public void ToggleObject(VRCPlayerApi player)
    {
        if (thisObjectValue == 1) // explorer on
        {
            SetClass(player, "Explorer", explorer);
        }
        if (thisObjectValue == 2) // barbarian on
        {
            SetClass(player, "Barbarian", barbarian);
        }
        if (thisObjectValue == 3) // alchemist on
        {
            SetClass(player, "Alchemist", alchemist);
        }
    }

    private void SetClass(VRCPlayerApi player, string className, GameObject classObject)
    {
        explorer.SetActive(false);
        barbarian.SetActive(false);
        alchemist.SetActive(false);

        classObject.SetActive(true);
        Bobys_WorldPortalSystem.ClassType = className;

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UpdateClassObjects");
    }

    public void UpdateClassObjects()
    {
        explorer.SetActive(explorer.activeSelf);
        barbarian.SetActive(barbarian.activeSelf);
        alchemist.SetActive(alchemist.activeSelf);
    }
}
