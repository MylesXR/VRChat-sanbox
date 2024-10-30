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

    [Space(5)][Header("Potion Crafting Bools")][Space(10)]  
    public bool CraftPotionWallBreaking;
    public bool CraftPotionSuperJumping;
    public bool CraftPotionWaterWalking;

    private int playerIndex;
    public int[] potionPoolPlayerIds;

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
        DeactivateAllPotionPools();

        int maxPlayers = 100; 
        potionPoolPlayerIds = new int[maxPlayers];
        for (int i = 0; i < potionPoolPlayerIds.Length; i++)
        {
            potionPoolPlayerIds[i] = -1; // Set unused slots to -1
        }
        debugMenu.Log("All potion pools have been deactivated for local player at scene start.");
    }


    private void DeactivateAllPotionPools()
    {
        // Deactivate all Wall Breaker potion pool objects
        foreach (var pool in wallBreakerPotionPool)
        {
            if (pool != null)
            {
                DeactivateAllPoolObjects(pool);
                pool.gameObject.SetActive(false);
            }
        }

        // Deactivate all Super Jump potion pool objects
        foreach (var pool in superJumpPotionPool)
        {
            if (pool != null)
            {
                DeactivateAllPoolObjects(pool);
                pool.gameObject.SetActive(false);
            }
        }

        // Deactivate all Water Walking potion pool objects
        foreach (var pool in waterWalkingPotionPool)
        {
            if (pool != null)
            {
                DeactivateAllPoolObjects(pool);
                pool.gameObject.SetActive(false);
            }
        }
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
            DeactivateAllPotionPools();
            DestroyAllDestroyedPotions();
            AssignPotionPool(player);
            debugMenu.Log($"Player {player.displayName} joined and was assigned a new potion pool.");

            // Enforce proper state of potions for the new player
            foreach (VRCObjectPool pool in wallBreakerPotionPool)
            {
                EnforcePotionOwnershipAndState(pool);
            }
            foreach (VRCObjectPool pool in superJumpPotionPool)
            {
                EnforcePotionOwnershipAndState(pool);
            }
            foreach (VRCObjectPool pool in waterWalkingPotionPool)
            {
                EnforcePotionOwnershipAndState(pool);
            }
        }
    }

    private void EnforcePotionOwnershipAndState(VRCObjectPool pool)
    {
        foreach (GameObject obj in pool.Pool)
        {
            PotionCollisionHandler potionHandler = obj.GetComponent<PotionCollisionHandler>();
            if (potionHandler != null)
            {
                // Ensure ownership is transferred to the local player before enforcing state
                Networking.SetOwner(Networking.LocalPlayer, obj);

                // Enforce destroyed state
                if (potionHandler.isDestroyed)
                {
                    obj.SetActive(false);
                    if (debugMenu != null)
                    {
                        debugMenu.Log($"Potion in pool {pool.name} marked as destroyed and deactivated.");
                    }
                }
                else
                {
                    obj.SetActive(true); // Ensure the potion is active if it's not destroyed
                    if (debugMenu != null)
                    {
                        debugMenu.Log($"Potion in pool {pool.name} is active for player {Networking.LocalPlayer.displayName}.");
                    }
                }
            }
        }
    }



    private void DestroyAllDestroyedPotions()
    {
        debugMenu.Log("InteractableObjectManager: Destroying all potions that are marked as destroyed.");

        // Iterate through all potion pools and destroy objects marked as destroyed
        foreach (VRCObjectPool pool in wallBreakerPotionPool)
        {
            if (pool != null)
            {
                DestroyPotionsInPool(pool);
            }
        }

        foreach (VRCObjectPool pool in superJumpPotionPool)
        {
            if (pool != null)
            {
                DestroyPotionsInPool(pool);
            }
        }

        foreach (VRCObjectPool pool in waterWalkingPotionPool)
        {
            if (pool != null)
            {
                DestroyPotionsInPool(pool);
            }
        }
    }

    private void DestroyPotionsInPool(VRCObjectPool pool)
    {
        foreach (GameObject obj in pool.Pool)
        {
            PotionCollisionHandler potionHandler = obj.GetComponent<PotionCollisionHandler>();
            if (potionHandler != null)
            {
                if (potionHandler.isDestroyed)
                {
                    obj.SetActive(false); // Deactivate the destroyed potion
                    if (debugMenu != null)
                    {
                        debugMenu.Log($"InteractableObjectManager: Destroying potion in pool {pool.name} marked as destroyed.");
                    }
                }  
            }
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
                ResetAndDeactivatePools(playerIndex);
                debugMenu.Log($"Player {player.displayName} left, cleared pool index {playerIndex}");
            }
        }

        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(ResetAndDeactivatePools));
    }

    #endregion

    #region Reset and Deactivate Potions

    private void ResetAndDeactivatePools(int index)
    {
        VRCObjectPool wallBreakerPool = GetWallBreakerPotionPool(index);
        if (wallBreakerPool != null)
        {
            DeactivateAllPoolObjects(wallBreakerPool);
            wallBreakerPool.gameObject.SetActive(false);
            debugMenu.Log($"Wall Breaker pool at index {index} reset and fully deactivated.");
        }

        VRCObjectPool superJumpPool = GetSuperJumpPotionPool(index);
        if (superJumpPool != null)
        {
            DeactivateAllPoolObjects(superJumpPool);
            superJumpPool.gameObject.SetActive(false);
            debugMenu.Log($"Super Jump pool at index {index} reset and fully deactivated.");
        }

        VRCObjectPool waterWalkingPool = GetWaterWalkingPotionPool(index);
        if (waterWalkingPool != null)
        {
            DeactivateAllPoolObjects(waterWalkingPool);
            waterWalkingPool.gameObject.SetActive(false);
            debugMenu.Log($"Water Walking pool at index {index} reset and fully deactivated.");
        }
    }

    private void DeactivateAllPoolObjects(VRCObjectPool pool)
    {
        foreach (GameObject obj in pool.Pool)
        {
            if (obj != null && obj.activeSelf)
            {
                obj.SetActive(false);  
            }
        }
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