using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

public class PotionCollisionHandler : UdonSharpBehaviour
{
    #region Variables

    private VRCPlayerApi localPlayer;

    [SerializeField] GameObject potionBreakVFX;
    public DebugMenu debugMenu;
    public InteractableObjectTracker IOT;
    public InteractableObjectManager IOM;

    public bool isHeld = false; // New flag to track potion hold status
    [UdonSynced] public bool isKinematic = true;
    [UdonSynced] public bool isDestroyed = false; // Tracks if the potion is destroyed

    #endregion

    #region On Start

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
    }

    #endregion

    #region On Pickup & Drop

    public override void OnPickup()
    {
        SetKinematicState(true);
        isHeld = true; // Lock the potion as held
        RequestSerialization(); // Sync hold status across the network
    }

    public override void OnDrop()
    {
        SetKinematicState(false);
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(UpdateKinematicState));
        RequestSerialization(); // Sync hold status across the network
    }

    //private void OnEnable()
    //{
    //    if (isHeld) // Prevent reset if the object is held
    //    {
    //        debugMenu.Log("Potion is held; skipping reset.");
    //        return;
    //    }

    //    isDestroyed = false; // Reset destruction state only if inactive
    //    RequestSerialization();
    //}



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
        }
    }

    #endregion

    #region On Collision Enter

    private void OnCollisionEnter(Collision collision)
    {
        if (Networking.IsOwner(gameObject)) // Only trigger if the local player owns the potion
        {
            if (IOT.ItemType == "PotionSuperJumping")
            {
                ActivateSuperJump();
            }
            isHeld = false; // Release hold lock when dropped
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TriggerVFXandDestroy));
        }
    }


    #endregion

    #region Destroy Potions and Play VFX 

    public void TriggerVFXandDestroy()
    {
        if (Networking.IsOwner(gameObject))
        {
            isDestroyed = true;
            RequestSerialization(); // Sync destruction state
            TriggerPotionBreakEffect();
            DestroyPotion(); // Only owner handles actual destruction
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

    private void DestroyPotion()
    {
        if (debugMenu != null)
        {
            debugMenu.Log("PotionCollisionHandler: Requesting potion destruction.");
        }
        if (localPlayer == Networking.LocalPlayer)
        {
            IOM.DestroyPotion(gameObject, Networking.LocalPlayer.playerId, IOT.ItemType);
        }
    }

    #endregion

    #region Super Jump Effect

    public void ActivateSuperJump()
    {
        if (localPlayer == Networking.LocalPlayer)
        {
            localPlayer.SetJumpImpulse(15);
            SendCustomEventDelayedSeconds(nameof(DeactivateSuperJump), 10f);
        }
    }

    public void DeactivateSuperJump()
    {
        if (localPlayer == Networking.LocalPlayer)
        {
            localPlayer.SetJumpImpulse(3);
        }
    }

    #endregion   
}