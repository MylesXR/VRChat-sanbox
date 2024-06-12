using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BarbarianObjectManager : UdonSharpBehaviour
{
    public GameObject[] barbarianObjects;
    [UdonSynced] private bool isBarbarian;

    void Start()
    {
        isBarbarian = false;
        ToggleBarbarianObjects();
    }

    public void SetAsBarbarian()
    {
        isBarbarian = true;
        RequestSerialization();
        ToggleBarbarianObjects();
    }

    public void SetAsNotBarbarian()
    {
        isBarbarian = false;
        RequestSerialization();
        ToggleBarbarianObjects();
    }

    public override void OnDeserialization()
    {
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