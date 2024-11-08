using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tether
{
    public class TetherController : UdonSharpBehaviour
    {
        [Header("Properties")]
        public TetherProperties properties;

        private bool editorMode = true;
        private VRCPlayerApi localPlayer;
        private float tetherInput = 0.0f;

        private bool tethering = false;
        private bool tetheringRigidbody = false;

        private Vector3 tetherPoint = Vector3.zero;
        private Vector3 tetherNormal = Vector3.zero;
        private GameObject tetherObject;
        private Rigidbody tetherRigidbody;

        private float tetherLength;
        private float tetherUnwindRate;

        public void Start()
        {
            localPlayer = Networking.LocalPlayer;
            if (localPlayer == null || !localPlayer.isLocal)
            {
                this.enabled = false; // Only allow script to run for local player
                return;
            }
        }

        public void Update()
        {
            if (tetherInput > properties.tetherInputDeadzone)
            {
                if (!tethering)
                {
                    StartTethering();
                }
                if (tethering)
                {
                    UpdateTether();
                }
            }
            else if (tethering)
            {
                StopTethering();
            }
        }

        private void StartTethering()
        {
            RaycastHit hit;
            bool detected = Physics.Raycast(transform.position, transform.forward, out hit, properties.tetherMaximumLength, properties.tetherDetectionMask);

            if (!detected)
            {
                for (int i = properties.tetherDetectionIncrements; i > 0 && !detected; i--)
                {
                    detected = Physics.SphereCast(transform.position, properties.tetherDetectionSize / i, transform.forward, out hit, properties.tetherMaximumLength, properties.tetherDetectionMask);
                }
            }

            if (detected)
            {
                tetherObject = hit.collider.gameObject;
                tetherRigidbody = hit.collider.GetComponent<Rigidbody>();
                tetherPoint = tetherObject.transform.InverseTransformPoint(hit.point);
                tetherNormal = hit.normal;
                tetherLength = Vector3.Distance(transform.position, hit.point);
                tethering = true;
                tetheringRigidbody = properties.manipulatesRigidbodies && tetherRigidbody != null;
            }
        }

        private void UpdateTether()
        {
            if (properties.allowUnwinding && !IsInputHeld())
            {
                tetherUnwindRate = properties.unwindRate * (1.0f - ((tetherInput - properties.tetherInputDeadzone) / (properties.tetherHoldDeadzone - properties.tetherInputDeadzone)));
                tetherLength = Mathf.Clamp(tetherLength + tetherUnwindRate * Time.deltaTime, 0.0f, properties.tetherMaximumLength);
            }

            Vector3 worldTetherPoint = GetTetherPoint();
            float distance = Vector3.Distance(transform.position, worldTetherPoint);

            if (distance > tetherLength)
            {
                Vector3 normal = worldTetherPoint - transform.position;
                normal.Normalize();
                Vector3 spring = normal * (distance - tetherLength) * properties.tetherSpringFactor;
                spring = Vector3.ClampMagnitude(spring * Time.deltaTime, properties.tetherMaximumSpringForce);

                Vector3 velocity = localPlayer.GetVelocity();
                Vector3 projected = Vector3.ProjectOnPlane(velocity, normal);
                localPlayer.SetVelocity(Vector3.MoveTowards(velocity, projected, properties.tetherProjectionRate * Time.deltaTime) + spring);
            }
        }

        private void StopTethering()
        {
            tethering = false;
            tetheringRigidbody = false;
            ResetTether();
        }

        // Added complete reset function for multiplayer stability
        public void ResetTether()
        {
            tetherObject = null;
            tetherRigidbody = null;
            tetherPoint = Vector3.zero;
            tetherNormal = Vector3.zero;
            tetherLength = 0.0f;
            tetherUnwindRate = 0.0f;
        }

        public float GetInput() => tetherInput > properties.tetherInputDeadzone ? tetherInput : 0.0f;
        public void SetInput(float value) => tetherInput = value;
        public bool IsInputHeld() => tetherInput > properties.tetherHoldDeadzone;
        public bool GetTethering() => tethering;

        public float GetTetherLength() => tethering ? tetherLength / properties.tetherMaximumLength : 1.0f;
        public float GetActualTetherLength() => tethering ? tetherLength : 0.0f;
        public GameObject GetTetherObject() => tetherObject;
        public Vector3 GetTetherStartPoint() => transform.position;
        public Vector3 GetTetherPoint() => tetherObject ? tetherObject.transform.TransformPoint(tetherPoint) : Vector3.zero;
        public Vector3 GetTetherNormal() => tetherNormal;
        public float GetTetherUnwindRate() => tetherUnwindRate == 0.0f ? 0.0f : tetherUnwindRate / properties.unwindRate;

        private void OnDisable()
        {
            tethering = false;
            tetheringRigidbody = false;
        }
    }
}
