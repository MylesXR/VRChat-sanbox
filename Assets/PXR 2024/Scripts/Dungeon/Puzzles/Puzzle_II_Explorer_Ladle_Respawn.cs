using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class Puzzle_II_Explorer_Ladle_Respawn : UdonSharpBehaviour
{
    [Tooltip("Array of object names that can snap back to their original positions")]
    public string[] targetObjectNames;

    private Vector3[] initialPositions;
    private Quaternion[] initialRotations;
    private GameObject[] targetObjects;

    void Start()
    {
        // Initialize arrays for storing initial positions and rotations
        initialPositions = new Vector3[targetObjectNames.Length];
        initialRotations = new Quaternion[targetObjectNames.Length];
        targetObjects = new GameObject[targetObjectNames.Length];

        // Loop through the array of object names and find them in the scene
        for (int i = 0; i < targetObjectNames.Length; i++)
        {
            GameObject targetObject = GameObject.Find(targetObjectNames[i]);
            if (targetObject != null)
            {
                targetObjects[i] = targetObject;
                initialPositions[i] = targetObject.transform.position;
                initialRotations[i] = targetObject.transform.rotation;
            }
            else
            {
                //Debug.LogWarning($"Object with name '{targetObjectNames[i]}' not found in the scene.");
            }
        }
    }

    // Trigger when any collider enters the trigger area
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is in the target objects list
        for (int i = 0; i < targetObjects.Length; i++)
        {
            if (other.gameObject == targetObjects[i])
            {
                // Check if the object is currently held by a player (picked up)
                VRC_Pickup pickup = targetObjects[i].GetComponent<VRC_Pickup>();
                if (pickup != null && pickup.IsHeld)
                {
                    // Force the player to drop the object before snapping it back
                    pickup.Drop();
                }

                // Send a network event to teleport ONLY the object that hit the trigger
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, $"SnapObject{i}");
                break; // Ensure we only handle the specific object that entered the trigger
            }
        }
    }

    // Snap methods for each object in the array
    public void SnapObject0() { SnapObject(0); }
    public void SnapObject1() { SnapObject(1); }
    public void SnapObject2() { SnapObject(2); }
    public void SnapObject3() { SnapObject(3); }
    public void SnapObject4() { SnapObject(4); }

    // Snap the object by index
    public void SnapObject(int index)
    {
        GameObject targetObject = targetObjects[index];
        if (targetObject != null)
        {
            // Snap the object back to its initial position and rotation
            targetObject.transform.SetPositionAndRotation(initialPositions[index], initialRotations[index]);

            // Set the object to be kinematic again
            Rigidbody rb = targetObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }
}