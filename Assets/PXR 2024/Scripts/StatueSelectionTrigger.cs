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
    public PlayerManager playerManager;

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
                SetClass("Explorer", explorer, player);
            }
            else if (thisObjectValue == 2)
            {
                SetClass("Barbarian", barbarian, player);
            }
            else if (thisObjectValue == 3)
            {
                SetClass("Alchemist", alchemist, player);
            }
        }
    }

    private void SetClass(string className, GameObject classObject, VRCPlayerApi player)
    {
        // Update the local player's class
        playerManager.SetPlayerClass(player, className);

        // Update the local player's visual representation
        explorer.SetActive(false);
        barbarian.SetActive(false);
        alchemist.SetActive(false);

        classObject.SetActive(true);
        Bobys_WorldPortalSystem.ClassType = className;

        // Notify other scripts that this player's class has changed
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UpdateClassObjects");
    }

    public void UpdateClassObjects()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        string className = playerManager.GetPlayerClass(localPlayer);

        explorer.SetActive(className == "Explorer");
        barbarian.SetActive(className == "Barbarian");
        alchemist.SetActive(className == "Alchemist");
    }
}
