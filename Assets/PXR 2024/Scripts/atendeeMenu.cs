
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

public class atendeeMenu : UdonSharpBehaviour
{
    [Space(5)]
    [Header("Class Settings")]
    [Space(10)]
    public string ClassType;
    public GameObject AlchemistMenu;
    public GameObject BarbarianMenu;
    public GameObject ExplorerMenu;
    public VRCObjectPool potionPool;


    [SerializeField] GameObject PopUpMessageCrafting;
    [SerializeField] GameObject PopUpMessageSpawning;
    [SerializeField] GameObject PopUpMessagePotionAlreadySpawned;


    //[SerializeField] GameObject PotionWallBreaker;
    [SerializeField] Transform PotionsSpawnPoint;


    [SerializeField] InteractableObjectManager IOM;
    [SerializeField] GameObject BreakableObject;

    // Start of Added methods for Attendee Menu
    //These methods have to be outside of the code below for some reason or it will ERROR
    public void AlchemistClass() { ClassType = "Alchemist"; }
    public void BarbarianClass() { ClassType = "Barbarian"; }
    public void ExplorerClass() { ClassType = "Explorer"; }

    public void HidePopupMessage()
    {
        PopUpMessageCrafting.SetActive(false);
        PopUpMessageSpawning.SetActive(false);
        PopUpMessagePotionAlreadySpawned.SetActive(false);
    }



    public void CraftWallBreakerPotion()
    {


        IOM.CanCraftPotionWallBreaker();

        if (IOM.CraftPotionWallBreaker)
        {
            IOM.PotionWallBreakerCollected++;
            IOM.UpdateUI();
            Debug.Log("WALL BREAKER POTION CRAFTED");
            RequestSerialization();
        }
        else
        {
            PopUpMessageCrafting.SetActive(true);
            SendCustomEventDelayedSeconds(nameof(HidePopupMessage), 6f);
            Debug.LogWarning("Not enough resources to craft the potion");
        }
    }

    public void SpawnWallBreakerPotion()
    {


        if (IOM.PotionWallBreakerCollected >= 1)
        {
            //GameObject spawnedPotion = VRCInstantiate(PotionWallBreaker);
            GameObject spawnedPotion = potionPool.TryToSpawn();
            spawnedPotion.transform.position = PotionsSpawnPoint.position;
            spawnedPotion.transform.rotation = PotionsSpawnPoint.rotation;

            Rigidbody potionRigidbody = spawnedPotion.GetComponent<Rigidbody>();

            if (potionRigidbody != null)
            {
                potionRigidbody.isKinematic = true;
            }

            PotionCollisionHandler potionHandler = spawnedPotion.GetComponent<PotionCollisionHandler>();
            if (potionHandler != null)
            {
                potionHandler.SetObjectToDestroy(IOM.GetObjectToDestroy());
            }

            IOM.PotionWallBreakerCollected--;
            IOM.UpdateUI();
            Debug.Log("WALL BREAKER POTION SPAWNED");
            RequestSerialization();
        }
        else
        {
            Debug.LogWarning("NO WALL BREAKER POTIONS IN INVENTORY");
            PopUpMessageSpawning.SetActive(true);
            SendCustomEventDelayedSeconds(nameof(HidePopupMessage), 3f);
        }
    }

    public override void OnDeserialization()
    {
        IOM.UpdateUI();
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        if (Networking.IsOwner(gameObject))
        {
            IOM.UpdateUI();
        }
    }
}
