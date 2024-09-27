
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Trap_Fire_Wall : UdonSharpBehaviour
{
    [SerializeField] private GameObject[] objectsToToggle;  // The game objects to toggle on/off
    [SerializeField] private float toggleDelay = 1.0f;  // Public variable to set the delay in seconds
    private bool areObjectsActive = true;

    private void Start()
    {
        // Start the toggling process
        ToggleObjects();
    }

    public void ToggleObjects()
    {
        // Toggle the active state of the objects
        areObjectsActive = !areObjectsActive;

        foreach (GameObject obj in objectsToToggle)
        {
            if (obj != null)
            {
                obj.SetActive(areObjectsActive);
            }
        }

        // Schedule the next toggle using the specified delay
        SendCustomEventDelayedSeconds(nameof(ToggleObjects), toggleDelay);
    }
}
