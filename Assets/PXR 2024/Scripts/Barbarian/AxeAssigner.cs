using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AxeAssigner : UdonSharpBehaviour
{
    public GameObject headTracker; // Single head tracker object assigned locally

    void Start()
    {
        if (headTracker == null)
        {
            return;
        }

        // Deactivate the head tracker on other players' screens
        headTracker.SetActive(true); // Activate only for the local player
        InitializeHeadTracker();
    }

    private void InitializeHeadTracker()
    {
        // Activate head tracker and set up only for local use
        ConfigureChildComponents(headTracker);
    }

    private void ConfigureChildComponents(GameObject trackerObject)
    {
        // Configure child components locally as required without networking ownership
        foreach (Transform child in trackerObject.transform)
        {
            child.gameObject.SetActive(false); // Customize visibility as needed
        }

        // Additional component-specific settings (e.g., for VR or PC)
        ConfigurePlatformSpecificComponents(trackerObject);
    }

    private void ConfigurePlatformSpecificComponents(GameObject trackerObject)
    {
        // Example settings based on VR or PC
        GameObject pcAxeChild = trackerObject.transform.GetChild(0).gameObject;
        pcAxeChild.SetActive(false); // Disable or enable based on platform needs

        GameObject returnPointChild = trackerObject.transform.GetChild(1).gameObject;
        returnPointChild.SetActive(true); // Activate or deactivate as needed

        // Repeat for VR and tether components as needed
    }

}
