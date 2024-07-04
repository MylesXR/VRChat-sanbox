using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class InteractableObjectTracker : UdonSharpBehaviour
{
    [Space(5)]
    [Header("Interactable Items")]
    [SerializeField] GameObject Herb;
    [SerializeField] GameObject Flower;
    [SerializeField] GameObject Gemstone;
    [SerializeField] GameObject Item4;
    [SerializeField] GameObject Item5;
    [SerializeField] GameObject Item6;
    [SerializeField] GameObject Item7;
    [SerializeField] GameObject Item8;

    [Space(5)]
    [Header("Potions")]
    [Space(10)]
    [SerializeField] GameObject PotionWallBreaker;
    [SerializeField] GameObject Potion2;
    [SerializeField] GameObject Potion3;
    [SerializeField] GameObject Potion4;
    private Rigidbody PotionWallBreakerRB;

    [Space(5)]
    [Header("Item Management")]
    [Space(10)]
    public string ItemType;
    public InteractableObjectManager IOM; //must be public

    [Header("Visual Indicators")]
    [SerializeField] GameObject visualIndicatorPrefab;
    private GameObject visualIndicatorInstanceOnPickup;
    private GameObject visualIndicatorInstanceOnDrop;

    private VRCPlayerApi localPlayer;

    void Start()
    {

        if (PotionWallBreaker != null)
        {
            PotionWallBreakerRB = PotionWallBreaker.GetComponent<Rigidbody>();
            if (ItemType == "PotionWallBreaker" && PotionWallBreakerRB != null)
            {
                PotionWallBreakerRB.isKinematic = true;
            }
        }
    }

    public override void OnPickup()
    {
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
        SyncOnPickup();
    }

    public void SyncOnPickup()
    {
        switch (ItemType)
        {
            case "Herb":
                HandleItemPickup(Herb);
                IOM.IncrementHerbsCollected();
                break;

            case "Flower":
                HandleItemPickup(Flower);
                IOM.IncrementFlowersCollected();
                break;

            case "Gemstone":
                HandleItemPickup(Gemstone);
                IOM.IncrementGemstonesCollected();
                break;

            case "PotionWallBreaker":
                PotionWallBreakerRB.isKinematic = false;
                DestroyVisualIndicators();
                ShowRadiusIndicatorOnPickup();
                break;
        }
    }

    private void HandleItemPickup(GameObject item)
    {
        if (item != null)
        {
            if (!Networking.IsOwner(item))
            {
                Networking.SetOwner(Networking.LocalPlayer, item);
            }
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
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
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
