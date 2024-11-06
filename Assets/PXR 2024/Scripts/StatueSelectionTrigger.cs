using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class StatueSelectionTrigger : UdonSharpBehaviour
{
    public GameObject explorer;
    public GameObject barbarian;
    public GameObject alchemist;
    public Bobys_WorldPortalSystem Bobys_WorldPortalSystem;
    public PlayerManager playerManager;

    [SerializeField] GameObject explorerText;
    [SerializeField] GameObject barbarianText;
    [SerializeField] GameObject alchemistText;

    public int thisObjectValue;

    private void Start()
    {
        // Start with all class-related objects and text hidden
        explorer.SetActive(false);
        barbarian.SetActive(false);
        alchemist.SetActive(false);
        explorerText.SetActive(false);
        barbarianText.SetActive(false);
        alchemistText.SetActive(false);
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            ToggleObject();
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            // Hide all class text when the player exits the trigger
            explorerText.SetActive(false);
            barbarianText.SetActive(false);
            alchemistText.SetActive(false);
        }
    }

    private void ToggleObject()
    {
        // Determine class based on `thisObjectValue` and update visibility
        switch (thisObjectValue)
        {
            case 1:
                SetClass("Explorer", explorer);
                explorerText.SetActive(true);
                barbarianText.SetActive(false);
                alchemistText.SetActive(false);
                break;
            case 2:
                SetClass("Barbarian", barbarian);
                barbarianText.SetActive(true);
                explorerText.SetActive(false);
                alchemistText.SetActive(false);
                break;
            case 3:
                SetClass("Alchemist", alchemist);
                alchemistText.SetActive(true);
                explorerText.SetActive(false);
                barbarianText.SetActive(false);
                break;
        }
    }

    private void SetClass(string className, GameObject classObject)
    {
        // Set the class for the local player only
        playerManager.SetPlayerClass(className);

        // Hide all class objects initially
        explorer.SetActive(false);
        barbarian.SetActive(false);
        alchemist.SetActive(false);

        // Show the selected class object
        classObject.SetActive(true);

        // Update the class type in the portal system
        Bobys_WorldPortalSystem.ClassType = className;
    }

    public void UpdateClassObjects()
    {
        // Update the visual objects based on the local player's class
        string className = playerManager.GetPlayerClass();

        explorer.SetActive(className == "Explorer");
        barbarian.SetActive(className == "Barbarian");
        alchemist.SetActive(className == "Alchemist");
    }
}
