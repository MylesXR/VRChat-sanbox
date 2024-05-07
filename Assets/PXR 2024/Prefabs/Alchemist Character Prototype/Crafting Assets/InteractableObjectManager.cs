
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class InteractableObjectManager : UdonSharpBehaviour
{
    public int HerbsCollected = 0;
    public int FlowersCollected = 0;
    public int GemstonesCollected = 0;
    public int PotionWallBreakerCollected = 0;
    public GameObject PotionWallBreaker;

    public void IncrementHerbsCollected()
    {
        HerbsCollected++;
        Debug.Log($"HerbAmount: {HerbsCollected}");
    }

    public void IncrementFlowersCollected()
    {
        FlowersCollected++;
        Debug.Log($"Flowers collected: {FlowersCollected}");
    }

    public void IncrementGemstonesCollected()
    {
        GemstonesCollected++;
        Debug.Log($"Gemstones collected: {GemstonesCollected}");
    }
    public void IncrementPotionWallBreakerCollected()
    {
        PotionWallBreakerCollected++;
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
}