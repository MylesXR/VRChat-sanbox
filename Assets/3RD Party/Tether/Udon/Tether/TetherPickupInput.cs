using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tether
{
    [RequireComponent(typeof(VRC_Pickup))]
    public class TetherPickupInput : UdonSharpBehaviour
    {
        [Header("Player Attachment")]
        public bool hasReturnPoint;
        public Transform returnPoint;

        [Header("Scripts")]
        public VRC_Pickup pickup;
        public TetherController controller;

        [Header("Inputs")]
        public string leftInput = "Oculus_CrossPlatform_PrimaryIndexTrigger";
        public string rightInput = "Oculus_CrossPlatform_SecondaryIndexTrigger";

        private bool currentlyHeld = false;
        private VRCPlayerApi localPlayer;

        private void Start()
        {
            localPlayer = Networking.LocalPlayer;
            UpdatePickupState();
        }

        private void Update()
        {
            if (localPlayer != null && currentlyHeld)
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
            UpdatePickupState();
        }

        public override void OnPickup()
        {
            currentlyHeld = true;
            UpdatePickupState();
        }

        public override void OnDrop()
        {
            currentlyHeld = false;
            UpdatePickupState();
        }

        private void UpdatePickupState()
        {
            if (!currentlyHeld && hasReturnPoint)
            {
                transform.SetPositionAndRotation(returnPoint.position, returnPoint.rotation);
            }
        }
    }
}
