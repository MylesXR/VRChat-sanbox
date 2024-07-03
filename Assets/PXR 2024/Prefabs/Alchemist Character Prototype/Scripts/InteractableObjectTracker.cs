using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class InteractableObjectTracker : UdonSharpBehaviour
{

    [Space(5)][Header("Interactable Items")]
    [SerializeField] GameObject Herb;
    [SerializeField] GameObject Flower;
    [SerializeField] GameObject Gemstone;
    [SerializeField] GameObject Item4;
    [SerializeField] GameObject Item5;
    [SerializeField] GameObject Item6;
    [SerializeField] GameObject Item7;
    [SerializeField] GameObject Item8;


    [Space(5)][Header("Potions")][Space(10)]
    [SerializeField] GameObject PotionWallBreaker;
    [SerializeField] GameObject Potion2;
    [SerializeField] GameObject Potion3;
    [SerializeField] GameObject Potion4;
    private Rigidbody PotionWallBreakerRB;


    [Space(5)][Header("Item Management")][Space(10)]
    public string ItemType;
    public InteractableObjectManager IOM; //must be public


    [Header("Visual Indicators")]
    [SerializeField] GameObject visualIndicatorPrefab;
    private GameObject visualIndicatorInstanceOnPickup;
    private GameObject visualIndicatorInstanceOnDrop;


    void Start()
    {
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
                Destroy(visualIndicatorInstanceOnPickup);
                ShowRadiusIndicatorOnPickup();
                Destroy(visualIndicatorInstanceOnDrop);
                break;
        }
    }



    public override void OnDrop()
    {     
        Destroy(visualIndicatorInstanceOnDrop);
        ShowRadiusIndicatorOnDrop();    
        Destroy(visualIndicatorInstanceOnPickup);
    }





    private void ShowRadiusIndicatorOnPickup()
    {
        if (visualIndicatorPrefab != null)
        {
            Vector3 playerPosition = Networking.LocalPlayer.GetPosition();
            RaycastHit hit;
            if (Physics.Raycast(playerPosition, Vector3.down, out hit))
            {
                visualIndicatorInstanceOnPickup = Instantiate(visualIndicatorPrefab, hit.point, Quaternion.identity);
            }
        }
    }


    private void ShowRadiusIndicatorOnDrop()
    {
        visualIndicatorInstanceOnDrop = Instantiate(visualIndicatorPrefab, PotionWallBreaker.transform);
        visualIndicatorInstanceOnDrop.transform.localPosition = Vector3.down * PotionWallBreaker.transform.localScale.y;
        visualIndicatorInstanceOnDrop.transform.rotation = Quaternion.Euler(0, 90, 0); // Ensure the indicator is flat on the ground
    }
}

