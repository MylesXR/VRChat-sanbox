using UdonSharp;
using UnityEngine;

public class VRC_ToggleKinematicOff_OnPickup : UdonSharpBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true; // Set kinematic to true at the start
        }
    }

    public override void OnPickup()
    {
        if (rb != null)
        {
            rb.isKinematic = false; // Toggle kinematic off when picked up
        }
    }
}
