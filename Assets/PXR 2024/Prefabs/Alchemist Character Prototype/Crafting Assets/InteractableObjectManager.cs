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

    public GameObject PotionWallBreaker;

    // Add references to UI Text components
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

    public void ActivateCraftingItem()
    {
        if (PotionWallBreaker != null)
        {
            PotionWallBreaker.SetActive(true);
            Debug.Log("CraftingItem1 activated by controller.");
        }
        else
        {
            Debug.LogWarning("CraftingItem1 reference is missing!");
        }
    }

    // Method to update the UI text components
    private void UpdateUI()
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
