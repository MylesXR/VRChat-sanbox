using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Player_Trigger_Zone : UdonSharpBehaviour
{
    public Player_Trigger_Zone_Manager manager; // Reference to the manager script
    private bool isPlayerOnPlate = false; // Tracks whether a player is on the plate

    [Header("Gizmo Settings")]
    [SerializeField] private Color gizmoColor = Color.cyan;
    [SerializeField] private bool showGizmos = true; // Checkbox to toggle Gizmos on/off

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!isPlayerOnPlate && player.isLocal)
        {
            isPlayerOnPlate = true;
            Debug.Log(gameObject.name + " activated by " + player.displayName + ".");
            manager.OnPlateActivated(); // Notify the manager that this plate is activated
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (isPlayerOnPlate && player.isLocal)
        {
            isPlayerOnPlate = false;
            Debug.Log(gameObject.name + " deactivated by " + player.displayName + ".");
            manager.OnPlateDeactivated(); // Notify the manager that this plate is deactivated
        }
    }

    public bool IsPlayerOnPlate()
    {
        return isPlayerOnPlate;
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
