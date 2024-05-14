
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BarbarianThrowAxe : UdonSharpBehaviour
{
    public float maxThrowForce = 20f; // Maximum throwing force
    public float minThrowForce = 5f; // Minimum throwing force
    public float forceHoldTime = 1f; // Time in seconds to reach max throw force
    public KeyCode throwKey = KeyCode.Space; // Change this to the desired input key
    public float resetTime = 10f; // Time in seconds before axe resets

    private Rigidbody axeRigidbody;
    public Transform playerHead;
    private Transform axeParent; // Reference to the axe's parent object
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private bool isThrown = false;
    private float throwTime;
    private float pressTime;
    private void OnEnable()
    {
        // Store the axe's parent object
        axeParent = transform.parent;

        // Store initial local position and rotation
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
    }
    private void Start()
    {
        axeRigidbody = GetComponent<Rigidbody>();

        
    }

    private void Update()
    {
        // Check if the throw key is pressed and the axe is not currently thrown
        if (Input.GetKeyDown(throwKey) && !isThrown)
        {
            pressTime = Time.time;
        }

        // Check if the throw key is released and the axe is not currently thrown
        if (Input.GetKeyUp(throwKey) && !isThrown)
        {
            float holdDuration = Time.time - pressTime;
            float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, Mathf.Clamp01(holdDuration / forceHoldTime));
            ThrowAxe(throwForce);
        }

        // Check if axe has been thrown and if it's time to reset
        if (isThrown && Time.time - throwTime >= resetTime)
        {
            ResetAxe();
        }
    }

    private void ThrowAxe(float force)
    {
        // Detach the axe from its parent
        transform.parent = null;

        // Calculate the direction to throw the axe
        Vector3 throwDirection = (playerHead.forward + Vector3.up).normalized;

        // Disable kinematic property
        axeRigidbody.isKinematic = false;

        // Apply the throw force to the axe
        axeRigidbody.AddForce(throwDirection * force, ForceMode.Impulse);

        // Set thrown flag
        isThrown = true;

        // Note the time the axe was thrown
        throwTime = Time.time;
    }

    private void ResetAxe()
    {
        // Reattach the axe to its parent
        transform.parent = axeParent;

        // Reset local position and rotation relative to parent
        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;

        // Enable kinematic property
        axeRigidbody.isKinematic = true;

        // Reset thrown flag
        isThrown = false;
    }
}

