using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class AxeToggle : UdonSharpBehaviour
{
    public KeyCode toggleKey = KeyCode.T; // Key to press for toggling

    // Reference to the child GameObject to toggle
    public GameObject axeObject;

    // Synced variable to track visibility state across network
    [UdonSynced]
    private bool isVisible = true;

    void Start()
    {
        // Ensure the axeObject is set
        if (axeObject == null)
        {
            Debug.LogError("Axe object reference is not set!");
            enabled = false; // Disable the script if the axeObject is not assigned
        }

        // Initialize visibility state based on synced variable
        SetVisibility(isVisible);
    }

    void Update()
    {
        if (Networking.IsOwner(gameObject)) // Check if the local player owns this object
        {
            if (Input.GetKeyDown(toggleKey))
            {
                // Toggle visibility locally and sync across network
                isVisible = !isVisible;
                SetVisibility(isVisible);

                // Request serialization to sync with other players
                RequestSerialization();
            }
        }
    }

    public override void OnDeserialization()
    {
        // Sync the visibility state when receiving network updates
        SetVisibility(isVisible);
    }

    private void SetVisibility(bool visible)
    {
        // Set the visibility of the child object
        axeObject.SetActive(visible);
    }
}
