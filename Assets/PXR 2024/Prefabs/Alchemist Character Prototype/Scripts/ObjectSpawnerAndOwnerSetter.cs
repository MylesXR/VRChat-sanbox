using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

public class ObjectSpawnerAndOwnerSetter : UdonSharpBehaviour
{
    [Header("Object Pool")]
    [Tooltip("The VRCObjectPool from which objects will be spawned.")]
    public VRCObjectPool objectPool;

    [Header("Debug")]
    [Tooltip("UI Text to display debug messages.")]
    public TMPro.TextMeshProUGUI debugText;

    private void Start()
    {
        if (objectPool == null)
        {
            LogError("Object pool is not assigned.");
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            // Spawn an object from the pool
            GameObject spawnedObject = objectPool.TryToSpawn();
            if (spawnedObject != null)
            {
                // Set the player as the owner of the spawned object and all its children
                SetOwnershipRecursively(spawnedObject, player);

                // Log success
                Log($"Spawned object {spawnedObject.name} and assigned ownership to player {player.displayName}.");
            }
            else
            {
                LogWarning("No available objects in the pool.");
            }
        }
        else
        {
            LogWarning("Non-owner attempted to spawn an object.");
        }
    }

    // Method to set ownership recursively for all descendants
    private void SetOwnershipRecursively(GameObject parent, VRCPlayerApi player)
    {
        // Set ownership for the parent GameObject
        Networking.SetOwner(player, parent);

        // Log the ownership setting for debugging
        Log($"Set ownership of {parent.name} to {player.displayName}");

        // Recursively set ownership for all children
        foreach (Transform child in parent.transform)
        {
            SetOwnershipRecursively(child.gameObject, player);
        }
    }

    private void Log(string message)
    {
        Debug.Log(message);
        if (debugText != null)
        {
            debugText.text += "\n" + message;
        }
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning(message);
        if (debugText != null)
        {
            debugText.text += "\nWARNING: " + message;
        }
    }

    private void LogError(string message)
    {
        Debug.LogError(message);
        if (debugText != null)
        {
            debugText.text += "\nERROR: " + message;
        }
    }
}
