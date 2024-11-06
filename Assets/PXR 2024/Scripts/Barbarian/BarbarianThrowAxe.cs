using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BarbarianThrowAxe : UdonSharpBehaviour
{
    public float maxThrowForce = 20f;
    public float minThrowForce = 5f;
    public float forceHoldTime = 1f;
    public KeyCode throwKey = KeyCode.Space;
    public float resetTime = 10f;

    private Rigidbody axeRigidbody;
    public Transform playerHead;
    private Transform axeParent;
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private bool isThrown = false;
    private float throwTime;
    private float pressTime;

    private bool hasBeenEnabled = false;

    private void OnEnable()
    {
        if (!hasBeenEnabled)
        {
            axeParent = transform.parent;
            initialLocalPosition = transform.localPosition;
            initialLocalRotation = transform.localRotation;
            Debug.Log("[BarbarianThrowAxe] OnEnable called");
            hasBeenEnabled = true;
        }
    }

    private void Start()
    {
        axeRigidbody = GetComponent<Rigidbody>();
        Debug.Log("[BarbarianThrowAxe] Start called");
    }

    private void Update()
    {
        // Reset position if the axe is not thrown
        if (!isThrown)
        {
            transform.localPosition = initialLocalPosition;
            transform.localRotation = initialLocalRotation;
        }

        // Handle throw action
        if (Input.GetKeyDown(throwKey) && !isThrown)
        {
            pressTime = Time.time;
            Debug.Log("[BarbarianThrowAxe] Throw key pressed");
        }

        if (Input.GetKeyUp(throwKey) && !isThrown)
        {
            float holdDuration = Time.time - pressTime;
            float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, Mathf.Clamp01(holdDuration / forceHoldTime));
            Debug.Log($"[BarbarianThrowAxe] Throw key released, hold duration: {holdDuration}, throw force: {throwForce}");
            ThrowAxe(throwForce);
        }

        // Check if it's time to reset the axe
        if (isThrown && Time.time - throwTime >= resetTime)
        {
            Debug.Log("[BarbarianThrowAxe] Resetting axe");
            ResetAxe();
        }
    }

    private void ThrowAxe(float force)
    {
        Debug.Log("[BarbarianThrowAxe] ThrowAxe called");

        // Detach from parent and apply force
        transform.parent = null;
        Vector3 throwDirection = playerHead.forward;

        axeRigidbody.isKinematic = false;
        axeRigidbody.AddForce(throwDirection * force, ForceMode.Impulse);

        isThrown = true;
        throwTime = Time.time;
    }

    private void ResetAxe()
    {
        Debug.Log("[BarbarianThrowAxe] ResetAxe called");

        // Reattach to parent and reset position/rotation
        transform.parent = axeParent;
        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;

        axeRigidbody.isKinematic = true;
        isThrown = false;

        // Automatically reactivate the axe after resetting
        gameObject.SetActive(true);
        Debug.Log("[BarbarianThrowAxe] Axe re-enabled after reset");
    }
}
