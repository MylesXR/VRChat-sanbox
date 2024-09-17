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

    public InteractableObjectTracker IOT;

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
                debugMenu.Log("Potion Rigidbody settings updated after deserialization: isKinematic = " + isKinematic);
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
                if(IOT.ItemType == "PotionWallBreaking")
                {
                    Destroy(objectToDestroy);
                }
                else
                {
                    return;
                }
                
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
        RequestSerialization();
        UpdateKinematicState();
    }

    public void SetShouldDestroy(bool state)
    {
        shouldDestroy = state;
        RequestSerialization();
        if (state)
        {
            DestroyPotion();
        }
    }

    private void TriggerPotionBreakEffect()
    {
        if (potionBreakVFX != null)
        {
            GameObject vfxInstance = Instantiate(potionBreakVFX, transform.position, Quaternion.identity);
            Destroy(vfxInstance, 5f);
        }
    }

    public void TriggerPotionBreakEffectNetworked()
    {
        TriggerPotionBreakEffect();
    }

    private void DestroyPotion()
    {
        if (debugMenu != null)
        {
            debugMenu.Log("Destroying potion.");
        }
        gameObject.SetActive(false); // Deactivate the object instead of destroying it
    }

    public void DestroyPotionNetworked()
    {
        DestroyPotion();
    }

    public override void OnPickup()
    {
        SetKinematicState(true);
    }

    public override void OnDrop()
    {
        SetKinematicState(false);
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(UpdateKinematicState));
    }
}
