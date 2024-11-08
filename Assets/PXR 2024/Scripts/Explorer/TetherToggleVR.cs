using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TetherToggleVR : UdonSharpBehaviour
{
    // Reference to the tether prefab to instantiate for each player
    public GameObject tetherPrefabVR;
    public PlayerManager playerManager;

    private bool isPlayerInVR = false;
    private bool tetherIsVisibleVR = false;
    private GameObject localTetherInstance;

    void Start()
    {
        // Ensure the tetherPrefabVR is set
        if (tetherPrefabVR == null)
        {
            Debug.LogError("Tether prefab reference is not set!");
            enabled = false; // Disable the script if tetherPrefabVR is not assigned
            return;
        }

        // Check VR status once at start
        isPlayerInVR = Networking.LocalPlayer != null && Networking.LocalPlayer.IsUserInVR();

        // Instantiate a local-only tether instance if in VR and set it initially inactive
        if (isPlayerInVR)
        {
            InstantiateLocalTether();
        }

        // Start the custom update cycle
        CustomUpdateSeconds();
    }

    private void InstantiateLocalTether()
    {
        // Instantiate the tether object locally
        localTetherInstance = Instantiate(tetherPrefabVR); // Use regular Instantiate for local-only
        localTetherInstance.transform.SetParent(this.transform); // Attach to this object's hierarchy
        localTetherInstance.SetActive(false); // Initially inactive

        // Add local player check to each UdonBehaviour within the instance
        UdonBehaviour[] udonBehaviours = localTetherInstance.GetComponentsInChildren<UdonBehaviour>();
        foreach (var udon in udonBehaviours)
        {
            // Add a local-only check to each component's Update() method
            if (udon != null)
            {
                udon.gameObject.SetActive(Networking.LocalPlayer != null && Networking.LocalPlayer.isLocal);
            }
        }
    }

    public void CustomUpdateSeconds()
    {
        // Check player's class every 1.5 seconds and update visibility if both conditions are met
        if (isPlayerInVR && playerManager.GetPlayerClass() == "Explorer")
        {
            tetherIsVisibleVR = true;
        }
        else
        {
            tetherIsVisibleVR = false;
        }

        UpdateVisibility();

        // Schedule the next call to CustomUpdateSeconds in 1.5 seconds
        SendCustomEventDelayedSeconds(nameof(CustomUpdateSeconds), 1.5f);
    }

    public void UpdateVisibility()
    {
        SetVisibility(tetherIsVisibleVR);
    }

    private void SetVisibility(bool visible)
    {
        // Set the visibility of the local tether instance
        if (localTetherInstance != null)
        {
            localTetherInstance.SetActive(visible);
        }
    }
}
