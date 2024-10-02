using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class VRC_InstantiateBoulderObject_Synced : UdonSharpBehaviour
{
    public Trap_Boulder trapBoulder;  // Reference to the Trap_Boulder script

    // This method is triggered when the button is interacted with
    public override void Interact()
    {
        if (trapBoulder != null)
        {
            // Send a custom network event to all clients to instantiate the boulder
            SendCustomNetworkEvent(NetworkEventTarget.All, "NetworkInstantiateBoulder");
            Debug.LogWarning("Boulder instantiation triggered via network event.");
        }
        else
        {
            Debug.LogError("Trap_Boulder script is not assigned to the button.");
        }
    }

    // This method will be called by all clients when the network event is received
    public void NetworkInstantiateBoulder()
    {
        if (trapBoulder != null)
        {
            trapBoulder.InstantiateBoulder();  // Call the InstantiateBoulder method on all clients
            Debug.LogWarning("Boulder instantiated on all clients.");
        }
    }
}