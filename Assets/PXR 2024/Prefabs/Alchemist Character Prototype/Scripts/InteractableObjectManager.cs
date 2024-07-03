using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRC.SDKBase;
using VRC.Udon;
using static UnityEditor.PlayerSettings;

public class InteractableObjectManager : UdonSharpBehaviour
{

    [Space(5)][Header("Item Amounts")][Space(10)]
    //These variables track how many of each resource or potions the player has
    public int HerbsCollected = 0;
    public int FlowersCollected = 0;
    public int GemstonesCollected = 0;
    public int PotionWallBreakerCollected = 0;
    public bool CraftPotionWallBreaker;


    [Space(5)][Header("Item Text")][Space(10)]
    //These are the text elements that will be updated on the players UI
    [SerializeField] TextMeshProUGUI HerbsText;
    [SerializeField] TextMeshProUGUI FlowersText;
    [SerializeField] TextMeshProUGUI GemstonesText;
    [SerializeField] TextMeshProUGUI PotionWallBreakerText;

    [Space(5)][Header("Other")][Space(10)]
    public GameObject BreakableObject;

    private void Start()
    {      
        UpdateUI();
    }

    public GameObject GetObjectToDestroy()
    {
        return BreakableObject;
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


    public void IncrementHerbsCollected()
    {
        HerbsCollected++;
        UpdateUI();
        Debug.Log($"HerbAmount: {HerbsCollected}");
    }


    public void IncrementFlowersCollected()
    {
        FlowersCollected++;
        UpdateUI();
        Debug.Log($"Flowers collected: {FlowersCollected}");
    }


    public void IncrementGemstonesCollected()
    {
        GemstonesCollected++;
        UpdateUI();
        Debug.Log($"Gemstones collected: {GemstonesCollected}");
    }


    public void IncrementPotionWallBreakerCollected()
    {
        PotionWallBreakerCollected++;
        UpdateUI();
        Debug.Log($"Crafting items collected: {PotionWallBreakerCollected}");
    }


    public void CanCraftPotionWallBreaker()
    {
        if (HerbsCollected >= 1 && FlowersCollected >= 1 && GemstonesCollected >= 1)
        {
            Debug.Log("Crafting");
            HerbsCollected--;
            FlowersCollected--;
            GemstonesCollected--;

            UpdateUI();
            CraftPotionWallBreaker = true;
        }
        else
        {
            Debug.LogWarning("Not enough resources to craft the potion.");
            CraftPotionWallBreaker = false;
        }
    }   
}
