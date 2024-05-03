using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System.Collections.Generic;


public class WhiteListMegaphone : UdonSharpBehaviour
{
    public GameObject targetObject;
    //public GameObject PortalUI;
    public string[] whitelistedUsers = { "MylesXR", "JustineKat", "The Threadman", "sammievu", "helllomandy", "JakeRuneckles", "singlethread" };
    private bool isObjectActive;

    void Start()
    {
        isObjectActive = targetObject.activeSelf;
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
        // Toggle the state and update the local variable
        isObjectActive = !isObjectActive;

        // Apply the new state to the target object
        if (targetObject != null)
        {
            targetObject.SetActive(isObjectActive);
        }
    }
}
