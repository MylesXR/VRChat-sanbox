using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Fire_Trap_Button : UdonSharpBehaviour
{
    public Fire_Trap_Button_Manager buttonManager; // Reference to the Fire_Trap_Button_Manager script

    public override void Interact()
    {
        Debug.Log(gameObject.name + " was pressed."); // Log which button was pressed
        buttonManager.RegisterButtonPress(gameObject); // Notify the manager of the button press
    }
}
