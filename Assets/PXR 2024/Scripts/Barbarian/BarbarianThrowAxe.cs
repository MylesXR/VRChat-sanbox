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

    [UdonSynced] private Vector3 syncedPosition;
    [UdonSynced] private Quaternion syncedRotation;
    [UdonSynced] private bool syncedIsThrown = false;

    private void OnEnable()
    {
        axeParent = transform.parent;
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
    }

    private void Start()
    {
        axeRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isThrown == false)
        {
            transform.localPosition = initialLocalPosition;
            transform.localRotation = initialLocalRotation;
        }

        if (Input.GetKeyDown(throwKey) && !isThrown)
        {
            pressTime = Time.time;
        }

        if (Input.GetKeyUp(throwKey) && !isThrown)
        {
            float holdDuration = Time.time - pressTime;
            float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, Mathf.Clamp01(holdDuration / forceHoldTime));
            ThrowAxe(throwForce);
        }

        if (isThrown && Time.time - throwTime >= resetTime)
        {
            ResetAxe();
        }
    }

    private void ThrowAxe(float force)
    {
        transform.parent = null;
        Vector3 throwDirection = (playerHead.forward + Vector3.up).normalized;

        axeRigidbody.isKinematic = false;
        axeRigidbody.AddForce(throwDirection * force, ForceMode.Impulse);

        isThrown = true;
        throwTime = Time.time;

        syncedPosition = transform.position;
        syncedRotation = transform.rotation;
        syncedIsThrown = true;
        RequestSerialization();
    }

    private void ResetAxe()
    {
        transform.parent = axeParent;
        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;

        axeRigidbody.isKinematic = true;
        isThrown = false;

        syncedPosition = transform.position;
        syncedRotation = transform.rotation;
        syncedIsThrown = false;
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        if (syncedIsThrown)
        {
            transform.position = syncedPosition;
            transform.rotation = syncedRotation;
            axeRigidbody.isKinematic = false;
        }
        else
        {
            transform.localPosition = initialLocalPosition;
            transform.localRotation = initialLocalRotation;
            axeRigidbody.isKinematic = true;
        }

        isThrown = syncedIsThrown;
    }
}
