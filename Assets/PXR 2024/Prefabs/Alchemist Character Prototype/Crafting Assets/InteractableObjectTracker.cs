
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class InteractableObjectTracker : UdonSharpBehaviour
{

    public GameObject Herb;
    public GameObject Flower;
    public GameObject Gemstone;
    public Rigidbody PotionWallBreakerRB;
    public GameObject PotionWallBreaker;
    

    public string ItemType;
    public InteractableObjectManager IOM;


    private void Start()
    {
        if (PotionWallBreakerRB == false)
        {
            PotionWallBreakerRB.isKinematic= true;
        }
    }

    public override void OnPickup()
    {
        switch (ItemType)
        {
            case "Herb":
                Destroy(Herb);
                IOM.IncrementHerbsCollected();
                break;

            case "Flower":
                Destroy(Flower);
                IOM.IncrementFlowersCollected();
                break;

            case "Gemstone":
                Destroy(Gemstone);
                IOM.IncrementGemstonesCollected();
                break;
        }

        // Toggle kinematic state off when the potion is picked up for the first time
        if (ItemType == "PotionWallBreaker")
        {

            PotionWallBreakerRB.isKinematic = false;
        }
        else
        {
            Debug.LogError("PotionWallBreakerRB is not properly initialized");
        }
    }
}