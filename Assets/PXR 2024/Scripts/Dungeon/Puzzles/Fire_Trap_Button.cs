using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Fire_Trap_Button : UdonSharpBehaviour
{
    public Fire_Trap_Button_Manager buttonManager; // Reference to the manager script
    public int buttonID; // Unique ID for this button

    public override void Interact()
    {
        Debug.LogWarning("[Fire_Trap_Button] Button pressed: " + gameObject.name + " with ID: " + buttonID); // Log button press
        buttonManager.RegisterButtonPress(buttonID); // Send the button ID to the manager
    }
}
