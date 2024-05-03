using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System.Collections.Generic;


public class WhiteListPortals : UdonSharpBehaviour
{
    public Bobys_WorldPortalSystem Bobys_WorldPortalSystem;
    //public GameObject PortalUI;
    public string[] whitelistedUsers = { "MylesXR", "JustineKat", "The Threadman", "sammievu", "helllomandy", "JakeRuneckles", "singlethread", "the_majesty", "zeNita", "nic․ej․b 2e78", "nicolepxr", "ColePaskuski", "Zzoltan", "SeaRitch",};
    

    void Start()
    {
        Bobys_WorldPortalSystem.enabled = false;
    }
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

    public void ToggleObject()
    {
        
        

        // Apply the new state to the target object
        if (Bobys_WorldPortalSystem != null)
        {
            Bobys_WorldPortalSystem.enabled = true;
        }
    }

}
