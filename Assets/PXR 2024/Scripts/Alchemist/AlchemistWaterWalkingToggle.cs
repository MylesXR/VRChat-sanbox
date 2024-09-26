
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AlchemistWaterWalkingToggle : UdonSharpBehaviour
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
            waterWalkingObject.SetActive(true);
        }
    }
}
