using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using VRC.Udon.Common.Interfaces;

public class Trap_Boulder : UdonSharpBehaviour
{
    [SerializeField] private VRCObjectPool objectPool;  // Reference to the VRC Object Pool
    [SerializeField] private Transform spawnLocation;   // The location where the boulder will be instantiated
    [SerializeField] private Transform teleportDestination;  // The teleport destination to assign to the spawned boulder
    [SerializeField] private float boulderLifetime = 5f;  // Time in seconds before the boulder is destroyed

    private GameObject[] spawnedBoulders;  // Track active boulders
    private int boulderCount = 0;

    private void Start()
    {
        // Initialize the array to match the object pool size
        if (objectPool != null && objectPool.Pool != null)
        {
            spawnedBoulders = new GameObject[objectPool.Pool.Length]; // Match the array size to pool size
            for (int i = 0; i < objectPool.Pool.Length; i++)
            {
                GameObject boulder = objectPool.Pool[i];
                if (boulder != null)
                {
                    boulder.SetActive(false);  // Ensure all boulders start inactive
                }
            }
        }
        else
        {
            Debug.LogError("Object pool or its Pool array is null. Please assign the object pool correctly.");
        }
    }

    // Public function to instantiate a boulder (networked)
    public void InstantiateBoulder()
    {
        if (objectPool == null || objectPool.Pool.Length == 0)
        {
            Debug.LogError("Object pool is empty or not assigned.");
            return;
        }

        GameObject boulder = objectPool.TryToSpawn();

        if (boulder != null && spawnLocation != null)
        {
            // Spawn the boulder at the correct spawn location
            boulder.transform.position = spawnLocation.position;
            boulder.transform.rotation = spawnLocation.rotation;

            // Ensure the boulder is activated properly
            boulder.SetActive(true);
            Rigidbody boulderRigidbody = boulder.GetComponent<Rigidbody>();
            if (boulderRigidbody != null)
            {
                boulderRigidbody.velocity = Vector3.zero;
                boulderRigidbody.angularVelocity = Vector3.zero;
            }

            // Set ownership to the local player for synchronization
            Networking.SetOwner(Networking.LocalPlayer, boulder);

            // Ensure boulderCount is within bounds of the array
            int index = boulderCount % spawnedBoulders.Length;  // Wrap index to avoid exceeding array bounds
            spawnedBoulders[index] = boulder;
            boulderCount++;

            // Send network event to synchronize boulder activation across players
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(NetworkActivateBoulder));

            // Start the timer for this specific boulder
            SendCustomEventDelayedSeconds($"DestroySpecificBoulder_{index}", boulderLifetime);
        }
        else
        {
            Debug.LogWarning("No available object in the pool or spawn location is not set.");
        }
    }

    public void NetworkActivateBoulder()
    {
        // Synchronize activation for all players
        if (spawnedBoulders != null && boulderCount > 0)
        {
            int index = (boulderCount - 1) % spawnedBoulders.Length;  // Get the last boulder index
            if (index >= 0 && index < spawnedBoulders.Length && spawnedBoulders[index] != null)
            {
                GameObject boulder = spawnedBoulders[index];
                boulder.SetActive(true); // Activate the boulder if it’s already spawned
                Debug.LogWarning("Network activated the boulder.");
            }
        }
    }

    // Dynamically created event for each boulder to destroy them
    public void DestroySpecificBoulder_0() { DestroySpecificBoulder(0); }
    public void DestroySpecificBoulder_1() { DestroySpecificBoulder(1); }
    public void DestroySpecificBoulder_2() { DestroySpecificBoulder(2); }
    public void DestroySpecificBoulder_3() { DestroySpecificBoulder(3); }
    public void DestroySpecificBoulder_4() { DestroySpecificBoulder(4); }
    public void DestroySpecificBoulder_5() { DestroySpecificBoulder(5); }
    public void DestroySpecificBoulder_6() { DestroySpecificBoulder(6); }
    public void DestroySpecificBoulder_7() { DestroySpecificBoulder(7); }
    public void DestroySpecificBoulder_8() { DestroySpecificBoulder(8); }
    public void DestroySpecificBoulder_9() { DestroySpecificBoulder(9); }

    // Method triggered by the delayed event to deactivate the specific boulder
    public void DestroySpecificBoulder(int boulderIndex)
    {
        Debug.LogWarning("Boulder destruction timer completed for boulder index: " + boulderIndex);

        // Double-check if the boulder is still active and destroy it
        if (boulderIndex >= 0 && boulderIndex < spawnedBoulders.Length && spawnedBoulders[boulderIndex] != null)
        {
            Debug.LogWarning("Ensuring boulder " + boulderIndex + " is deactivated after the timer.");
            SetShouldDestroy(boulderIndex); // Destroy this boulder
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(NetworkReturnSpecificBoulder)); // Sync deactivation
        }
    }

    public void SetShouldDestroy(int boulderIndex)
    {
        if (boulderIndex >= 0 && boulderIndex < spawnedBoulders.Length && spawnedBoulders[boulderIndex] != null)
        {
            Debug.LogWarning("Boulder at index " + boulderIndex + " marked for destruction.");
            DeactivateSpecificBoulder(boulderIndex);
        }
    }

    public void NetworkReturnSpecificBoulder()
    {
        Debug.LogWarning("Network return of boulder initiated.");
    }

    private void DeactivateSpecificBoulder(int boulderIndex)
    {
        if (boulderIndex >= 0 && boulderIndex < spawnedBoulders.Length && spawnedBoulders[boulderIndex] != null)
        {
            GameObject boulder = spawnedBoulders[boulderIndex];

            // Reset velocity and angular velocity
            Rigidbody boulderRigidbody = boulder.GetComponent<Rigidbody>();
            if (boulderRigidbody != null)
            {
                boulderRigidbody.velocity = Vector3.zero;  // Reset velocity
                boulderRigidbody.angularVelocity = Vector3.zero;  // Reset angular velocity
            }

            // Return boulder to the object pool
            objectPool.Return(boulder);
            boulder.SetActive(false);  // Deactivate instead of destroying

            Debug.LogWarning("Boulder at index " + boulderIndex + " deactivated and returned to the object pool.");
            spawnedBoulders[boulderIndex] = null;
        }
        else
        {
            Debug.LogWarning("Boulder index " + boulderIndex + " is out of bounds or the boulder is null.");
        }
    }
}
