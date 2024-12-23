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
    public class TetherPickupInput : UdonSharpBehaviour
    {
        [Header("Player Attachment")]
        [Tooltip("If this pickup should return to a point when the player lets go.")]
        public bool hasReturnPoint;
        [Tooltip("Position to return this pickup when the player lets go. Typically attached to behind the player's head.")]
        public Transform returnPoint;

        [Header("Scripts")]
        [Tooltip("The VRC_Pickup to use. Required to be on the same game object.")]
        public VRC_Pickup pickup;
        [Tooltip("TetherController script.")]
        public TetherController controller;

        [Header("Inputs")]
        [Tooltip("Input to read if pickup is in left hand.")]
        public string leftInput = "Oculus_CrossPlatform_PrimaryIndexTrigger";
        [Tooltip("Input to read if pickup is in right hand.")]
        public string rightInput = "Oculus_CrossPlatform_SecondaryIndexTrigger";

        private bool currentlyHeld = false;

        [UdonSynced, FieldChangeCallback(nameof(SyncedCurrentlyHeld))]
        private bool syncedCurrentlyHeld;
        public bool SyncedCurrentlyHeld
        {
            get => syncedCurrentlyHeld;
            set
            {
                syncedCurrentlyHeld = value;
                UpdatePickupState();
            }
        }

        private void Start()
        {
            // Ensure owner is set correctly on start
            if (Networking.LocalPlayer.IsOwner(gameObject))
            {
                SyncedCurrentlyHeld = currentlyHeld;
            }
        }

        private void Update()
        {
            if (Networking.LocalPlayer.IsOwner(gameObject))
            {
                // Read analog values of triggers instead of using OnPickupUseDown
                if (currentlyHeld)
                {
                    float input = 0.0f;

                    switch (pickup.currentHand)
                    {
                        case VRC_Pickup.PickupHand.Left:
                            input = Input.GetAxis(leftInput);
                            break;
                        case VRC_Pickup.PickupHand.Right:
                            input = Input.GetAxis(rightInput);
                            break;
                    }

                    controller.SetInput(input);
                }
                else
                {
                    controller.SetInput(0.0f);
                }

                // Request serialization to sync the pickup state with other players
                if (syncedCurrentlyHeld != currentlyHeld)
                {
                    SyncedCurrentlyHeld = currentlyHeld;
                    RequestSerialization();
                }
            }
            else
            {
                pickup.pickupable = false;
            }
        }

        private void LateUpdate()
        {
            if (!currentlyHeld && hasReturnPoint)
            {
                transform.SetPositionAndRotation(returnPoint.position, returnPoint.rotation);
            }
        }

        private void OnDisable()
        {
            currentlyHeld = false;
            controller.SetInput(0.0f);
            SyncedCurrentlyHeld = currentlyHeld;
            RequestSerialization();
        }

        public override void OnPickup()
        {
            currentlyHeld = true;
            SyncedCurrentlyHeld = currentlyHeld;
            RequestSerialization();
        }

        public override void OnDrop()
        {
            currentlyHeld = false;
            SyncedCurrentlyHeld = currentlyHeld;
            RequestSerialization();
        }

        private void UpdatePickupState()
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject))
            {
                currentlyHeld = syncedCurrentlyHeld;
                if (!currentlyHeld && hasReturnPoint)
                {
                    transform.SetPositionAndRotation(returnPoint.position, returnPoint.rotation);
                }
            }
        }
    }
}
