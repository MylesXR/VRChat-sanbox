using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class PotionCollisionHandler : UdonSharpBehaviour
{
    [SerializeField] GameObject potionBreakVFX;
    private GameObject objectToDestroy;
    public DebugMenu debugMenu;

    [UdonSynced] public bool isKinematic = true;
    [UdonSynced] public bool shouldDestroy = false;

    public void SetObjectToDestroy(GameObject target)
    {
        objectToDestroy = target;
        if (debugMenu != null)
        {
            debugMenu.Log("Object to destroy set to: " + objectToDestroy.name);
        }
    }

    public override void OnDeserialization()
    {
        UpdateKinematicState();
        if (shouldDestroy)
        {
            DestroyPotion();
        }
    }

    private void UpdateKinematicState()
    {
        Rigidbody potionRigidbody = GetComponent<Rigidbody>();
        if (potionRigidbody != null)
        {
            potionRigidbody.isKinematic = isKinematic;
            potionRigidbody.useGravity = !isKinematic;

            if (debugMenu != null)
            {
                debugMenu.Log("Potion Rigidbody settings updated: isKinematic = " + isKinematic);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (debugMenu != null)
        {
            debugMenu.Log("Potion has collided with: " + collision.gameObject.name);
        }

        if (Networking.IsOwner(gameObject))
        {
            if (collision.gameObject == objectToDestroy)
            {
                if (debugMenu != null)
                {
                    debugMenu.Log("Potion collided with the destroyable object: " + objectToDestroy.name);
                }
                Destroy(objectToDestroy);
            }

            TriggerPotionBreakEffect();
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TriggerPotionBreakEffectNetworked));
            SetShouldDestroy(true);
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(DestroyPotionNetworked));
        }
    }

    public void SetKinematicState(bool state)
    {
        isKinematic = state;
        RequestSerialization();  // Sync the kinematic state over the network
        UpdateKinematicState();  // Apply the kinematic state immediately on the local potion
    }

    public void SetShouldDestroy(bool state)
    {
        shouldDestroy = state;
        RequestSerialization();  // Sync the destruction state over the network
        if (state)
        {
            DestroyPotion();  // Apply destruction immediately if needed
        }
    }

    private void TriggerPotionBreakEffect()
    {
        if (potionBreakVFX != null)
        {
            GameObject vfxInstance = Instantiate(potionBreakVFX, transform.position, Quaternion.identity);
            Destroy(vfxInstance, 5f);  // Destroy the VFX instance after a few seconds
        }
    }

    public void TriggerPotionBreakEffectNetworked()
    {
        TriggerPotionBreakEffect();  // Trigger the break effect on the networked clients
    }

    private void DestroyPotion()
    {
        if (debugMenu != null)
        {
            debugMenu.Log("Destroying potion.");
        }
        gameObject.SetActive(false);  // Deactivate the potion instead of destroying it
    }

    public void DestroyPotionNetworked()
    {
        DestroyPotion();  // Destroy the potion on all networked clients
    }

    public override void OnPickup()
    {
        SetKinematicState(true);  // When picked up, set to kinematic
    }

    public override void OnDrop()
    {
        SetKinematicState(false);  // When dropped, allow it to be non-kinematic
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(UpdateKinematicState));  // Sync the state with other players
    }
}
