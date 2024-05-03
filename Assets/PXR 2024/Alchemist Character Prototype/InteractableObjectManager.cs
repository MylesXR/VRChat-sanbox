
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class InteractableObjectManager : UdonSharpBehaviour
{
    public int HerbsCollected = 0;
    public int FlowersCollected = 0;
    public int GemstonesCollected = 0;
    public int CraftingItem1Collected = 0;
    public GameObject CraftingItem1;

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
    public void IncrementCraftingItem1Collected()
    {
        CraftingItem1Collected++;
        Debug.Log($"Crafting items collected: {CraftingItem1Collected}");
    }

    public void ActivateCraftingItem()
    {
        if (CraftingItem1 != null)
        {
            CraftingItem1.SetActive(true);
            Debug.Log("CraftingItem1 activated by controller.");
        }
        else
        {
            Debug.LogWarning("CraftingItem1 reference is missing!");
        }
    }
}