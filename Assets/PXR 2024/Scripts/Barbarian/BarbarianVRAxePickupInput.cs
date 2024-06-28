using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace Tether
{
    [RequireComponent(typeof(VRC_Pickup))]
    public class BarbarianVRAxePickupInput : UdonSharpBehaviour
    {
        [Header("Player Attachment")]
        [Tooltip("If this pickup should return to a point when the player lets go.")]
        public bool hasReturnPoint;
        [Tooltip("Position to return this pickup when the player lets go. Typically attached to behind the player's head.")]
        public Transform returnPoint;

        [Header("Scripts")]
        [Tooltip("The VRC_Pickup to use. Required to be on the same game object.")]
        public VRC_Pickup pickup;

        [Header("Inputs")]
        [Tooltip("Input to read if pickup is in left hand.")]
        public string leftInput = "Oculus_CrossPlatform_PrimaryIndexTrigger";
        [Tooltip("Input to read if pickup is in right hand.")]
        public string rightInput = "Oculus_CrossPlatform_SecondaryIndexTrigger";

        [Header("Debugging")]
        [Tooltip("Text component to display debug logs in the VRChat world.")]
        public Text debugText;

        public MeshRenderer axeMeshRenderer;

        //private bool currentlyHeld = false;
        [UdonSynced] private bool resetPosition;
        private VRCPlayerApi owner;

        public float resetTime = 10f;
        private float dropTime;
        private string logMessages = "";

        [UdonSynced] private Vector3 syncedPosition;
        [UdonSynced] private Quaternion syncedRotation;
        [UdonSynced] private bool syncedCurrentlyHeld;
        [UdonSynced] private bool syncedMeshRendererEnabled;

        void Start()
        {
            owner = Networking.GetOwner(gameObject);
            LogDebug($"Start: Axe owned by {owner.displayName}");
        }

        public void Update()
        {
            if (!syncedCurrentlyHeld && Time.time - dropTime >= resetTime)
            {
                resetPosition = true;
            }
        }

        public void LateUpdate()
        {
            if (!syncedCurrentlyHeld && resetPosition && hasReturnPoint)
            {
                transform.SetPositionAndRotation(returnPoint.position, returnPoint.rotation);
                axeMeshRenderer.enabled = false;
                resetPosition = false;

                // Sync state across network
                syncedPosition = returnPoint.position;
                syncedRotation = returnPoint.rotation;
                syncedCurrentlyHeld = false;
                syncedMeshRendererEnabled = false;
                RequestSerialization();

                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SyncAxeReturn");
            }
        }

        private void OnEnable()
        {
            if (!syncedCurrentlyHeld && hasReturnPoint)
            {
                transform.SetPositionAndRotation(returnPoint.position, returnPoint.rotation);
                axeMeshRenderer.enabled = false;
                LogDebug("OnEnable: Axe returned to return point");
            }
        }

        private void OnDisable()
        {
            syncedCurrentlyHeld = false;
            axeMeshRenderer.enabled = false;
            LogDebug("OnDisable: Axe mesh renderer disabled");
        }

        public override void OnPickup()
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            if (player != owner)
            {
                pickup.Drop();
                LogDebug("OnPickup: Non-owner attempted to pick up the axe");
                return;
            }

            resetPosition = false;
            syncedCurrentlyHeld = true;
            axeMeshRenderer.enabled = true;
            LogDebug("OnPickup: Axe picked up");

            // Detach from return point
            transform.SetParent(null);

            // Sync the state
            syncedPosition = transform.position;
            syncedRotation = transform.rotation;
            syncedCurrentlyHeld = true;
            syncedMeshRendererEnabled = true;
            RequestSerialization();

            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SyncAxePickup");
        }

        public override void OnDrop()
        {
            syncedCurrentlyHeld = false;
            dropTime = Time.time;
            LogDebug("OnDrop: Axe dropped");

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                LogDebug("OnDrop: Rigidbody kinematic turned off");
            }

            // Sync the state
            syncedPosition = transform.position;
            syncedRotation = transform.rotation;
            syncedCurrentlyHeld = false;
            syncedMeshRendererEnabled = false;
            RequestSerialization();

            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SyncAxeDrop");
        }

        public override void OnDeserialization()
        {
            // Handle synchronization of the axe mesh renderer and kinematic state
            transform.SetPositionAndRotation(syncedPosition, syncedRotation);
            axeMeshRenderer.enabled = syncedMeshRendererEnabled;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = syncedCurrentlyHeld;
                LogDebug("OnDeserialization: Rigidbody kinematic state synchronized");
            }
        }

        public void SyncAxePickup()
        {
            if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
            {
                axeMeshRenderer.enabled = false;
                LogDebug($"SyncAxePickup: Axe mesh renderer set to false for non-owner player {Networking.LocalPlayer.displayName}");
            }
        }

        public void SyncAxeDrop()
        {
            if (axeMeshRenderer != null)
            {
                axeMeshRenderer.enabled = syncedCurrentlyHeld;
                LogDebug("SyncAxeDrop: Axe mesh renderer state synchronized");
            }

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = syncedCurrentlyHeld;
                LogDebug("SyncAxeDrop: Rigidbody kinematic state synchronized");
            }
        }

        public void SyncAxeReturn()
        {
            if (!syncedCurrentlyHeld && hasReturnPoint)
            {
                transform.SetPositionAndRotation(returnPoint.position, returnPoint.rotation);
                axeMeshRenderer.enabled = false;
                //LogDebug("SyncAxeReturn: Axe returned to return point and visibility updated");
            }
        }

        private void LogDebug(string message)
        {
            Debug.Log(message);
            logMessages += message + "\n";
            if (debugText != null)
            {
                debugText.text = logMessages;
            }
        }

        public override void OnOwnershipTransferred(VRCPlayerApi newOwner)
        {
            owner = newOwner;
            LogDebug($"OnOwnershipTransferred: New owner is {owner.displayName}");
        }
    }
}
