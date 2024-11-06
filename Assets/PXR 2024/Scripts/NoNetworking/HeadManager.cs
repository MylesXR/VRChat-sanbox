
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HeadManager : UdonSharpBehaviour
{
    public GameObject headTracker;
    private VRCPlayerApi localPlayer;
    void Start()
    {
        localPlayer = Networking.LocalPlayer;
        if (headTracker != null)
        {
            headTracker.SetActive(true);
            Networking.SetOwner(Networking.LocalPlayer, headTracker);
        }
    }
}
