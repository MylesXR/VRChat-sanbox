using UdonSharp;
using UnityEngine;
using TMPro;
using VRC.SDKBase;
using VRC.SDK3.Components;
using VRC.Udon.Common.Interfaces;

public class InteractableObjectManager : UdonSharpBehaviour
{
    #region Variables

    #region Inventory Items

    [Space(5)][Header("Inventory Item Amounts")][Space(10)]
    public int HerbsCollected = 10;
    public int FlowersCollected = 10;
    public int GemstonesCollected = 10;
    public int MushroomsCollected = 10;
    public int BerriesCollected = 10;
    public int SticksCollected = 10;
    public int PotionWallBreakingCollected = 10;
    public int PotionWaterWalkingCollected = 10;
    public int PotionSuperJumpingCollected = 10;

    #endregion

    #region Inventory Items Text

    [Space(5)][Header("Inventory Items Text")][Space(10)]
    [SerializeField] TextMeshProUGUI HerbsText;
    [SerializeField] TextMeshProUGUI FlowersText;
    [SerializeField] TextMeshProUGUI GemstonesText;
    [SerializeField] TextMeshProUGUI MushroomsText;
    [SerializeField] TextMeshProUGUI BerriesText;
    [SerializeField] TextMeshProUGUI SticksText;
    [SerializeField] TextMeshProUGUI PotionWallBreakingText;
    [SerializeField] TextMeshProUGUI PotionWaterWalkingText;
    [SerializeField] TextMeshProUGUI PotionSuperJumpingText;

    #endregion

    #region Potions

    [Space(5)][Header("Potion Pools")][Space(10)]
    [SerializeField] private VRCObjectPool[] wallBreakerPotionPool;
    [SerializeField] private VRCObjectPool[] superJumpPotionPool;
    [SerializeField] private VRCObjectPool[] waterWalkingPotionPool;
    public int[] potionPoolPlayerIds;

    [Space(5)][Header("Potion Crafting Bools")][Space(10)]  
    public bool CraftPotionWallBreaking;
    public bool CraftPotionSuperJumping;
    public bool CraftPotionWaterWalking;

    private int playerIndex;
    

    public bool isOwner;



    #endregion

    #region Debugging

    [Space(5)][Header("Debugging")][Space(10)]
    [SerializeField] private DebugMenu debugMenu;
    private VRCPlayerApi localPlayer;

    #endregion

    #endregion

    #region Object Pools 

    public VRCObjectPool GetWallBreakerPotionPool(int playerIndex) { return wallBreakerPotionPool[playerIndex]; }

    public VRCObjectPool GetSuperJumpPotionPool(int playerIndex) { return superJumpPotionPool[playerIndex]; }

    public VRCObjectPool GetWaterWalkingPotionPool(int playerIndex) { return waterWalkingPotionPool[playerIndex]; }

    #endregion

    #region On Start 

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        UpdateUI();
        UpdateOwner();

        int maxPlayers = 100; 
        potionPoolPlayerIds = new int[maxPlayers];
        for (int i = 0; i < potionPoolPlayerIds.Length; i++)
        {
            potionPoolPlayerIds[i] = -1; // Set unused slots to -1
        }
        debugMenu.Log("All potion pools have been deactivated for local player at scene start.");
    }

    public override void OnOwnershipTransferred(VRCPlayerApi newOwner)
    {
        UpdateOwner();
    }

    private void UpdateOwner()
    {
        isOwner = Networking.IsOwner(gameObject);
    }

    #endregion

    #region On Player Join and Leave

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (localPlayer == Networking.LocalPlayer)
        {
            AssignPotionPool(player);
            debugMenu.Log($"Player {player.displayName} joined and was assigned a new potion pool.");
        }
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        int playerIndex = FindAssignedIndex(player.playerId);

        if (localPlayer == Networking.LocalPlayer)
        {
            if (playerIndex != -1)
            {
                potionPoolPlayerIds[playerIndex] = -1;
                debugMenu.Log($"Player {player.displayName} left, cleared pool index {playerIndex}");
            }
        }
    }

    #endregion

    #region Destroy Potion

    private VRCObjectPool GetPotionPool(int playerId, string potionType)
    {
        int playerIndex = FindAssignedIndex(playerId);
        if (playerIndex == -1)
        {
            debugMenu.LogError("Player's pool index not found.");
            return null;
        }

        VRCObjectPool selectedPool = null;

        // Check potion type to determine the appropriate pool
        switch (potionType)
        {
            case "PotionWallBreaking":
                selectedPool = wallBreakerPotionPool[playerIndex];
                break;
            case "PotionSuperJumping":
                selectedPool = superJumpPotionPool[playerIndex];
                break;
            case "PotionWaterWalking":
                selectedPool = waterWalkingPotionPool[playerIndex];
                break;
            default:
                debugMenu.LogError($"Invalid potion type provided: {potionType}");
                return null;
        }

        if (selectedPool == null)
        {
            debugMenu.LogError($"Pool for potion type {potionType} is not set or inactive for player index {playerIndex}.");
        }

        return selectedPool;
    }

    public void DestroyPotion(GameObject potion, int playerId, string potionType)
    {
        debugMenu.Log($"Attempting to destroy potion: Player ID {playerId}, Potion Type {potionType}");

        VRCObjectPool pool = GetPotionPool(playerId, potionType);

        if (potion == null)
        {
            debugMenu.LogError("DestroyPotion error: Potion reference is null.");
            return;
        }

        if (pool == null)
        {
            debugMenu.LogError("DestroyPotion error: Pool reference is null for the provided player and potion type.");
            return;
        }

        potion.SetActive(false);
        pool.Return(potion);
        debugMenu.Log("Potion successfully returned to pool.");
    }

    private int FindAssignedIndex(int playerId)
    {
        for (int i = 0; i < potionPoolPlayerIds.Length; i++)
        {
            if (potionPoolPlayerIds[i] == playerId)
            {
                return i; // Return the assigned index if found
            }
        }
        return -1; // Return -1 if no assigned index is found
    }

    #endregion

    #region Assign Potion Pool

    private void AssignPotionPool(VRCPlayerApi player)
    {
        for (int i = 0; i < potionPoolPlayerIds.Length; i++)
        {
            // Ensure unassigned slots are available
            if (potionPoolPlayerIds[i] == -1)
            {
                potionPoolPlayerIds[i] = player.playerId;
                AssignPlayerPools(player, i);
                debugMenu.Log($"Assigned pool slot {i} to player {player.displayName}");
                return;
            }
        }
        debugMenu.LogError("No available pool slot for new player.");
    }

    private void AssignPlayerPools(VRCPlayerApi player, int index)
    {
        // Ensure that slot assignment is valid for the player
        if (potionPoolPlayerIds[index] != -1 && potionPoolPlayerIds[index] != player.playerId)
        {
            debugMenu.LogError($"Slot {index} is occupied by another player. Unable to assign to {player.displayName}.");
            return;
        }

        potionPoolPlayerIds[index] = player.playerId; // Update the player ID in the tracking array

        if (localPlayer == Networking.LocalPlayer)
        {
            VRCObjectPool wallBreakerPool = GetWallBreakerPotionPool(index);
            if (wallBreakerPool != null && !wallBreakerPool.gameObject.activeSelf)
            {
                Networking.SetOwner(player, wallBreakerPool.gameObject);
                wallBreakerPool.gameObject.SetActive(true);
                debugMenu.Log($"Wall Breaker pool assigned to player {player.displayName} at index {index}.");
            }

            VRCObjectPool superJumpPool = GetSuperJumpPotionPool(index);
            if (superJumpPool != null && !superJumpPool.gameObject.activeSelf)
            {
                Networking.SetOwner(player, superJumpPool.gameObject);
                superJumpPool.gameObject.SetActive(true);
                debugMenu.Log($"Super Jump pool assigned to player {player.displayName} at index {index}.");
            }

            VRCObjectPool waterWalkingPool = GetWaterWalkingPotionPool(index);
            if (waterWalkingPool != null && !waterWalkingPool.gameObject.activeSelf)
            {
                Networking.SetOwner(player, waterWalkingPool.gameObject);
                waterWalkingPool.gameObject.SetActive(true);
                debugMenu.Log($"Water Walking pool assigned to player {player.displayName} at index {index}.");
            }
        }
    }

    #endregion

    #region Get Potion Pool ID

    public VRCObjectPool GetPlayerPotionPool(int playerId, string potionType)
    {
        // Find the player's assigned pool index in potionPoolPlayerIds
        int playerIndex = -1;
        for (int i = 0; i < potionPoolPlayerIds.Length; i++)
        {
            if (potionPoolPlayerIds[i] == playerId)
            {
                playerIndex = i;
                break;
            }
        }

        if (playerIndex == -1)
        {
            debugMenu.LogError("Player's potion pool not found. Verify assignment process.");
            return null;
        }

        // Retrieve the correct pool based on the potion type
        VRCObjectPool pool = null;
        switch (potionType)
        {
            case "SuperJump":
                pool = superJumpPotionPool[playerIndex];
                break;
            case "WaterWalk":
                pool = waterWalkingPotionPool[playerIndex];
                break;
            case "WallBreaker":
                pool = wallBreakerPotionPool[playerIndex];
                break;
            default:
                debugMenu.LogError("Invalid potion type.");
                return null;
        }

        return pool;
    }

    #endregion

    #region Update Alchemist UI

    public void UpdateUI()
    {
        if (HerbsText != null)
            HerbsText.text = $"{HerbsCollected}";

        if (FlowersText != null)
            FlowersText.text = $"{FlowersCollected}";

        if (GemstonesText != null)
            GemstonesText.text = $"{GemstonesCollected}";

        if (MushroomsText != null)
            MushroomsText.text = $"{MushroomsCollected}";

        if (BerriesText != null)
            BerriesText.text = $"{BerriesCollected}";

        if (SticksText != null)
            SticksText.text = $"{SticksCollected}";

        if (PotionWallBreakingText != null)
            PotionWallBreakingText.text = $"{PotionWallBreakingCollected}";

        if (PotionWaterWalkingText != null)
            PotionWaterWalkingText.text = $"{PotionWaterWalkingCollected}";

        if (PotionSuperJumpingText != null)
            PotionSuperJumpingText.text = $"{PotionSuperJumpingCollected}";
    }

    #endregion
    
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

    public void IncrementMushroomsCollected()
    {
        MushroomsCollected++;
        UpdateUI();
    }

    public void IncrementBerriesCollected()
    {
        BerriesCollected++;
        UpdateUI();
    }

    public void IncrementSticksCollected()
    {
        SticksCollected++;
        UpdateUI();
    }

    public void IncrementPotionWallBreakingCollected()
    {
        PotionWallBreakingCollected++;
        UpdateUI();
    }

    public void IncrementPotionWaterWalkingCollected()
    {
        PotionWaterWalkingCollected++;
        UpdateUI();
    }

    public void IncrementPotionSuperJumpingCollected()
    {
        PotionSuperJumpingCollected++;
        UpdateUI();
    }

    #endregion

    #region Can Craft Potions

    public void CanCraftPotionWallBreaking()
    {
        if (HerbsCollected >= 2 && FlowersCollected >= 2 && GemstonesCollected >= 1)
        {
            HerbsCollected -= 2;
            FlowersCollected -= 2;
            GemstonesCollected--;

            UpdateUI();
            CraftPotionWallBreaking = true;
        }
        else
        {
            CraftPotionWallBreaking = false;
        }
    }

    public void CanCraftPotionSuperJumping()
    {
        if (BerriesCollected >= 2 && FlowersCollected >= 3 && SticksCollected >= 2)
        {
            BerriesCollected -= 2;
            FlowersCollected -= 3;
            SticksCollected -= 2;

            UpdateUI();
            CraftPotionSuperJumping = true;
        }
        else
        {
            CraftPotionSuperJumping = false;
        }
    }

    public void CanCraftPotionWaterWalking()
    {
        if (MushroomsCollected >= 3 && SticksCollected >= 3 && BerriesCollected >= 2 && GemstonesCollected >= 2)
        {
            MushroomsCollected -= 3;
            SticksCollected -= 3;
            BerriesCollected -= 2;
            GemstonesCollected -= 2;
            

            UpdateUI();
            CraftPotionWaterWalking = true;
        }
        else
        {
            CraftPotionWaterWalking = false;
        }
    }

    #endregion
}