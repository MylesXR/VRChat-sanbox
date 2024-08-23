using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerSpeedZone : UdonSharpBehaviour
{
    [Header("Original Speeds")]
    [SerializeField] private float originalWalkSpeed = 4.5f;
    [SerializeField] private float originalRunSpeed = 7f;
    [SerializeField] private float originalStrafeSpeed = 3f;
    [SerializeField] private float originalJumpImpulse = 3f;
    [SerializeField] private float originalGravityStrength = 1f;

    [Header("Slowed Speeds")]
    [SerializeField] private float slowedWalkSpeed = 3.0f;
    [SerializeField] private float slowedRunSpeed = 5.0f;
    [SerializeField] private float slowedStrafeSpeed = 2.0f;
    [SerializeField] private float slowedJumpImpulse = 0.5f;
    [SerializeField] private float increasedGravityStrength = 3f;

    [Header("Debug Menu")]
    [SerializeField] private DebugMenu debugMenu;

    [Header("Gizmo Settings")]
    [SerializeField] private Color gizmoColor = Color.cyan;
    [SerializeField] private bool showGizmos = true; // Checkbox to toggle Gizmos on/off

    private VRCPlayerApi localPlayer;

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        if (localPlayer != null)
        {
            LogInitialSpeeds();
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            ApplySlowedSpeeds();
            if (debugMenu != null)
            {
                debugMenu.Log("Player entered speed zone. Speeds slowed.");
            }
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            RestoreOriginalSpeeds();
            if (debugMenu != null)
            {
                debugMenu.Log("Player exited speed zone. Speeds restored.");
            }
        }
    }

    public void ApplySlowedSpeeds()
    {
        if (localPlayer != null)
        {
            localPlayer.SetWalkSpeed(slowedWalkSpeed);
            localPlayer.SetRunSpeed(slowedRunSpeed);
            localPlayer.SetStrafeSpeed(slowedStrafeSpeed);
            localPlayer.SetJumpImpulse(slowedJumpImpulse);
            localPlayer.SetGravityStrength(increasedGravityStrength);
        }
    }

    public void RestoreOriginalSpeeds()
    {
        if (localPlayer != null)
        {
            localPlayer.SetWalkSpeed(originalWalkSpeed);
            localPlayer.SetRunSpeed(originalRunSpeed);
            localPlayer.SetStrafeSpeed(originalStrafeSpeed);
            localPlayer.SetJumpImpulse(originalJumpImpulse);
            localPlayer.SetGravityStrength(originalGravityStrength);
        }
    }

    private void LogInitialSpeeds()
    {
        if (debugMenu != null)
        {
            debugMenu.Log($"Initial Walk Speed: {originalWalkSpeed}");
            debugMenu.Log($"Initial Run Speed: {originalRunSpeed}");
            debugMenu.Log($"Initial Strafe Speed: {originalStrafeSpeed}");
            debugMenu.Log($"Initial Jump Impulse: {originalJumpImpulse}");
            debugMenu.Log($"Initial Gravity Strength: {originalGravityStrength}");
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return; // Only draw gizmos if the checkbox is enabled

        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            return;
        }

        Gizmos.color = gizmoColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        if (collider.GetType() == typeof(BoxCollider))
        {
            BoxCollider boxCollider = (BoxCollider)collider;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
        else if (collider.GetType() == typeof(SphereCollider))
        {
            SphereCollider sphereCollider = (SphereCollider)collider;
            Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
        }
        else if (collider.GetType() == typeof(CapsuleCollider))
        {
            CapsuleCollider capsuleCollider = (CapsuleCollider)collider;
            DrawWireCapsule(capsuleCollider);
        }
    }

    private void DrawWireCapsule(CapsuleCollider capsuleCollider)
    {
        Vector3 center = capsuleCollider.center;
        float radius = capsuleCollider.radius;
        float height = Mathf.Max(capsuleCollider.height, radius * 2f);
        int direction = capsuleCollider.direction;

        Vector3 pointOffset = Vector3.zero;

        if (direction == 0)
            pointOffset = transform.right * (height / 2 - radius);
        else if (direction == 1)
            pointOffset = transform.up * (height / 2 - radius);
        else if (direction == 2)
            pointOffset = transform.forward * (height / 2 - radius);

        // Draw two spheres at the ends of the capsule
        Gizmos.DrawWireSphere(transform.position + center + pointOffset, radius);
        Gizmos.DrawWireSphere(transform.position + center - pointOffset, radius);

        // Draw lines between the spheres
        if (direction == 0)
        {
            Gizmos.DrawLine(center + transform.up * radius, center - transform.up * radius);
            Gizmos.DrawLine(center + transform.forward * radius, center - transform.forward * radius);
        }
        else if (direction == 1)
        {
            Gizmos.DrawLine(center + transform.right * radius, center - transform.right * radius);
            Gizmos.DrawLine(center + transform.forward * radius, center - transform.forward * radius);
        }
        else if (direction == 2)
        {
            Gizmos.DrawLine(center + transform.up * radius, center - transform.up * radius);
            Gizmos.DrawLine(center + transform.right * radius, center - transform.right * radius);
        }
    }
}
