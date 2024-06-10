using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BarbarianVRAxeBehavior : UdonSharpBehaviour
{
    public BarbarianVRAxeManager axeManager;

    private void OnPickup()
    {
        // Logic when the axe is picked up
    }

    private void OnDrop()
    {
        // Logic when the axe is thrown
        axeManager.AxeThrown();
    }
}
