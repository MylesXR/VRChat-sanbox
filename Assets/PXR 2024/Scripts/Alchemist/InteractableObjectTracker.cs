using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
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

    [Header("Visual Indicators")]
    [SerializeField] GameObject visualIndicatorPrefab;
    private GameObject visualIndicatorInstanceOnPickup;
    private GameObject visualIndicatorInstanceOnDrop;

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





    }

    public override void OnPickup()
    {
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
                DestroyVisualIndicators();
                ShowRadiusIndicatorOnPickup();
            }
        }
        else if (ItemType == "PotionSuperJumping")
        {
            if (PotionSuperJumpingRB != null)
            {
                PotionSuperJumpingRB.isKinematic = false;
                DestroyVisualIndicators();
                ShowRadiusIndicatorOnPickup();
            }
        }
        else if (ItemType == "PotionWaterWalking")
        {
            if (PotionWaterWalkingRB != null)
            {
                PotionWaterWalkingRB.isKinematic = false;
                DestroyVisualIndicators();
                ShowRadiusIndicatorOnPickup();
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

    public void DeactivateItem()
    {
        gameObject.SetActive(false);
        SendCustomEventDelayedSeconds(nameof(ReactivateItem), 30f);
    }

    public void ReactivateItem()
    {
        gameObject.SetActive(true);
    }

    public override void OnDrop()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(SyncOnDrop));
    }

    public void SyncOnDrop()
    {
        DestroyVisualIndicators();
        ShowRadiusIndicatorOnDrop();
    }

    private void DestroyVisualIndicators()
    {
        if (visualIndicatorInstanceOnPickup != null)
        {
            Destroy(visualIndicatorInstanceOnPickup);
        }
        if (visualIndicatorInstanceOnDrop != null)
        {
            Destroy(visualIndicatorInstanceOnDrop);
        }
    }

    private void ShowRadiusIndicatorOnPickup()
    {
        if (visualIndicatorPrefab != null)
        {
            Vector3 playerPosition = Networking.LocalPlayer.GetPosition();
            if (Physics.Raycast(playerPosition, Vector3.down, out RaycastHit hit))
            {
                visualIndicatorInstanceOnPickup = VRCInstantiate(visualIndicatorPrefab);
                visualIndicatorInstanceOnPickup.transform.position = hit.point;
                visualIndicatorInstanceOnPickup.transform.rotation = Quaternion.identity;
            }
        }
    }

    private void ShowRadiusIndicatorOnDrop()
    {
        if (visualIndicatorPrefab != null)
        {
            visualIndicatorInstanceOnDrop = VRCInstantiate(visualIndicatorPrefab);
            visualIndicatorInstanceOnDrop.transform.SetParent(PotionWallBreaker.transform);
            visualIndicatorInstanceOnDrop.transform.localPosition = Vector3.down * PotionWallBreaker.transform.localScale.y;
            visualIndicatorInstanceOnDrop.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }
}
