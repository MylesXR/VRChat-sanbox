using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BreakableObjectAlchemist : UdonSharpBehaviour
{
    public void DestroyObject()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "NetworkDestroyObject");
    }

    public void NetworkDestroyObject()
    {
        Networking.Destroy(gameObject); // Networked destruction
    }
}
