using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)] // Manual sync for networking
public class Trap_Boulder : UdonSharpBehaviour
{
    [SerializeField] private GameObject boulderPrefab;  // The boulder prefab to instantiate
    [SerializeField] private Transform spawnLocation;   // The location where the boulder will be instantiated
    [SerializeField] private Transform teleportDestination;  // The teleport destination to assign to the spawned boulder
    public string correctObjectName;

    private GameObject[] spawnedBoulders = new GameObject[10]; // Array to track instantiated boulders
    private int boulderCount = 0;

    [UdonSynced] private int syncedBoulderCount = 0; // Synced boulder count

    private void Start()
    {
        if (boulderPrefab == null)
        {
            Debug.LogError("Boulder prefab is not assigned!");
        }

        if (spawnLocation == null)
        {
            Debug.LogError("Spawn location is not assigned!");
        }

        if (teleportDestination == null)
        {
            Debug.LogError("Teleport destination is not assigned!");
        }
    }

    // Public function to be called from a button press in VRChat to instantiate a boulder
    public void InstantiateBoulder()
    {
        if (boulderPrefab != null && spawnLocation != null && boulderCount < spawnedBoulders.Length)
        {
            // Ensure the local player owns the boulder before instantiating it
            SendCustomNetworkEvent(NetworkEventTarget.All, "NetworkInstantiateBoulder");
        }
        else
        {
            Debug.LogError("Instantiation failed: Boulder prefab or spawn location is not set, or maximum boulders reached.");
        }
    }

    // Networked method to instantiate boulder
    public void NetworkInstantiateBoulder()
    {
        if (boulderPrefab != null && spawnLocation != null && syncedBoulderCount < spawnedBoulders.Length)
        {
            // Instantiate the boulder
            GameObject newBoulder = VRCInstantiate(boulderPrefab);
            newBoulder.transform.SetPositionAndRotation(spawnLocation.position, spawnLocation.rotation);

            // Now, get the UdonBehaviour component from the spawned boulder and set the "Destination" variable
            UdonBehaviour boulderUdon = newBoulder.GetComponent<UdonBehaviour>();
            if (boulderUdon != null)
            {
                // Set the "Destination" variable in the Udon graph
                boulderUdon.SetProgramVariable("Destination", teleportDestination);
                Debug.LogWarning("Set 'Destination' on boulder to: " + teleportDestination.name);
            }
            else
            {
                Debug.LogError("No UdonBehaviour found on the spawned boulder prefab.");
            }

            spawnedBoulders[syncedBoulderCount] = newBoulder;
            syncedBoulderCount++;
            RequestSerialization(); // Sync the boulder count across the network
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Make sure the other collider is not null
        if (other == null || other.gameObject == null)
        {
            Debug.LogError("The collider or game object is null.");
            return;
        }

        // Check if the object entering the trigger has the correct name
        Debug.LogWarning("Trigger entered by: " + other.gameObject.name);

        if (other.gameObject.name == correctObjectName)
        {
            Debug.LogWarning("Correct object in Trigger: " + other.gameObject.name);
            DestroyBoulder(other.gameObject);  // Destroy the boulder
        }
        else
        {
            Debug.LogWarning("Incorrect object in Trigger: " + other.gameObject.name);
        }
    }

    // Method to destroy the boulder locally and across the network
    public void DestroyBoulder(GameObject boulder)
    {
        if (boulder != null)
        {
            Debug.LogWarning("Attempting to destroy boulder: " + boulder.name);
            Networking.SetOwner(Networking.LocalPlayer, boulder); // Ensure the local player owns the object
            if (Networking.IsOwner(boulder))
            {
                Debug.LogWarning("Local player is owner, destroying boulder.");
                Destroy(boulder); // Destroy the boulder locally
                SendCustomNetworkEvent(NetworkEventTarget.All, "NetworkDestroyBoulder"); // Trigger destruction across the network
            }
            else
            {
                Debug.LogError("Local player is not the owner of the boulder: " + boulder.name);
            }
        }
        else
        {
            Debug.LogError("No boulder to destroy.");
        }
    }

    // Networked destruction method
    public void NetworkDestroyBoulder()
    {
        // Loop through all boulders and destroy them across the network
        for (int i = 0; i < syncedBoulderCount; i++)
        {
            if (spawnedBoulders[i] != null)
            {
                Destroy(spawnedBoulders[i]); // Destroy each boulder
                spawnedBoulders[i] = null;
            }
        }
        syncedBoulderCount = 0; // Reset the synced boulder count
        RequestSerialization(); // Sync the state
        Debug.LogWarning("Boulder destroyed across the network.");
    }

    // Backup function to destroy all spawned boulders
    public void DestroyAllBoulders()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "NetworkDestroyBoulder"); // Destroy all boulders across the network
    }

    public override void OnDeserialization()
    {
        // Sync the boulder count and ensure everything is properly networked
        Debug.LogWarning("Synced boulder count: " + syncedBoulderCount);
    }
}
