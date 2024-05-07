using UdonSharp;
using UnityEngine;

public class StatueSelectionTrigger : UdonSharpBehaviour
{
    public GameObject explorer; // explorer
    public GameObject barbarian; // barbarian
    public GameObject alchemist; // alchemist
    public Bobys_WorldPortalSystem Bobys_WorldPortalSystem; 

    public int thisObjectValue; //value of this trigger, changes what class the trigger effects

    private void OnPlayerTriggerEnter()
    {
        ToggleObject();
    }

    public void ToggleObject()
    {
        if (thisObjectValue == 1) //explorer on
        {
            explorer.SetActive(true);
            barbarian.SetActive(false);
            alchemist.SetActive(false);
            Bobys_WorldPortalSystem.ClassType = "Explorer";
        }
        if (thisObjectValue == 2) // barbarian on 
        {
            explorer.SetActive(false);
            barbarian.SetActive(true);
            alchemist.SetActive(false);
            Bobys_WorldPortalSystem.ClassType = "Barbarian";
        }
        if (thisObjectValue == 3) // alchemist on
        {
            explorer.SetActive(false);
            barbarian.SetActive(false);
            alchemist.SetActive(true);
            Bobys_WorldPortalSystem.ClassType = "Alchemist";
        }
    }
}
