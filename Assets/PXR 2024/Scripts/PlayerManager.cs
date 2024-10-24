using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerManager : UdonSharpBehaviour
{
    private int[] playerIds = new int[0];
    private string[] playerClasses = new string[0];

    private int FindPlayerIndex(int playerId)
    {
        //Debug.Log("[PlayerManager] Finding player index for ID: " + playerId);
        for (int i = 0; i < playerIds.Length; i++)
        {
            if (playerIds[i] == playerId)
            {
                //Debug.Log("[PlayerManager] Player index found: " + i);
                return i;
            }
        }
        //Debug.Log("[PlayerManager] Player index not found");
        return -1;
    }

    private void AddPlayer(int playerId, string className)
    {
        //Debug.Log("[PlayerManager] Adding new player ID: " + playerId + " with class: " + className);
        int newLength = playerIds.Length + 1;
        int[] newPlayerIds = new int[newLength];
        string[] newPlayerClasses = new string[newLength];

        for (int i = 0; i < playerIds.Length; i++)
        {
            newPlayerIds[i] = playerIds[i];
            newPlayerClasses[i] = playerClasses[i];
        }

        newPlayerIds[newLength - 1] = playerId;
        newPlayerClasses[newLength - 1] = className;

        playerIds = newPlayerIds;
        playerClasses = newPlayerClasses;

        //Debug.Log("[PlayerManager] New player added successfully");
    }

    public void SetPlayerClass(VRCPlayerApi player, string className)
    {
        int playerId = player.playerId;
        //Debug.Log("[PlayerManager] Setting class for player ID: " + playerId + " to: " + className);
        int index = FindPlayerIndex(playerId);

        if (index >= 0)
        {
            playerClasses[index] = className;
            //Debug.Log("[PlayerManager] Updated existing player's class");
        }
        else
        {
            AddPlayer(playerId, className);
        }
    }

    public string GetPlayerClass(VRCPlayerApi player)
    {
        int playerId = player.playerId;
        //Debug.Log("[PlayerManager] Getting class for player ID: " + playerId);
        int index = FindPlayerIndex(playerId);

        if (index >= 0)
        {
            string className = playerClasses[index];
            //Debug.Log("[PlayerManager] Player class found: " + className);
            return className;
        }
        //Debug.Log("[PlayerManager] Player class not found");
        return null;
    }
}
