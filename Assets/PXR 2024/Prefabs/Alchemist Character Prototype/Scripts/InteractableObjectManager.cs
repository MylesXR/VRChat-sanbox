using UdonSharp;
using UnityEngine;
using TMPro;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

public class InteractableObjectManager : UdonSharpBehaviour
{
    [Space(5)][Header("Players Inventory Items")][Space(10)]
    [UdonSynced] public int HerbsCollected = 10;
    [UdonSynced] public int FlowersCollected = 10;
    [UdonSynced] public int GemstonesCollected = 10;
    [UdonSynced] public int PotionWallBreakerCollected = 0;
    [UdonSynced] public bool CraftPotionWallBreaker;

    [Space(5)][Header("Players Inventory Items Text")][Space(10)]
    [SerializeField] TextMeshProUGUI HerbsText;
    [SerializeField] TextMeshProUGUI FlowersText;
    [SerializeField] TextMeshProUGUI GemstonesText;
    [SerializeField] TextMeshProUGUI PotionWallBreakerText;

    public GameObject BreakableObject;


    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (HerbsText != null)
            HerbsText.text = $"{HerbsCollected}";

        if (FlowersText != null)
            FlowersText.text = $"{FlowersCollected}";

        if (GemstonesText != null)
            GemstonesText.text = $"{GemstonesCollected}";

        if (PotionWallBreakerText != null)
            PotionWallBreakerText.text = $"{PotionWallBreakerCollected}";
    }

    public GameObject GetObjectToDestroy()
    {
        return BreakableObject;
    }

    public void IncrementHerbsCollected()
    {
        if (!Networking.IsOwner(gameObject)) return;

        HerbsCollected++;
        UpdateUI();
    }

    public void IncrementFlowersCollected()
    {
        if (!Networking.IsOwner(gameObject)) return;

        FlowersCollected++;
        UpdateUI();
    }

    public void IncrementGemstonesCollected()
    {
        if (!Networking.IsOwner(gameObject)) return;

        GemstonesCollected++;
        UpdateUI();
    }

    public void IncrementPotionWallBreakerCollected()
    {
        if (!Networking.IsOwner(gameObject)) return;

        PotionWallBreakerCollected++;
        UpdateUI();
    }

    public void CanCraftPotionWallBreaker()
    {
        if (!Networking.IsOwner(gameObject)) return;

        if (HerbsCollected >= 1 && FlowersCollected >= 1 && GemstonesCollected >= 1)
        {
            HerbsCollected--;
            FlowersCollected--;
            GemstonesCollected--;

            UpdateUI();
            CraftPotionWallBreaker = true;
        }
        else
        {
            CraftPotionWallBreaker = false;
        }
    }
}
