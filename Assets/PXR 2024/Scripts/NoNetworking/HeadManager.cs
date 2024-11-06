
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HeadManager : UdonSharpBehaviour
{
    public GameObject headTracker;

    void Start()
    {
        headTracker.SetActive(true);
    }

}
