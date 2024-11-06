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
        public DebugMenu debugMenu; // Reference to DebugMenu for logging

        [Header("Inputs")]
        public string leftInput = "Oculus_CrossPlatform_PrimaryIndexTrigger";
        public string rightInput = "Oculus_CrossPlatform_SecondaryIndexTrigger";

        private bool currentlyHeld = false;

        private void Start()
        {
            LogDebug("Script started.");
            UpdatePickupState();
        }

        private void Update()
        {
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
                    default:
                        LogWarning("Pickup hand is not detected.");
                        break;
                }

                LogDebug($"Input value: {input}");
                controller.SetInput(input);
            }
            else
            {
                //LogDebug("Currently not held. Setting input to 0.");
                controller.SetInput(0.0f);
            }
        }

        private void LateUpdate()
        {
            if (!currentlyHeld && hasReturnPoint)
            {
                //LogDebug("Resetting position to return point.");
                transform.SetPositionAndRotation(returnPoint.position, returnPoint.rotation);
            }
        }

        private void OnDisable()
        {
            currentlyHeld = false;
            LogDebug("OnDisable called. Resetting input and pickup state.");
            controller.SetInput(0.0f);
            UpdatePickupState();
        }

        public override void OnPickup()
        {
            currentlyHeld = true;
            LogDebug("OnPickup called. Object picked up.");
            UpdatePickupState();
        }

        public override void OnDrop()
        {
            currentlyHeld = false;
            LogDebug("OnDrop called. Object dropped.");
            UpdatePickupState();
        }

        private void UpdatePickupState()
        {
            if (!currentlyHeld && hasReturnPoint)
            {
                transform.SetPositionAndRotation(returnPoint.position, returnPoint.rotation);
                LogDebug("UpdatePickupState: Resetting to return point.");
            }
            else
            {
                LogDebug($"UpdatePickupState: currentlyHeld = {currentlyHeld}");
            }
        }

        private void LogDebug(string message)
        {
            if (debugMenu != null)
            {
                debugMenu.Log(message); // Log to DebugMenu
            }
            else
            {
                Debug.Log(message); // Fallback to Unity console
            }
        }

        private void LogWarning(string message)
        {
            if (debugMenu != null)
            {
                debugMenu.LogWarning(message); // Log warning to DebugMenu
            }
            else
            {
                Debug.LogWarning(message); // Fallback to Unity console
            }
        }
    }
}
