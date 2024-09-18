using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class PotionCollisionHandler : UdonSharpBehaviour
{
    [SerializeField] GameObject potionBreakVFX;
    
    private GameObject objectToDestroy;
    private GameObject objectToActivate;

    private VRCPlayerApi localPlayer;
    public DebugMenu debugMenu;

    [UdonSynced] public bool isKinematic = true;
    [UdonSynced] public bool shouldDestroy = false;

    public InteractableObjectTracker IOT;
    public bool SuperJumpEnabled = false;
  

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
    }

    public void SetObjectToDestroy(GameObject target)
    {
        objectToDestroy = target;
        if (debugMenu != null)
        {
            debugMenu.Log("Object to destroy set to: " + objectToDestroy.name);
        }
    }

    public void SetObjectToActivate(GameObject target)
    {
        objectToActivate = target;  // Assign the object to activate
        if (debugMenu != null)
        {
            debugMenu.Log("Object to activate set to: " + objectToActivate.name);
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

                if(IOT.ItemType == "PotionWallBreaking")
                {
                    Destroy(objectToDestroy);
                    if (debugMenu != null)
                    {
                        debugMenu.Log("Potion collided with the destroyable object: " + objectToDestroy.name);
                    }
                }
            }

            if (IOT.ItemType == "PotionSuperJumping")
            {

                ActivateSuperJump();
            }

            if (IOT.ItemType == "PotionWaterWalking")
            {
                if (objectToActivate != null)
                {
                    objectToActivate.SetActive(true);  // Activate the object
                    if (debugMenu != null)
                    {
                        debugMenu.Log("Water Walking Potion has activated the object: " + objectToActivate.name);
                    }
                }
            }

            TriggerVFXandDestroy();
        }
    }






   
    public void ActivateSuperJump()
    {
        SuperJumpEnabled = true;
        localPlayer.SetJumpImpulse(20);
        SendCustomEventDelayedSeconds(nameof(DeactivateSuperJump), 10f);
    }

    public void DeactivateSuperJump()
    {
        SuperJumpEnabled = false;
        localPlayer.SetJumpImpulse(3);
    }









    public void TriggerVFXandDestroy ()
    {
        TriggerPotionBreakEffect();
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TriggerPotionBreakEffectNetworked));
        SetShouldDestroy(true);
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(DestroyPotionNetworked));
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
