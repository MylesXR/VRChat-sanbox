using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PreviousImageButton : UdonSharpBehaviour
{
    public SlideShowController slideshowController;  // Reference to the slideshow controller

    // Detect player interaction with the button
    public override void Interact()
    {
        slideshowController.PreviousImage();  // Call the PreviousImage method
    }
}
