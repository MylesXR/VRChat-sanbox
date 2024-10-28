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
        objectToActivate = target; 
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

        isDestroyed = true;
        //TriggerVFXandDestroy();
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TriggerVFXandDestroy));
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



    #region Destroy Potions and Play VFX 

    public void SetShouldDestroy(bool state)
    {
        shouldDestroy = state;

        if (state)
        {
            //DestroyPotion();  // Destroy locally
            //SendCustomNetworkEvent(NetworkEventTarget.All, nameof(DestroyPotionNetworked));
            //SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TriggerVFXandDestroy));
        }
    }

    public void TriggerVFXandDestroy()
    {
        // Ensure the local player is the owner before setting destruction state
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

    
        TriggerPotionBreakEffect();
        DestroyPotion();
        isDestroyed = true;

        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(DestroyPotionNetworked));
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TriggerPotionBreakEffect));
    }


    private void TriggerPotionBreakEffect()
    {
        if (potionBreakVFX != null)
        {
            GameObject vfxInstance = Instantiate(potionBreakVFX, transform.position, Quaternion.identity);
            Destroy(vfxInstance, 5f);
        }
    }

    private void DestroyPotion()
    {
        if (debugMenu != null)
        {
            debugMenu.Log("Destroying potion.");
        }
        gameObject.SetActive(false);
    }

    public void DestroyPotionNetworked()
    {
        DestroyPotion();
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
}