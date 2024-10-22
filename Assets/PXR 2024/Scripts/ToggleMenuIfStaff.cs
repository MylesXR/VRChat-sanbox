
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleMenuIfStaff : UdonSharpBehaviour
{
    public GameObject attendeeMenuObject;
    public GameObject staffMenuObject;
    private bool isAttendee;
    private bool isStaff;
    public PlayerManager playerManager;
    public GameObject explorerScriptObject;
    public GameObject barbarianScriptObject;
    public GameObject alchemistScriptObject;
    public Bobys_WorldPortalSystem Bobys_WorldPortalSystem;
    public Bobbys_WorldPortalSystem1 bobys_WorldPortalSystem1;

    public string[] whitelistedUsers = { "MylesXR", "JustineKat", "The Threadman", "sammievu", "helllomandy", "JakeRuneckles", "singlethread", "the_majesty", "zeNita", "nic․ej․b 2e78", "nicolepxr", "ColePaskuski", "Zzoltan", "SeaRitch",};

    void Start()
    {
        attendeeMenuObject.SetActive(true);
        bobys_WorldPortalSystem1.enabled = false;

    }
    //private void Update() removed when optomizing
    //{
    //    attendeeMenuObject.SetActive(isAttendee);
    //    staffMenuObject.SetActive(isStaff);
    //}
    public override void Interact()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        foreach (string user in whitelistedUsers)
        {
            if (user == player.displayName)
            {
                attendeeMenuObject.SetActive(false);
                staffMenuObject.SetActive(true);
                bobys_WorldPortalSystem1.enabled = true;
            }
        }
    }
    public void SetStaff()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        foreach (string user in whitelistedUsers)
        {
            if (user == player.displayName)
            {
                attendeeMenuObject.SetActive(false);
                staffMenuObject.SetActive(true);
                bobys_WorldPortalSystem1.enabled = true;
            }
        }
    }
    public void SetStaffBarbarian()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        foreach (string user in whitelistedUsers)
        {
            if (user == player.displayName)
            {
                attendeeMenuObject.SetActive(true);
                staffMenuObject.SetActive(false);
                SetClass("Barbarian", barbarianScriptObject, player);
            }
        }

    }
    public void SetStaffExplorer()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        foreach (string user in whitelistedUsers)
        {
            if (user == player.displayName)
            {
                attendeeMenuObject.SetActive(true);
                staffMenuObject.SetActive(false);
                SetClass("Explorer", explorerScriptObject, player);
            }
        }

    }
    public void SetStaffAlchemist()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        foreach (string user in whitelistedUsers)
        {
            if (user == player.displayName)
            {
                attendeeMenuObject.SetActive(true);
                staffMenuObject.SetActive(false);
                SetClass("Alchemist", alchemistScriptObject, player);
            }
        }

    }
    private void SetClass(string className, GameObject classObject, VRCPlayerApi player)
    {
        // Update the local player's class
        playerManager.SetPlayerClass(player, className);

        // Update the local player's visual representation
        explorerScriptObject.SetActive(false);
        barbarianScriptObject.SetActive(false);
        alchemistScriptObject.SetActive(false);

        classObject.SetActive(true);
        Bobys_WorldPortalSystem.ClassType = className;

        // Notify other scripts that this player's class has changed
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UpdateClassObjects");
    }
    public void UpdateClassObjects()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        string className = playerManager.GetPlayerClass(localPlayer);

        explorerScriptObject.SetActive(className == "Explorer");
        barbarianScriptObject.SetActive(className == "Barbarian");
        alchemistScriptObject.SetActive(className == "Alchemist");
    }
}
