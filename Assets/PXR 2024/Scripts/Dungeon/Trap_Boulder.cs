using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Trap_Boulder : UdonSharpBehaviour
{
    [SerializeField] private GameObject boulderPrefab;  // The boulder prefab to instantiate
    [SerializeField] private Transform spawnLocation;   // The location where the boulder will be instantiated
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
    }

    // Public function to be called from a button press in VRChat
    public void InstantiateBoulder()
    {
        if (boulderPrefab != null && spawnLocation != null && boulderCount < spawnedBoulders.Length)
        {
            GameObject newBoulder = VRCInstantiate(boulderPrefab);
            newBoulder.transform.SetPositionAndRotation(spawnLocation.position, spawnLocation.rotation);
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
        if (other.gameObject.name == correctObjectName)
        {
            Debug.Log("Correct object in Trigger: " + other.gameObject.name);
            DestroyBoulder(other.gameObject);  // Destroy the boulder
        }
    }

    // Method to destroy the boulder locally and across the network
    public void DestroyBoulder(GameObject boulder)
    {
        if (boulder != null)
        {
            Networking.SetOwner(Networking.LocalPlayer, boulder); // Ensure the local player owns the object
            Destroy(boulder); // Destroy the boulder locally
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "NetworkDestroyBoulder"); // Trigger destruction across the network
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
        Debug.Log("All boulders destroyed.");
    }
}
