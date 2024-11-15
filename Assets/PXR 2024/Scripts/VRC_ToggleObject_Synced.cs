using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class VRC_ToggleObject_Synced : UdonSharpBehaviour
{
    public GameObject[] targetObjects; 
    [UdonSynced] public bool isObjectActive;

    private void Start()
    {
        if (targetObjects != null && targetObjects.Length > 0)
        {
            isObjectActive = targetObjects[0].activeSelf;
            SetObjectsActive(isObjectActive);
        }
    }

    public override void Interact()
    {
        // Check if the local player is the owner
        if (!Networking.IsOwner(gameObject))
        {
            // Request ownership
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        ToggleObjects();
        RequestSerialization();
    }

    public void ToggleObjects()
    {
        isObjectActive = !isObjectActive;
        SetObjectsActive(isObjectActive);
    }

    public override void OnDeserialization()
    {
        // Update the objects' state when the synced variable changes
        SetObjectsActive(isObjectActive);
    }

    private void SetObjectsActive(bool state)
    {
        if (targetObjects != null)
        {
            foreach (GameObject obj in targetObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(state);
                }
            }
        }
    }
}