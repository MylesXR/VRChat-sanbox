
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tether
{
    /// <summary>
    /// TetherController input script for VRC_Pickup based grapple guns.
    /// </summary>
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

        public MeshRenderer axeMeshRenderer;

        private bool currentlyHeld = false;
        private bool resetPosition;

        public float resetTime = 10f;

        private float dropTime;
        public void Update()
        {
            if (!currentlyHeld && Time.time - dropTime >= resetTime)
            {
                resetPosition = true;
            }
        }

        public void LateUpdate()
        {
            if (!currentlyHeld && resetPosition && hasReturnPoint)
            {
                transform.SetPositionAndRotation(returnPoint.position, returnPoint.rotation);
                axeMeshRenderer.enabled = false;
            }
        }

        private void OnEnable()
        {
            if (!currentlyHeld && hasReturnPoint)
            {
                transform.SetPositionAndRotation(returnPoint.position, returnPoint.rotation);
                axeMeshRenderer.enabled = false;
            }
        }

        private void OnDisable()
        {
            currentlyHeld = false;
            axeMeshRenderer.enabled = false;
        }

        override public void OnPickup()
        {
            resetPosition = false;
            currentlyHeld = true;
            axeMeshRenderer.enabled = true;
        }

        override public void OnDrop()
        {
            currentlyHeld = false;
            dropTime = Time.time;
        }
    }
}
