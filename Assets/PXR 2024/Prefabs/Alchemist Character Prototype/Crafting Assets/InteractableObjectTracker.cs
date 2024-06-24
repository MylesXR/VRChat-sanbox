
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class InteractableObjectTracker : UdonSharpBehaviour
{
    [SerializeField] GameObject Herb;
    [SerializeField] GameObject Flower;
    [SerializeField] GameObject Gemstone;
    [SerializeField] GameObject Item4;
    [SerializeField] GameObject Item5;
    [SerializeField] GameObject Item6;
    [SerializeField] GameObject Item7;
    [SerializeField] GameObject Item8;

    [SerializeField] GameObject PotionWallBreaker;
    [SerializeField] GameObject Potion2;
    [SerializeField] GameObject Potion3;
    [SerializeField] GameObject Potion4;


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