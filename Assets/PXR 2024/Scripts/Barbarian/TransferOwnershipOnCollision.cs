using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Player
{
    public class TransferOwnershipOnCollision : UdonSharpBehaviour
    {
        [Tooltip("The sphere collider that will trigger ownership transfer.")]
        public SphereCollider triggerCollider;

        private void Start()
        {
            if (triggerCollider == null)
            {
                Debug.LogError("Sphere collider not assigned to TransferOwnershipOnCollision script.");
                enabled = false;
            }
        }

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (player.isLocal)
            {
                // Transfer ownership to the local player
                TransferOwnershipToPlayer(player);
                triggerCollider.enabled = false;
                Debug.Log($"{player} is owner of {gameObject}");
            }
        }

        private void TransferOwnershipToPlayer(VRCPlayerApi player)
        {
            // Transfer ownership of this GameObject and all its children
            Networking.SetOwner(player, gameObject);
            TransferOwnershipRecursive(player, transform);
        }

        private void TransferOwnershipRecursive(VRCPlayerApi player, Transform parent)
        {
            foreach (Transform child in parent)
            {
                Networking.SetOwner(player, child.gameObject);
                TransferOwnershipRecursive(player, child);
            }
        }
    }
}
