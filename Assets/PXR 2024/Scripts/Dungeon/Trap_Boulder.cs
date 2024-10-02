using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;


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



    // Public function to be called from a button press in VRChat to instantiate a boulder
    // Public function to instantiate a boulder (networked)
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

            // Ensure it's synced for everyone
            Networking.SetOwner(Networking.LocalPlayer, newBoulder);

            // Now, get the UdonBehaviour component from the spawned boulder and set the "Destination" variable
            UdonBehaviour boulderUdon = newBoulder.GetComponent<UdonBehaviour>();
            if (boulderUdon != null)
            {
                boulderUdon.SetProgramVariable("Destination", teleportDestination);
                Debug.LogWarning("Set 'Destination' on boulder to: " + teleportDestination.name);
            }

            spawnedBoulders[syncedBoulderCount] = newBoulder;
            syncedBoulderCount++;
            RequestSerialization(); // Sync the boulder count across the network
        }
    }

    // Method to destroy the boulder locally and across the network
    public void DestroyBoulder(GameObject boulder)
    {
        if (boulder != null)
        {
            int boulderIndex = System.Array.IndexOf(spawnedBoulders, boulder);
            if (boulderIndex != -1)
            {
                Networking.SetOwner(Networking.LocalPlayer, boulder); // Take ownership
                SendCustomNetworkEvent(NetworkEventTarget.All, "NetworkDestroySpecificBoulder_" + boulderIndex); // Trigger destruction across the network
                Destroy(boulder); // Destroy the boulder locally
            }
            else
            {
                Debug.LogError("Boulder not found in the spawnedBoulders array.");
            }
        }
        else
        {
            Debug.LogError("No boulder to destroy.");
        }
    }

    // Networked destruction of a specific boulder by index
    public void NetworkDestroySpecificBoulder(int boulderIndex)
    {
        if (boulderIndex >= 0 && boulderIndex < spawnedBoulders.Length && spawnedBoulders[boulderIndex] != null)
        {
            Destroy(spawnedBoulders[boulderIndex]); // Destroy that specific boulder
            spawnedBoulders[boulderIndex] = null;
        }
        syncedBoulderCount--;
        RequestSerialization(); // Sync the state after destruction
    }

    // Sync the state of the boulders across the network
    public override void OnDeserialization()
    {
        Debug.LogWarning("Synced boulder count: " + syncedBoulderCount);
    }
}
