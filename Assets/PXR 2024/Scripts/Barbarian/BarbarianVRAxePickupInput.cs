using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tether
{
    [RequireComponent(typeof(VRC_Pickup))]
    public class BarbarianVRAxePickupInput : UdonSharpBehaviour
    {
        [Header("Player Attachment")]
        public bool hasReturnPoint; // If the axe should return to a point when released
        public Transform returnPoint; // Position to return this pickup when released
        public Transform axeParentVR; // Original parent of the axe

        [Header("Scripts")]
        public VRC_Pickup pickup; // Reference to the VRC_Pickup component

        [Header("Inputs")]
        public string leftInput = "Oculus_CrossPlatform_PrimaryIndexTrigger"; // Input for left hand
        public string rightInput = "Oculus_CrossPlatform_SecondaryIndexTrigger"; // Input for right hand

        [Header("Properties")]
        public Rigidbody axeRigidbodyVR;
        public MeshRenderer axeMeshRenderer;

        [Header("Settings")]
        public float resetTime = 10f; // Time before the axe resets after being thrown

        private float dropTime;
        private bool resetPosition = false;
        private bool positionResetTriggered = false;
        private bool currentlyHeld = false;

        void Start()
        {
            // Initialization code, if needed
        }

        void Update()
        {
            // Check if the axe needs to reset position after being dropped
            if (!currentlyHeld && !resetPosition && !positionResetTriggered && Time.time - dropTime >= resetTime)
            {
                resetPosition = true;
                positionResetTriggered = true;
            }

            // Reset position if required and return point is enabled
            if (!currentlyHeld && resetPosition && hasReturnPoint)
            {
                ResetAxePosition();
                resetPosition = false;
            }
        }

        private void ResetAxePosition()
        {
            axeRigidbodyVR.isKinematic = true;
            transform.parent = axeParentVR;
            transform.SetPositionAndRotation(returnPoint.position, returnPoint.rotation);
            axeMeshRenderer.enabled = false;
        }

        public override void OnPickup()
        {
            resetPosition = false;
            positionResetTriggered = false;
            currentlyHeld = true;
            axeMeshRenderer.enabled = true;

            // Detach from head tracker or return point
            transform.SetParent(null);
        }

        public override void OnDrop()
        {
            currentlyHeld = false;
            dropTime = Time.time;
            axeRigidbodyVR.isKinematic = false; // Enable gravity for the axe
        }
    }
}
