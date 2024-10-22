using UdonSharp;
using UnityEngine;

public class VRC_ToggleObject_Local : UdonSharpBehaviour
{
    public GameObject[] targetObjects; // Array of GameObjects to be toggled
    private bool isObjectActive;

    public override void Interact()
    {
        ToggleObjects();
    }

    public void ToggleObjects()
    {
        isObjectActive = !isObjectActive;

        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
            {
                obj.SetActive(isObjectActive);
            }
        }
    }
}