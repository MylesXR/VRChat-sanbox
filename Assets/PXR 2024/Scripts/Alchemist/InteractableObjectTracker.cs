﻿using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

public class InteractableObjectTracker : UdonSharpBehaviour
{
    [Space(5)][Header("Interactable Items")]
    [SerializeField] GameObject Herb;
    [SerializeField] GameObject Flower;
    [SerializeField] GameObject Gemstone;
    [SerializeField] GameObject Mushroom;
    [SerializeField] GameObject Berry;
    [SerializeField] GameObject Stick;

    [Space(5)][Header("Potions")][Space(10)]
    [SerializeField] GameObject PotionWallBreaker;
    [SerializeField] GameObject PotionSuperJumping;
    [SerializeField] GameObject PotionWaterWalking;
    Rigidbody PotionWallBreakerRB;
    Rigidbody PotionSuperJumpingRB;
    Rigidbody PotionWaterWalkingRB;

    [Space(5)][Header("Item Management")][Space(10)]
    public string ItemType;
    public InteractableObjectManager IOM; //must be public
    public VRC.SDK3.Components.VRCObjectSync objectSync; // Reference to VRC Object Sync



    private VRCPlayerApi localPlayer;

    void Start()
    {
        // Wall Breaker Potion Rigidbody Setup
        if (PotionWallBreaker != null)
        {
            PotionWallBreakerRB = PotionWallBreaker.GetComponent<Rigidbody>();
            if (ItemType == "PotionWallBreaking" && PotionWallBreakerRB != null)
            {
                PotionWallBreakerRB.isKinematic = true;
            }
        }

        // Super Jumping Potion Rigidbody Setup
        if (PotionSuperJumping != null)
        {
            PotionSuperJumpingRB = PotionSuperJumping.GetComponent<Rigidbody>();
            if (ItemType == "PotionSuperJumping" && PotionSuperJumpingRB != null)
            {
                PotionSuperJumpingRB.isKinematic = true;
            }
        }

        // Water Walking Potion Rigidbody Setup
        if (PotionWaterWalking != null)
        {
            PotionWaterWalkingRB = PotionWaterWalking.GetComponent<Rigidbody>();
            if (ItemType == "PotionWaterWalking" && PotionWaterWalkingRB != null)
            {
                PotionWaterWalkingRB.isKinematic = true;
            }
        }


        // Disable VRC Object Sync at the start
        if (objectSync != null)
        {
            objectSync.enabled = false;
            Debug.LogWarning("Object Synced OFF");
        }
    }

    public override void OnPickup() 
    {

        // Enable VRC Object Sync on pickup
        if (objectSync != null)
        {
            objectSync.enabled = true;
            Debug.LogWarning("Object Synced ON");
        }

        // Handle item pickups
        if (ItemType == "Herb")
        {
            HandleItemPickup(Herb);
            IOM.IncrementHerbsCollected();
        }
        else if (ItemType == "Flower")
        {
            HandleItemPickup(Flower);
            IOM.IncrementFlowersCollected();
        }
        else if (ItemType == "Gemstone")
        {
            HandleItemPickup(Gemstone);
            IOM.IncrementGemstonesCollected();
        }
        else if (ItemType == "Mushroom")
        {
            HandleItemPickup(Mushroom);
            IOM.IncrementMushroomsCollected();
        }
        else if (ItemType == "Berry")
        {
            HandleItemPickup(Berry);
            IOM.IncrementBerriesCollected();
        }
        else if (ItemType == "Stick")
        {
            HandleItemPickup(Stick);
            IOM.IncrementSticksCollected();
        }

        // Handle potion pickups
        else if (ItemType == "PotionWallBreaking")
        {
            if (PotionWallBreakerRB != null)
            {
                PotionWallBreakerRB.isKinematic = false;
            }
        }
        else if (ItemType == "PotionSuperJumping")
        {
            if (PotionSuperJumpingRB != null)
            {
                PotionSuperJumpingRB.isKinematic = false;
            }
        }
        else if (ItemType == "PotionWaterWalking")
        {
            if (PotionWaterWalkingRB != null)
            {
                PotionWaterWalkingRB.isKinematic = false;
            }
        }
    }

    private void HandleItemPickup(GameObject item)
    {
        if (item != null)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(DeactivateItem));
        }
    }

    #region Activate and Deactivate Item

    public void DeactivateItem()
    {
        gameObject.SetActive(false);
        SendCustomEventDelayedSeconds(nameof(ReactivateItem), 30f);
    }

    public void ReactivateItem()
    {
        gameObject.SetActive(true);
    }

    #endregion
}