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
        [Tooltip("Original parent of this axe.")]

        public Transform axeParentVR;

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
        private string logMessages = "";

        [Header("Properties")]
        public Rigidbody axeRidgidbodyVR;
        public MeshRenderer axeMeshRenderer;

        [Header("Settings")]
        [Tooltip("The Time it will take this axe to reset after thrown.")]
        public float resetTime = 10f;


        private float dropTime;
        private VRCPlayerApi owner;

        [UdonSynced] private Vector3 syncedPosition;
        [UdonSynced] private Quaternion syncedRotation;
        [UdonSynced] private bool syncedCurrentlyHeld;
        [UdonSynced] private bool syncedMeshRendererEnabled;
        //[UdonSynced] private bool syncedAtReturnPoint;
        [UdonSynced] private bool resetPosition;
        [UdonSynced] private bool positionResetTriggered;
        [UdonSynced] private bool currentlyHeld = false;

        void Start()
        {
            owner = Networking.GetOwner(gameObject);
            LogDebug($"Start: Axe owned by {owner.displayName}");
        }

        public void Update()
        {
            if (Networking.IsOwner(gameObject))
            {
                if (!currentlyHeld && !resetPosition && !positionResetTriggered && Time.time - dropTime >= resetTime)
                {
                    resetPosition = true;
                    positionResetTriggered = true;
                    LogDebug("reset position true update fired");
                } 
                // Sync position, rotation, and gravity state frequently
                //syncedPosition = transform.position;
                //syncedRotation = transform.rotation;

                if (!currentlyHeld && resetPosition && hasReturnPoint)
                {
                    LogDebug("reset position false update fired");
                    axeRidgidbodyVR.isKinematic = true;
                    transform.parent = axeParentVR;
                    transform.SetPositionAndRotation(returnPoint.position, returnPoint.rotation);
                    axeMeshRenderer.enabled = false;
                    syncedCurrentlyHeld = false;
                    syncedMeshRendererEnabled = false;
                    //syncedAtReturnPoint = true;
                    resetPosition = false;

                }
                else
                {

                    syncedMeshRendererEnabled = axeMeshRenderer.enabled;
                    //syncedAtReturnPoint = !currentlyHeld && resetPosition && hasReturnPoint;
                }
                //RequestSerialization();
            }
            else
            {
                // Apply synced state for non-owners
                //transform.SetPositionAndRotation(syncedPosition, syncedRotation);
                axeMeshRenderer.enabled = syncedMeshRendererEnabled; // && !syncedAtReturnPoint;
                currentlyHeld = syncedCurrentlyHeld;
                axeRidgidbodyVR.isKinematic = currentlyHeld;

            }
        }

        private void OnEnable()
        {
            if (!currentlyHeld && hasReturnPoint)
            {
                transform.SetPositionAndRotation(returnPoint.position, returnPoint.rotation);
                axeMeshRenderer.enabled = false;
                LogDebug("OnEnable: Axe returned to return point");
            }
        }

        private void OnDisable()
        {
            currentlyHeld = false;
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
            positionResetTriggered = false;
            currentlyHeld = true;
            axeMeshRenderer.enabled = true;
            LogDebug($"OnPickup: Axe picked up, reset position = {resetPosition}, positionResetTriggered = {positionResetTriggered}");

            // Detach from head tracker
            transform.SetParent(null);

            // Sync the state
            syncedCurrentlyHeld = true;
            syncedMeshRendererEnabled = true;
            //syncedAtReturnPoint = false;
            RequestSerialization();

            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SyncAxePickup");
        }

        public override void OnDrop()
        {
            currentlyHeld = false;
            dropTime = Time.time;
            LogDebug("OnDrop: Axe dropped");


                axeRidgidbodyVR.isKinematic = false;
                LogDebug("OnDrop: Rigidbody kinematic turned off, gravity turned on");


            // Sync the state
            syncedCurrentlyHeld = false;
            syncedMeshRendererEnabled = true; // Keep renderer enabled while axe is moving
            //syncedAtReturnPoint = false;
            RequestSerialization();

            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SyncAxeDrop");
        }

        public override void OnDeserialization()
        {
            // Handle synchronization of the axe mesh renderer and kinematic state
            transform.SetPositionAndRotation(syncedPosition, syncedRotation);
            axeMeshRenderer.enabled = syncedMeshRendererEnabled; //&& !syncedAtReturnPoint;
            currentlyHeld = syncedCurrentlyHeld;

                axeRidgidbodyVR.isKinematic = currentlyHeld;
                //LogDebug("OnDeserialization: Rigidbody kinematic and gravity state synchronized");

        }

        public void SyncAxePickup()
        {
            if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
            {
                axeMeshRenderer.enabled = true;
                LogDebug($"SyncAxePickup: Axe mesh renderer set to true for non-owner player {Networking.LocalPlayer.displayName}");
            }
        }

        public void SyncAxeDrop()
        {
            if (axeMeshRenderer != null)
            {
                axeMeshRenderer.enabled = true;
                LogDebug("SyncAxeDrop: Axe mesh renderer state synchronized");
            }

                axeRidgidbodyVR.isKinematic = currentlyHeld;
                LogDebug("SyncAxeDrop: Rigidbody kinematic and gravity state synchronized");

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
    }
}
