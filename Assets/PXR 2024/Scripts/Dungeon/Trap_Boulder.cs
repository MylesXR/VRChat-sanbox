using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Trap_Boulder : UdonSharpBehaviour
{
    [SerializeField] private GameObject boulderPrefab;  // The boulder prefab to instantiate
    [SerializeField] private Transform spawnLocation;   // The location where the boulder will be instantiated
    [SerializeField] private Transform teleportDestination;  // The teleport destination to assign to the spawned boulder
    public string correctObjectName;

    private GameObject[] spawnedBoulders = new GameObject[10]; // Array to track instantiated boulders
    private int boulderCount = 0;

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

    // Public function to be called from a button press in VRChat
    public void InstantiateBoulder()
    {
        if (boulderPrefab != null && spawnLocation != null && boulderCount < spawnedBoulders.Length)
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

            spawnedBoulders[boulderCount] = newBoulder;
            boulderCount++;
        }
        else
        {
            Debug.LogError("Instantiation failed: Boulder prefab or spawn location is not set, or maximum boulders reached.");
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
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "NetworkDestroyBoulder"); // Trigger destruction across the network
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
        // This function doesn't need to do anything specific in this case
        // since the boulder destruction is handled by DestroyBoulder
        // The event just ensures synchronization across the network
    }

    // Backup function to destroy all spawned boulders
    public void DestroyAllBoulders()
    {
        for (int i = 0; i < boulderCount; i++)
        {
            if (spawnedBoulders[i] != null)
            {
                Networking.SetOwner(Networking.LocalPlayer, spawnedBoulders[i]); // Ensure the local player owns the object
                Destroy(spawnedBoulders[i]); // Destroy the boulder locally
                spawnedBoulders[i] = null;
            }
        }
        boulderCount = 0; // Reset the boulder count
        Debug.LogWarning("All boulders destroyed.");
    }
}
