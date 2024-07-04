using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using System.Collections.Generic;

public class InventoryManager : UdonSharpBehaviour
{
    public GameObject inventoryPrefab;
    private InteractableObjectManager[] playerInventories = new InteractableObjectManager[16]; // Adjust size based on maximum players

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player != null && player.isLocal)
        {
            CreatePlayerInventory(player);
        }
    }

    private void CreatePlayerInventory(VRCPlayerApi player)
    {
        if (player == null || inventoryPrefab == null) return;

        GameObject inventoryInstance = VRCInstantiate(inventoryPrefab);
        Networking.SetOwner(player, inventoryInstance);
        inventoryInstance.SetActive(true);

        InteractableObjectManager inventoryManager = inventoryInstance.GetComponent<InteractableObjectManager>();
        

        playerInventories[player.playerId] = inventoryManager;
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (playerInventories[player.playerId] != null)
        {
            Networking.Destroy(playerInventories[player.playerId].gameObject);
            playerInventories[player.playerId] = null;
        }
    }
}
