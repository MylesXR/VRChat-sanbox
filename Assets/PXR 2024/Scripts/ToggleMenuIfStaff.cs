
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

    public string[] whitelistedUsers = { "MylesXR", "JustineKat", "The Threadman", "sammievu", "helllomandy", "JakeRuneckles", "singlethread", "the_majesty", "zeNita", "nic․ej․b 2e78", "nicolepxr", "ColePaskuski", "Zzoltan", "SeaRitch",};

    void Start()
    {
        isAttendee = true;
        isStaff = false; 
    }
    private void Update()
    {
        attendeeMenuObject.SetActive(isAttendee);
        staffMenuObject.SetActive(isStaff);
    }
    public void SetStaff()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        foreach (string user in whitelistedUsers)
        {
            if (user == player.displayName)
            {
                isAttendee = false;
                isStaff = true;
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
                isAttendee = true;
                SetClass("Barbarian", barbarianScriptObject, player);
                isStaff = false;
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
                isAttendee = true;
                SetClass("Explorer", explorerScriptObject, player);
                isStaff = false;
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
                isAttendee = true;
                SetClass("Alchemist", alchemistScriptObject, player);
                isStaff = false;
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
