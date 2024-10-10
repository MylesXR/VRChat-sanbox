using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRC_ObjectRespawn_OnPickup : UdonSharpBehaviour
{
    [Tooltip("The prefab to respawn when the object is picked up")]
    public GameObject objectPrefab;

    [Tooltip("The respawn position for the new object")]
    public Transform respawnPoint;

    [Tooltip("Time in seconds before the object respawns after being picked up")]
    public float respawnDelay = 5f;

    private VRC_Pickup pickup;

    void Start()
    {
        pickup = (VRC_Pickup)GetComponent<VRC_Pickup>();

        if (pickup == null)
        {
            Debug.LogWarning("No VRC_Pickup component found on this object.");
        }
    }

    public override void OnPickup()
    {
        // Call respawn after a delay
        SendCustomEventDelayedSeconds(nameof(RespawnObject), respawnDelay);

        // Optionally destroy the current object immediately after pickup
        Destroy(this.gameObject);
    }

    public void RespawnObject()
    {
        // Instantiate a new object at the respawn point
        if (objectPrefab != null && respawnPoint != null)
        {
            Instantiate(objectPrefab, respawnPoint.position, respawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("Respawn point or prefab not set!");
        }
    }
}
