
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PXRStaffMenuWhiteList : UdonSharpBehaviour
{
    public Bobbys_WorldPortalSystem1 Bobbys_WorldPortalSystem1;
    //public GameObject PortalUI;
    public string[] whitelistedUsers = { "MylesXR", "JustineKat", "The Threadman", "sammievu", "helllomandy", "JakeRuneckles", "singlethread", "the_majesty", "zeNita", "nic․ej․b 2e78", "nicolepxr", "ColePaskuski", "Zzoltan", "SeaRitch", };


    void Start()
    {
        Bobbys_WorldPortalSystem1.enabled = false;
    }

    public void ToggleStaffMenu()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        foreach (string user in whitelistedUsers)
        {
            if (user == player.displayName)
            {
                ToggleObject();
            }
        }
    }
    /*
    public override void Interact()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        foreach (string user in whitelistedUsers)
        {
            if (user == player.displayName)
            {
                ToggleObject();
            }
        }
    }
    */
    public void ToggleObject()
    {
        // Apply the new state to the target object
        if (Bobbys_WorldPortalSystem1 != null)
        {
            Bobbys_WorldPortalSystem1.enabled = true;
        }
    }

}
