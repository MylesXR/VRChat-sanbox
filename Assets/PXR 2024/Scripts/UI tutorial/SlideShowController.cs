using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SlideShowController : UdonSharpBehaviour
{
    public GameObject[] imageObjects;  // Array of images (GameObjects)
    public GameObject leftButton;      // Left navigation button
    public GameObject rightButton;     // Right navigation button

    private int currentIndex = 0;      // Track the current image index

    void Start()
    {
        // Initialize by showing only the first image
        UpdateImageVisibility();
    }

    // Go to the next image
    public void NextImage()
    {
        currentIndex = (currentIndex + 1) % imageObjects.Length;  // Wrap around to start if at the end
        UpdateImageVisibility();
    }

    // Go to the previous image
    public void PreviousImage()
    {
        currentIndex = (currentIndex - 1 + imageObjects.Length) % imageObjects.Length;  // Wrap around to end if at the beginning
        UpdateImageVisibility();
    }

    // Update which image is visible based on the current index
    private void UpdateImageVisibility()
    {
        for (int i = 0; i < imageObjects.Length; i++)
        {
            imageObjects[i].SetActive(i == currentIndex);  // Only show the current image
        }
    }

    // Handle interaction with the left button
    public void InteractLeftButton()
    {
        PreviousImage();
    }

    // Handle interaction with the right button
    public void InteractRightButton()
    {
        NextImage();
    }
}
