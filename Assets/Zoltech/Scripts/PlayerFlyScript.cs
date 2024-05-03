using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerFlyScript : UdonSharpBehaviour
{
    [SerializeField]
    private float flySpeed = 2f; // Fly speed

    private bool isFlying = false; // Flying status

    public override void Interact() // Overriding the Interact method from VRCSDK's UdonSharpBehaviour
    {
        isFlying = !isFlying; // Toggle flying status
        var player = Networking.LocalPlayer;
        if (player == null) return;
        player.SetGravityStrength(isFlying ? 0 : 1); // If player is flying, set gravity to 0
    }

    private void Update()
    {
        var player = Networking.LocalPlayer;
        if (player == null || !isFlying) return;

        // Getting the player's inputs
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 moveDirection = (player.GetRotation() * Vector3.forward * vertical) + (player.GetRotation() * Vector3.right * horizontal);
        player.SetVelocity(moveDirection * flySpeed); // Apply velocity
    }
}
