using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class PotionCollisionHandler : UdonSharpBehaviour
{
    #region Variables

    public InteractableObjectTracker IOT;
    [SerializeField] GameObject potionBreakVFX;
    [SerializeField] AudioSource potionBreakSFX;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isHeld = false;  
    private VRCPlayerApi localPlayer;
 
    #endregion

    #region On Start

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    #endregion

    #region On Pickup & Drop

    public override void OnPickup()
    {
        SetKinematicState(true);
        isHeld = true;
    }

    public override void OnDrop()
    {
        SetKinematicState(false);
        isHeld = false;
    }

    #endregion

    #region Set Kinematic State

    public void SetKinematicState(bool state)
    {
        Rigidbody potionRigidbody = GetComponent<Rigidbody>();

        if (potionRigidbody != null)
        {
            potionRigidbody.isKinematic = state;
            potionRigidbody.useGravity = !state;
        }
    }

    #endregion

    #region On Collision Enter

    private void OnCollisionEnter(Collision collision)
    {
        if (!isHeld)
        {
            potionBreakSFX.Play();

            if (IOT.ItemType == "PotionSuperJumping")
            {
                ActivateSuperJump();
            }
            
            TriggerPotionBreakEffect();
            ResetPosition();
        }
    }

    #endregion

    #region Reset Position and VFX

    private void TriggerPotionBreakEffect()
    {       
        if (potionBreakVFX != null)
        {
            GameObject vfxInstance = Instantiate(potionBreakVFX, transform.position, Quaternion.identity);
            Destroy(vfxInstance, 5f);
        }
    }

    private void ResetPosition()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        SetKinematicState(true);
        gameObject.SetActive(false);
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