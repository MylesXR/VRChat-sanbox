
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AlchemistWaterWalkingToggleSynced : UdonSharpBehaviour
{
    public GameObject waterWalkingObject;
    public string correctObjectName;

    void Start()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == correctObjectName)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ToggleObject");
        }
    }
    public void ToggleObject()
    {
        waterWalkingObject.SetActive(true);
    }
}