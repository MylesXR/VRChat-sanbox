using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

public class PotionCollisionHandler : UdonSharpBehaviour
{
    #region Variables

    private VRCPlayerApi localPlayer;
    private bool SuperJumpEnabled = false;

    [SerializeField] GameObject potionBreakVFX;
    public DebugMenu debugMenu;
    public InteractableObjectTracker IOT;
    public InteractableObjectManager IOM;
  
    [UdonSynced] public bool isKinematic = true;
    [UdonSynced] public bool isDestroyed = false; // Tracks if the potion is destroyed

    #endregion

    #region On Start

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        // Explicitly enforce destroyed state for newly joined players
        if (isDestroyed == true)
        {
            DestroyPotion();
            debugMenu.Log($"PotionCollisionHandler: OnPlayerJoined - Destroying potion for new player: {player.displayName}");
        }
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

    #region On Collision Enter

    private void OnCollisionEnter(Collision collision)
    {
        if (IOT.ItemType == "PotionSuperJumping")
        {
            ActivateSuperJump();          
        }

        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TriggerVFXandDestroy));      
    }

    #endregion

    #region Destroy Potions and Play VFX 

    public void TriggerVFXandDestroy()
    {
        isDestroyed = true;
        TriggerPotionBreakEffect();
        DestroyPotion();      
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

        if (IOM != null)
        {
            // Assuming Networking.LocalPlayer is the player who owns this potion
            IOM.DestroyPotion(gameObject, Networking.LocalPlayer.playerId, IOT.ItemType);
        }
        else
        {
            debugMenu.LogError("InteractableObjectManager reference is missing.");
        }
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