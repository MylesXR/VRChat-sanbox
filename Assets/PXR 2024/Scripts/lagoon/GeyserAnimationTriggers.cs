using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GeyserAnimationTriggers : UdonSharpBehaviour
{
    public Animator thisGeyser;
    public Animator otherGeyser;
    public string rockObject;
    public float triggerCooldown = 2.0f;  // Cooldown time in seconds

    private bool objectInside = false;  // Track if the object is inside the trigger

    //To do, fix glitching in multi
    void Start()
    {
        // Initialize if needed
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is not already inside the trigger
        if (!objectInside && other.gameObject.name == rockObject)
        {
            objectInside = true;  // Mark that the object is inside the trigger
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayAnimationSet1");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Only trigger exit if the object is already marked as inside
        if (objectInside && other.gameObject.name == rockObject)
        {
            objectInside = false;  // Mark that the object has exited the trigger
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayAnimationSet2");
        }
    }

    public void PlayAnimationSet1()
    {
        thisGeyser.SetTrigger("PlayAnimation1");
        otherGeyser.SetTrigger("PlayAnimation2");
    }

    public void PlayAnimationSet2()
    {
        thisGeyser.SetTrigger("PlayAnimation4");
        otherGeyser.SetTrigger("PlayAnimation3");
    }
}
