using UdonSharp;
using UnityEngine;

[ExecuteInEditMode]
public class View_Collider_Gizmo : UdonSharpBehaviour
{
    public bool showGizmos = true;
    public Color gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // Set Gizmo color
        Gizmos.color = gizmoColor;

        // Get all BoxColliders
        BoxCollider[] boxColliders = GetComponents<BoxCollider>();
        foreach (BoxCollider boxCollider in boxColliders)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }

        // Get all SphereColliders
        SphereCollider[] sphereColliders = GetComponents<SphereCollider>();
        foreach (SphereCollider sphereCollider in sphereColliders)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
        }

        // Get all CapsuleColliders
        CapsuleCollider[] capsuleColliders = GetComponents<CapsuleCollider>();
        foreach (CapsuleCollider capsuleCollider in capsuleColliders)
        {
            DrawWireCapsule(capsuleCollider);
        }
    }

    private void DrawWireCapsule(CapsuleCollider capsuleCollider)
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Vector3 pointOffset = Vector3.zero;
        float radius = capsuleCollider.radius;
        float height = capsuleCollider.height / 2f - radius;

        if (capsuleCollider.direction == 0) // X-axis
            pointOffset = Vector3.right * height;
        else if (capsuleCollider.direction == 1) // Y-axis
            pointOffset = Vector3.up * height;
        else if (capsuleCollider.direction == 2) // Z-axis
            pointOffset = Vector3.forward * height;

        // Draw two spheres at the ends
        Gizmos.DrawWireSphere(capsuleCollider.center + pointOffset, radius);
        Gizmos.DrawWireSphere(capsuleCollider.center - pointOffset, radius);

        // Draw lines between spheres
        if (capsuleCollider.direction == 0)
        {
            Gizmos.DrawLine(capsuleCollider.center + Vector3.up * radius, capsuleCollider.center - Vector3.up * radius);
            Gizmos.DrawLine(capsuleCollider.center + Vector3.forward * radius, capsuleCollider.center - Vector3.forward * radius);
        }
        else if (capsuleCollider.direction == 1)
        {
            Gizmos.DrawLine(capsuleCollider.center + Vector3.right * radius, capsuleCollider.center - Vector3.right * radius);
            Gizmos.DrawLine(capsuleCollider.center + Vector3.forward * radius, capsuleCollider.center - Vector3.forward * radius);
        }
        else if (capsuleCollider.direction == 2)
        {
            Gizmos.DrawLine(capsuleCollider.center + Vector3.up * radius, capsuleCollider.center - Vector3.up * radius);
            Gizmos.DrawLine(capsuleCollider.center + Vector3.right * radius, capsuleCollider.center - Vector3.right * radius);
        }
    }
}
