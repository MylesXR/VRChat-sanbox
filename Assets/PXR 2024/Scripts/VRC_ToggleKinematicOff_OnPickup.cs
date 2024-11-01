using UdonSharp;
using UnityEngine;

public class VRC_ToggleKinematicOff_OnPickup : UdonSharpBehaviour
{
    [SerializeField] Rigidbody rigidBody;
    [UdonSynced] public bool isKinematic;

    void Start()
    {
        if (rigidBody != null)
        {
            rigidBody.isKinematic = true; // Set kinematic to true at the start
            isKinematic= true;
        }
    }

    public override void OnPickup()
    {
        DisableKinematic();
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DisableKinematic");
    }

    public override void OnDrop()
    {
        DisableKinematic();
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DisableKinematic");
    }

    public void DisableKinematic()
    {
        if (rigidBody != null)
        {
            rigidBody.isKinematic = false; // Toggle kinematic off when picked up
        }
    }
}