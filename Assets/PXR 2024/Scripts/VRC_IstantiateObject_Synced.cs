
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRC_IstantiateObject_Synced : UdonSharpBehaviour
{
    public Trap_Boulder trapBoulder;  // Reference to the Trap_Boulder script

    // This method is triggered when the button is interacted with
    public override void Interact()
    {
        if (trapBoulder != null)
        {
            trapBoulder.InstantiateBoulder();  // Call the InstantiateBoulder method
            Debug.LogWarning("Boulder instantiation triggered via button.");
        }
        else
        {
            Debug.LogError("Trap_Boulder script is not assigned to the button.");
        }
    }
}
