
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class AlchemistObjectManager : UdonSharpBehaviour
{
    public GameObject[] alchemistObjects;
    private bool isAlchemist;

    void Start()
    {
        isAlchemist = false;
        ToggleAlchemistObjects();
    }

    public void SetAsAlchemist()
    {
        isAlchemist = true;
        ToggleAlchemistObjects();
    }

    public void SetAsNotAlchemist()
    {
        isAlchemist = false;
        ToggleAlchemistObjects();
    }

    public void ToggleAlchemistObjects()
    {

        foreach (GameObject obj in alchemistObjects)
        {
            VRCPickup vrcPickup = obj.GetComponent<VRCPickup>();
            vrcPickup.pickupable = isAlchemist;
        }
    } 
}
