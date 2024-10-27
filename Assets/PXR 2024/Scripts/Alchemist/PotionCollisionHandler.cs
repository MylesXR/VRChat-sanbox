using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class PotionCollisionHandler : UdonSharpBehaviour
{
    #region Variables

    private GameObject objectToDestroy;
    private GameObject objectToActivate;
    private VRCPlayerApi localPlayer;

    [SerializeField] GameObject potionBreakVFX;
    public DebugMenu debugMenu;
    public InteractableObjectTracker IOT;

    public bool SuperJumpEnabled = false;
    [UdonSynced] public bool isKinematic = true;
    [UdonSynced] public bool shouldDestroy = false;
    [UdonSynced] public bool isDestroyed = false; // Tracks if the potion is destroyed


    #endregion

    #region On Start

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
    }

    #endregion

    #region Set Object to Destroy & Activate

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

    #endregion

    #region Set Kinematic State

    public void SetKinematicState(bool state)
    {
        isKinematic = state;
        RequestSerialization();
        UpdateKinematicState();
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

    #endregion

    #region On Collision Enter

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
                if (IOT.ItemType == "PotionWallBreaking")
                {
                    Destroy(objectToDestroy);
                    if (debugMenu != null)
                    {
                        debugMenu.Log("Potion collided with the destroyable object: " + objectToDestroy.name);
                    }
                }
            }

            // Set destruction flags and request serialization for sync
            isDestroyed = true;
            TriggerVFXandDestroy();
            RequestSerialization(); // Ensure all clients receive the updated state
        }
    }


    #endregion

    #region Super Jump Effect

    public void ActivateSuperJump()
    {
        SuperJumpEnabled = true;
        localPlayer.SetJumpImpulse(15);
        SendCustomEventDelayedSeconds(nameof(DeactivateSuperJump), 10f);
    }

    public void DeactivateSuperJump()
    {
        SuperJumpEnabled = false;
        localPlayer.SetJumpImpulse(3);
    }

    #endregion

    public void SyncPotionState()
    {
        if (isDestroyed)
        {
            DestroyPotionNetworked();
        }
        else
        {
            UpdateKinematicState();
        }
    }




    #region Destroy Potions and Play VFX 

    public void SetShouldDestroy(bool state)
    {
        shouldDestroy = state;
        RequestSerialization();

        if (state)
        {
            DestroyPotion();  // Destroy locally
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(DestroyPotionNetworked));
        }
    }





    public void TriggerVFXandDestroy()
    {
        TriggerPotionBreakEffect();
        isDestroyed = true; // Set destruction flag for sync
        SetShouldDestroy(true); // Local destruction
        RequestSerialization(); // Ensure network synchronization
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TriggerPotionBreakEffectNetworked));
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(DestroyPotionNetworked));
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
        gameObject.SetActive(false);  // Ensure the potion is deactivated for all players
    }

    #endregion

    #region On Pickup & Drop

    public override void OnPickup()
    {
        SetKinematicState(true);
    }

    public override void OnDrop()
    {
        SetKinematicState(false);
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(UpdateKinematicState));
    }

    #endregion

    #region Networking

    public override void OnDeserialization()
    {
        if (isDestroyed)
        {
            DestroyPotion();  // Immediately deactivate if potion is marked destroyed
        }
        else
        {
            UpdateKinematicState();
        }
    }

    #endregion
}