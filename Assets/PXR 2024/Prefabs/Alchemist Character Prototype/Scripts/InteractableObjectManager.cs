using UdonSharp;
using UnityEngine;
using TMPro;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

public class InteractableObjectManager : UdonSharpBehaviour
{
    [Space(5)][Header("Players Inventory Items")][Space(10)]
    public int HerbsCollected = 10;
    public int FlowersCollected = 10;
    public int GemstonesCollected = 10;
    public int PotionWallBreakerCollected = 0;
    public bool CraftPotionWallBreaker;

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
        HerbsCollected++;
        UpdateUI();
    }

    public void IncrementFlowersCollected()
    {

        FlowersCollected++;
        UpdateUI();
    }

    public void IncrementGemstonesCollected()
    {


        GemstonesCollected++;
        UpdateUI();
    }

    public void IncrementPotionWallBreakerCollected()
    {


        PotionWallBreakerCollected++;
        UpdateUI();
    }

    public void CanCraftPotionWallBreaker()
    {

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
