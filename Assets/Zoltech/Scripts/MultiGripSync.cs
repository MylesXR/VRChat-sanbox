using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class MultiGripSync : UdonSharpBehaviour
{
    public GameObject mainObject;  // Reference to the main object (e.g., LargeCube)
    private Rigidbody mainRigidbody;
    private bool isBeingGrabbed = false;

    public Vector3 initialPositionOffset;  // Offset between the main object and the grip point for position
    public Quaternion initialRotationOffset;  // Offset between the main object and the grip point for rotation

    void Start()
    {
        if (mainObject != null)
        {
            mainRigidbody = mainObject.GetComponent<Rigidbody>();
        }
    }

    public override void OnPickup()
    {
        isBeingGrabbed = true;

        if (mainRigidbody != null)
        {
            mainRigidbody.isKinematic = true;

            // If you need to calculate the offsets dynamically, you can do so here:
            // initialPositionOffset = mainObject.transform.position - transform.position;
            // initialRotationOffset = Quaternion.Inverse(transform.rotation) * mainObject.transform.rotation;
        }
    }

    public override void OnDrop()
    {
        isBeingGrabbed = false;

        if (mainRigidbody != null)
        {
            mainRigidbody.isKinematic = false;
        }
    }

    void Update()
    {
        if (isBeingGrabbed && Networking.IsOwner(gameObject))
        {
            // Update the main object's position relative to the grip point, applying the initial position offset
            mainObject.transform.position = transform.position + initialPositionOffset;

            // Update the main object's rotation relative to the grip point, applying the initial rotation offset
            mainObject.transform.rotation = transform.rotation * initialRotationOffset;
        }
    }
}
