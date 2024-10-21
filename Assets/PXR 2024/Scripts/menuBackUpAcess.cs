using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System.Collections.Generic;


public class menuBackUpAcess : UdonSharpBehaviour
{
    public Bobbys_WorldPortalSystem1 Bobys_WorldPortalSystem1;
    //public GameObject PortalUI;



    void Start()
    {
        Bobys_WorldPortalSystem1.enabled = false;
    }
    public override void Interact()
    {
        ToggleObject();
    }

    public void ToggleObject()
    {
        // Apply the new state to the target object
        if (Bobys_WorldPortalSystem1 != null)
        {
            Bobys_WorldPortalSystem1.enabled = true;
        }
    }

}
