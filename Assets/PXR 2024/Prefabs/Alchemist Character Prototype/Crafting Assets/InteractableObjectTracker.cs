
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
    [SerializeField] GameObject potionBreakVFX;
    private Rigidbody PotionWallBreakerRB;

    [Space(5)][Header("Other")][Space(10)]
    public string ItemType;
    public InteractableObjectManager IOM;


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
                break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (ItemType == "PotionWallBreaker")
        {
            Debug.Log("Potion has collided with the ground.");

            if (potionBreakVFX != null)
            {
                // Instantiate the VFX at the collision point and play it
                GameObject vfxInstance = Instantiate(potionBreakVFX, transform.position, Quaternion.identity);
                ParticleSystem ps = vfxInstance.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();
                    // Destroy the VFX after 6 seconds
                    Destroy(vfxInstance, 6f);
                }
                else
                {
                    Debug.LogWarning("No ParticleSystem component found on the instantiated VFX.");
                }
            }
            else
            {
                Debug.LogWarning("No VFX prefab assigned.");
            }

            // Destroy the potion after collision and VFX
            Destroy(gameObject);
        }
    }


}