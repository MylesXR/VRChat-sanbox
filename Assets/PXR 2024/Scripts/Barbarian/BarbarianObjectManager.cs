using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BarbarianObjectManager : UdonSharpBehaviour
{
    public GameObject[] barbarianObjects;
    private bool isBarbarian;

    void Start()
    {
        isBarbarian = false;
        ToggleBarbarianObjects();
    }

    public void SetAsBarbarian()
    {
        isBarbarian = true;
        ToggleBarbarianObjects();
    }

    public void SetAsNotBarbarian()
    {
        isBarbarian = false;
        ToggleBarbarianObjects();
    }

    public void ToggleBarbarianObjects()
    {
        foreach (GameObject obj in barbarianObjects)
        {
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = isBarbarian;
            }
        }
    }
}