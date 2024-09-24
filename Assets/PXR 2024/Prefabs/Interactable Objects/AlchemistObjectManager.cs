
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AlchemistObjectManager : UdonSharpBehaviour
{
    public GameObject[] alchemistObjects;
    private bool isAlchemist;

    void Start()
    {
        isAlchemist = false;
        ToggleBarbarianObjects();
    }

    public void SetAsBarbarian()
    {
        isAlchemist = true;
        ToggleBarbarianObjects();
    }

    public void SetAsNotBarbarian()
    {
        isAlchemist = false;
        ToggleBarbarianObjects();
    }

    public void ToggleBarbarianObjects()
    {
        foreach (GameObject obj in alchemistObjects)
        {
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = isAlchemist;
            }
        }
    } 
}
