using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
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

    private bool SuperJumpEnabled = false;
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
    }

    public override void OnDrop()
    {
        SetKinematicState(false);
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(UpdateKinematicState));
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
        }
    }

    #endregion

    public override void OnDeserialization()
    {
        if (isDestroyed == true)
        {
            DestroyPotion();
        }
    }

    #region On Collision Enter

    private void OnCollisionEnter(Collision collision)
    {
        if (IOT.ItemType == "PotionSuperJumping")
        {
            ActivateSuperJump();          
        }

        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TriggerVFXandDestroy));
        OnDeserialization();
    }

    #endregion

    #region Destroy Potions and Play VFX 

    public void TriggerVFXandDestroy()
    {
        isDestroyed = true;
        RequestSerialization();

        if (localPlayer == Networking.LocalPlayer)
        {
            TriggerPotionBreakEffect();
            DestroyPotion();
        }

        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TriggerPotionBreakEffect));
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(DestroyPotion));
        OnDeserialization();
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

    #endregion

    #region Super Jump Effect

    public void ActivateSuperJump()
    {
        if (localPlayer == Networking.LocalPlayer)
        {
            SuperJumpEnabled = true;
            localPlayer.SetJumpImpulse(15);
            SendCustomEventDelayedSeconds(nameof(DeactivateSuperJump), 10f);
        }
    }

    public void DeactivateSuperJump()
    {
        if (localPlayer == Networking.LocalPlayer)
        {
            SuperJumpEnabled = false;
            localPlayer.SetJumpImpulse(3);
        }
    }

    #endregion

    
}