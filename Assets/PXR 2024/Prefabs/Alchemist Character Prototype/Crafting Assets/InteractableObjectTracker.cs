
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class InteractableObjectTracker : UdonSharpBehaviour
{

    public GameObject Herb;
    public GameObject Flower;
    public GameObject Gemstone;

    public string ItemType;
    public InteractableObjectManager IOC;

    private void Start()
    {
        IOC.PotionWallBreaker.SetActive(false);
    }

    public override void OnPickup()
    {
        if (ItemType == "Herb")
        {
            Destroy(Herb);
            IOC.IncrementHerbsCollected();
        }
        else if (ItemType == "Flower")
        {
            Destroy(Flower);
            IOC.IncrementFlowersCollected();
        }
        else if (ItemType == "Gemstone")
        {
            Destroy(Gemstone);
            IOC.IncrementGemstonesCollected();
        }
        else if (ItemType == "CraftingItem1")
        {
            Destroy(IOC.PotionWallBreaker);
            IOC.IncrementPotionWallBreakerCollected();
        }
 


        // Check if the player has collected one of each item
        if 
           (IOC.HerbsCollected >= 1
            && IOC.FlowersCollected >= 1
            && IOC.GemstonesCollected >= 1)
        {
            IOC.PotionWallBreaker.SetActive(true);

            // Remove objects that were used to craft.
            IOC.HerbsCollected = 0;
            IOC.FlowersCollected = 0;
            IOC.GemstonesCollected = 0;

        }
    }
}