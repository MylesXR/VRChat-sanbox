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

    private VRCPlayerApi localPlayer;
    public AxeAssigner axeManager;  // Set this reference in AxeAssigner script
    public VRCPlayerApi ownerPlayer; // Set this in the AxeAssigner script
    [UdonSynced] public int axeIndex; // Set this in the AxeAssigner script

    private void OnEnable()
    {
        axeParent = transform.parent;
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
        Debug.Log("[BarbarianThrowAxe] OnEnable called");
    }

    private void Start()
    {
        axeRigidbody = GetComponent<Rigidbody>();
        localPlayer = Networking.LocalPlayer;
        Debug.Log("[BarbarianThrowAxe] Start called");
        Debug.Log($"[BarbarianThrowAxe] axeIndex is {axeIndex}");
    }

    private void Update()
    {
        if (localPlayer != null && localPlayer.isLocal)
        {
            if (isThrown == false)
            {
                transform.localPosition = initialLocalPosition;
                transform.localRotation = initialLocalRotation;
            }

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

            if (isThrown && Time.time - throwTime >= resetTime)
            {
                Debug.Log("[BarbarianThrowAxe] Resetting axe");
                ResetAxe();
            }
        }
    }

    private void ThrowAxe(float force)
    {
        Debug.Log($"[BarbarianThrowAxe] Attempting to throw axe with index: {axeIndex}");

        // Ensure only the local player who owns the axe throws it
        if (Networking.IsOwner(gameObject))
        {
            Debug.Log("[BarbarianThrowAxe] Local player owns the axe, throwing it");
            transform.parent = null;
            Vector3 throwDirection = (playerHead.forward + Vector3.up).normalized;

            axeRigidbody.isKinematic = false;
            axeRigidbody.AddForce(throwDirection * force, ForceMode.Impulse);

            isThrown = true;
            throwTime = Time.time;

            // Sync the throw action across the network
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SyncThrow");
        }
        else
        {
            Debug.LogWarning($"[BarbarianThrowAxe] Local player does not own the axe, cannot throw.");
        }
    }

    public void SyncThrow()
    {
        if (Networking.IsOwner(gameObject)) return;

        Debug.Log("[BarbarianThrowAxe] SyncThrow called on remote client");
        transform.parent = null;
        Vector3 throwDirection = (playerHead.forward + Vector3.up).normalized;

        axeRigidbody.isKinematic = false;
        axeRigidbody.AddForce(throwDirection * maxThrowForce, ForceMode.Impulse);

        isThrown = true;
        throwTime = Time.time;
    }

    private void ResetAxe()
    {
        Debug.Log("[BarbarianThrowAxe] ResetAxe called");
        transform.parent = axeParent;
        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;

        axeRigidbody.isKinematic = true;
        isThrown = false;

        // Return the axe to the pool
        axeManager.axePool.Return(gameObject);
        Debug.Log("[BarbarianThrowAxe] Axe returned to pool");
    }
}
