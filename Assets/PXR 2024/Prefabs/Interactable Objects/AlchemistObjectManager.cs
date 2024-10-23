
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

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
            if (vrcPickup != null) // Check if the VRCPickup component exists
            {
                vrcPickup.pickupable = isAlchemist;
            }
            else
            {
                Debug.LogWarning($"{obj.name} does not have a VRCPickup component attached.");
            }
        }
    }
}