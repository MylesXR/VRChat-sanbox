using UdonSharp;
using UnityEngine;
using TMPro;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Components;


public class InteractableObjectManager : UdonSharpBehaviour
{

    #region Variables

    #region Inventory Items

    [Space(5)][Header("Inventory Items")][Space(10)]
    public int HerbsCollected = 10;
    public int FlowersCollected = 10;
    public int GemstonesCollected = 10;
    public int PotionWallBreakerCollected = 0;
    public bool CraftPotionWallBreaker;

    #endregion

    #region Inventory Items Text

    [Space(5)][Header("Inventory Items Text")][Space(10)]
    [SerializeField] TextMeshProUGUI HerbsText;
    [SerializeField] TextMeshProUGUI FlowersText;
    [SerializeField] TextMeshProUGUI GemstonesText;
    [SerializeField] TextMeshProUGUI PotionWallBreakerText;

    #endregion

    [Space(5)][Header("Potions")][Space(10)]
    [SerializeField] private VRCObjectPool[] playerPotionPools; // Set this in the Unity Inspector
    private int maxPlayers = 20;


    public GameObject BreakableObject;


    #region Debugging

    [Space(5)][Header("Debug Text")][Space(10)]
    [SerializeField] private DebugMenu debugMenu;
    private VRCPlayerApi localPlayer;

    #endregion

    #endregion




    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        debugMenu.Log("Game Started");
        UpdateUI();
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        base.OnPlayerJoined(player);
        debugMenu.Log($"Player joined: {player.displayName}, ID: {player.playerId}");
        AssignPotionPool(player);
    }

    private void AssignPotionPool(VRCPlayerApi player)
    {
        int playerIndex = player.playerId % maxPlayers;

        if (playerPotionPools[playerIndex] != null)
        {
            debugMenu.Log($"Potion pool already assigned to player {player.displayName} (ID: {player.playerId})");
            return;
        }

        if (playerIndex >= playerPotionPools.Length)
        {
            debugMenu.LogError("Player index exceeds potion pool array length.");
            return;
        }

        VRCObjectPool pool = playerPotionPools[playerIndex];
        if (pool == null)
        {
            debugMenu.LogError($"Potion pool at index {playerIndex} is null.");
            return;
        }

        foreach (GameObject potion in pool.Pool)
        {
            if (potion != null)
            {
                potion.SetActive(false);
            }
            else
            {
                debugMenu.Log("Found a null potion in the pool.");
            }
        }

        debugMenu.Log($"Assigned potion pool to player {player.displayName} (ID: {player.playerId}) at index {playerIndex}");
    }

    public VRCObjectPool GetPlayerPotionPool(int playerId)
    {
        int playerIndex = playerId % maxPlayers;
        if (playerIndex >= playerPotionPools.Length)
        {
            debugMenu.LogError("Player index exceeds potion pool array length.");
            return null;
        }

        VRCObjectPool pool = playerPotionPools[playerIndex];
        if (pool != null)
        {
            debugMenu.Log($"Retrieved potion pool for player ID {playerId} at index {playerIndex}");
            return pool;
        }

        debugMenu.LogError($"No potion pool found for player {playerId} at index {playerIndex}");
        return null;
    }

    private void OnEnable()
    {
        if (localPlayer != null && localPlayer.isLocal)
        {
            Networking.SetOwner(localPlayer, gameObject);
        }
    }








    public void UpdateUI()
    {
        if (HerbsText != null)
            HerbsText.text = $"{HerbsCollected}";

        if (FlowersText != null)
            FlowersText.text = $"{FlowersCollected}";

        if (GemstonesText != null)
            GemstonesText.text = $"{GemstonesCollected}";

        if (PotionWallBreakerText != null)
            PotionWallBreakerText.text = $"{PotionWallBreakerCollected}";
    }

    public GameObject GetObjectToDestroy()
    {
        debugMenu.Log("Returning breakable object.");
        return BreakableObject;
    }

    #region Increment Collected Items

    public void IncrementHerbsCollected()
    {
        HerbsCollected++;
        UpdateUI();
    }

    public void IncrementFlowersCollected()
    {
        FlowersCollected++;
        UpdateUI();
    }

    public void IncrementGemstonesCollected()
    {
        GemstonesCollected++;
        UpdateUI();
    }

    public void IncrementPotionWallBreakerCollected()
    {
        PotionWallBreakerCollected++;
        UpdateUI();
    }

    #endregion

    #region Can Craft Wall Breaker Potion

    public void CanCraftPotionWallBreaker()
    {
        if (HerbsCollected >= 1 && FlowersCollected >= 1 && GemstonesCollected >= 1)
        {
            HerbsCollected--;
            FlowersCollected--;
            GemstonesCollected--;

            UpdateUI();
            CraftPotionWallBreaker = true;
            debugMenu.Log("Potion Wall Breaker crafted.");
        }
        else
        {
            CraftPotionWallBreaker = false;
            debugMenu.Log("Not enough resources to craft Potion Wall Breaker.");
        }
    }

    #endregion
}