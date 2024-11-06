using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerManager : UdonSharpBehaviour
{
    private string localPlayerClass; // Local-only player class storage

    public void SetPlayerClass(string className)
    {
        // Set the class for the local player
        localPlayerClass = className;
    }

    public string GetPlayerClass()
    {
        // Return the class for the local player
        return localPlayerClass;
    }
}
