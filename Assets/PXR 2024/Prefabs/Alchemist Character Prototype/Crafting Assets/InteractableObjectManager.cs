using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class InteractableObjectManager : UdonSharpBehaviour
{
    public int HerbsCollected = 0;
    public int FlowersCollected = 0;
    public int GemstonesCollected = 0;
    public int PotionWallBreakerCollected = 0;
    public bool CraftPotionWallBreaker;

    //public GameObject PotionWallBreaker;

    public Text HerbsText;
    public Text FlowersText;
    public Text GemstonesText;
    public Text PotionWallBreakerText;

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

    // Method to craft a potion
    public void CanCraftPotionWallBreaker()
    {
        if (HerbsCollected >= 1 && FlowersCollected >= 1 && GemstonesCollected >= 1)
        {
            Debug.Log("Crafting Now");
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

    // Method to update the UI text components
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
}
