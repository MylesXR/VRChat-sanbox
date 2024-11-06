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

            if (localPlayer != null)
            {
                editorMode = false;
            }
        }

        public void Update()
        {
            if (!editorMode && localPlayer != null)
            {
                if (tetherInput > properties.tetherInputDeadzone)
                {
                    if (!tethering)
                    {
                        bool detected = false;
                        RaycastHit hit = new RaycastHit();

                        detected = Physics.Raycast(transform.position, transform.forward, out hit, properties.tetherMaximumLength, properties.tetherDetectionMask);
                        if (!detected)
                        {
                            for (int i = properties.tetherDetectionIncrements; !detected && i > 0; i--)
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

                    if (tethering)
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
                            if (!tetheringRigidbody || (tetheringRigidbody && properties.playerMass <= tetherRigidbody.mass))
                            {
                                Vector3 normal = (worldTetherPoint - transform.position).normalized;
                                Vector3 spring = normal * (distance - tetherLength) * properties.tetherSpringFactor;
                                spring = Vector3.ClampMagnitude(spring * Time.deltaTime, properties.tetherMaximumSpringForce);

                                Vector3 velocity = localPlayer.GetVelocity();
                                Vector3 projected = Vector3.ProjectOnPlane(velocity, normal);
                                localPlayer.SetVelocity(Vector3.MoveTowards(velocity, projected, properties.tetherProjectionRate * Time.deltaTime) + spring);
                            }
                        }
                    }
                }
                else if (tethering)
                {
                    tethering = false;
                    tetheringRigidbody = false;
                }
            }
        }

        public void FixedUpdate()
        {
            if (tetheringRigidbody)
            {
                Vector3 worldTetherPoint = GetTetherPoint();
                float distance = Vector3.Distance(transform.position, worldTetherPoint);

                if (distance > tetherLength)
                {
                    Vector3 normal = (transform.position - worldTetherPoint).normalized;
                    Vector3 spring = normal * (distance - tetherLength) * properties.rigidbodySpringFactor * properties.playerMass;
                    spring = Vector3.ClampMagnitude(spring, properties.rigidbodyMaximumSpringForce * properties.playerMass);

                    Vector3 projectedVelocity = Vector3.ProjectOnPlane(tetherRigidbody.velocity, normal);
                    tetherRigidbody.velocity = Vector3.MoveTowards(tetherRigidbody.velocity, projectedVelocity, properties.rigidbodyProjectionRate * properties.playerMass * Time.deltaTime);
                    tetherRigidbody.AddForceAtPosition(spring, worldTetherPoint);
                }
            }
        }

        public void OnDisable()
        {
            tethering = false;
            tetheringRigidbody = false;
        }

        public float GetInput()
        {
            return tetherInput > properties.tetherInputDeadzone ? tetherInput : 0.0f;
        }

        public void SetInput(float value)
        {
            tetherInput = value;
        }

        public bool IsInputHeld()
        {
            return tetherInput > properties.tetherHoldDeadzone;
        }

        public bool GetTethering()
        {
            return tethering;
        }

        public float GetTetherLength()
        {
            return tethering ? tetherLength / properties.tetherMaximumLength : 1.0f;
        }

        public float GetActualTetherLength()
        {
            return tethering ? tetherLength : 0.0f;
        }

        public GameObject GetTetherObject()
        {
            return tetherObject;
        }

        public Vector3 GetTetherStartPoint()
        {
            return transform.position;
        }

        public Vector3 GetTetherPoint()
        {
            return tetherObject.transform.TransformPoint(tetherPoint);
        }

        public Vector3 GetTetherNormal()
        {
            return tetherNormal;
        }

        public float GetTetherUnwindRate()
        {
            return tetherUnwindRate == 0.0f ? 0.0f : tetherUnwindRate / properties.unwindRate;
        }
    }
}
