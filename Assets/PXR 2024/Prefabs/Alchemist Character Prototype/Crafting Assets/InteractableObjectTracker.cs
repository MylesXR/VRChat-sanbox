
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class InteractableObjectTracker : UdonSharpBehaviour
{
    public GameObject Herb;
    public GameObject Flower;
    public GameObject Gemstone;
    public GameObject PotionWallBreaker;
    public Rigidbody PotionWallBreakerRB;
    public string ItemType;
    public InteractableObjectManager IOM;

    void Start()
    {
        // Assign the Rigidbody component from the PotionWallBreaker GameObject
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

            case "PotionWallBreaker":
                PotionWallBreakerRB.isKinematic = false;
                break;
        }
    }
}