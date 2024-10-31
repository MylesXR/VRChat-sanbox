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

    [UdonSynced] private int wallBreakerPoolVersion = 0;
    [UdonSynced] private int superJumpPoolVersion = 0;
    [UdonSynced] private int waterWalkingPoolVersion = 0;

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

    #region On Player Join and Leave

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (localPlayer == Networking.LocalPlayer)
        {
            AssignPotionPool(player);
            ResetPoolsForNewPlayer(player); // Reset pools for the joining player
            Networking.SetOwner(localPlayer, gameObject); // Ensure ownership for state sync

            debugMenu.Log($"Player {player.displayName} joined and was assigned a new potion pool.");
        }
    }


    private void ResetPoolsForNewPlayer(VRCPlayerApi player)
    {
        foreach (VRCObjectPool pool in wallBreakerPotionPool)
        {
            ResetPoolObjects(pool);
        }
        foreach (VRCObjectPool pool in superJumpPotionPool)
        {
            ResetPoolObjects(pool);
        }
        foreach (VRCObjectPool pool in waterWalkingPotionPool)
        {
            ResetPoolObjects(pool);
        }
        debugMenu.Log("All pools reset for new player join.");
    }

    private void ResetPoolObjects(VRCObjectPool pool)
    {
        if (pool == null) return;

        foreach (GameObject obj in pool.Pool)
        {
            // Get PotionCollisionHandler to check isHeld status
            var collisionHandler = obj.GetComponent<PotionCollisionHandler>();
            if (collisionHandler != null && collisionHandler.isHeld)
            {
                debugMenu.Log("Potion is held; skipping reset for this object.");
                continue; // Skip reset if potion is held
            }

            // Reset position and rotation
            obj.transform.position = pool.transform.position;
            obj.transform.rotation = Quaternion.identity;

            // Remove any active visual effects
            var vfxComponent = obj.GetComponentInChildren<ParticleSystem>();
            if (vfxComponent != null)
            {
                vfxComponent.Stop();
                vfxComponent.Clear();
            }
        }
    }




    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        int playerIndex = FindAssignedIndex(player.playerId);
        if (playerIndex != -1)
        {
            potionPoolPlayerIds[playerIndex] = -1;
            debugMenu.Log($"Player {player.displayName} left, reset pool index {playerIndex}.");
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
        VRCObjectPool pool = GetPotionPool(playerId, potionType);

        if (pool != null)
        {
            if (!Networking.IsOwner(localPlayer, pool.gameObject))
            {
                Networking.SetOwner(localPlayer, pool.gameObject);
                if (!Networking.IsOwner(localPlayer, pool.gameObject))
                {
                    debugMenu.LogError("Failed to transfer ownership of the pool.");
                    return;
                }
            }

            // Full reset of potion state before returning to pool
            ResetPotionState(potion);

            pool.Return(potion); // Properly return the potion to the pool
            debugMenu.Log("Potion successfully returned to pool with full reset.");
        }
        else
        {
            debugMenu.LogError("DestroyPotion error: Pool reference is null for the provided player and potion type.");
        }
    }


    private void ResetPotionState(GameObject potion)
    {
        // Reset position and rotation
        potion.transform.position = Vector3.zero;
        potion.transform.rotation = Quaternion.identity;

        // Ensure all VFX are deactivated
        var vfxComponent = potion.GetComponentInChildren<ParticleSystem>();
        if (vfxComponent != null)
        {
            vfxComponent.Stop();
            vfxComponent.Clear();
        }

        // Reset destruction state
        var collisionHandler = potion.GetComponent<PotionCollisionHandler>();
        if (collisionHandler != null)
        {
            collisionHandler.isDestroyed = false;
            collisionHandler.RequestSerialization(); // Sync reset state
        }
    }







    #endregion

    #region Assign Potion Pool

    private void AssignPotionPool(VRCPlayerApi player)
    {
        for (int i = 0; i < potionPoolPlayerIds.Length; i++)
        {
            if (potionPoolPlayerIds[i] == -1) // Ensure unassigned slots are available
            {
                potionPoolPlayerIds[i] = player.playerId; // Assign player to this slot
                AssignPlayerPools(player, i);
                debugMenu.Log($"Assigned pool slot {i} to player {player.displayName}");

                // Ensure activation and ownership of WallBreaker pool
                VRCObjectPool wallBreakerPool = GetPlayerPotionPool(player.playerId, "WallBreaker");
                if (wallBreakerPool != null)
                {
                    Networking.SetOwner(player, wallBreakerPool.gameObject); // Set owner
                    wallBreakerPool.gameObject.SetActive(true); // Activate the pool
                }

                // Ensure activation and ownership of SuperJump pool
                VRCObjectPool superJumpPool = GetPlayerPotionPool(player.playerId, "SuperJump");
                if (superJumpPool != null)
                {
                    Networking.SetOwner(player, superJumpPool.gameObject);
                    superJumpPool.gameObject.SetActive(true);
                }

                // Ensure activation and ownership of WaterWalking pool
                VRCObjectPool waterWalkingPool = GetPlayerPotionPool(player.playerId, "WaterWalk");
                if (waterWalkingPool != null)
                {
                    Networking.SetOwner(player, waterWalkingPool.gameObject);
                    waterWalkingPool.gameObject.SetActive(true);
                }

                return;
            }
        }
        debugMenu.LogError("No available pool slot for new player.");
    }




    private void AssignPlayerPools(VRCPlayerApi player, int index)
    {
        if (potionPoolPlayerIds[index] != -1 && potionPoolPlayerIds[index] != player.playerId)
        {
            debugMenu.LogError($"Slot {index} is occupied by another player. Unable to assign to {player.displayName}.");
            return;
        }

        potionPoolPlayerIds[index] = player.playerId;

        if (localPlayer == Networking.LocalPlayer)
        {
            VRCObjectPool wallBreakerPool = GetWallBreakerPotionPool(index);
            if (wallBreakerPool != null && !wallBreakerPool.gameObject.activeSelf)
            {
                Networking.SetOwner(player, wallBreakerPool.gameObject);
                wallBreakerPool.gameObject.SetActive(true);
                wallBreakerPoolVersion++; // Increment version to sync
                debugMenu.Log($"Wall Breaker pool assigned to player {player.displayName} at index {index}.");
            }

            VRCObjectPool superJumpPool = GetSuperJumpPotionPool(index);
            if (superJumpPool != null && !superJumpPool.gameObject.activeSelf)
            {
                Networking.SetOwner(player, superJumpPool.gameObject);
                superJumpPool.gameObject.SetActive(true);
                superJumpPoolVersion++; // Increment version to sync
                debugMenu.Log($"Super Jump pool assigned to player {player.displayName} at index {index}.");
            }

            VRCObjectPool waterWalkingPool = GetWaterWalkingPotionPool(index);
            if (waterWalkingPool != null && !waterWalkingPool.gameObject.activeSelf)
            {
                Networking.SetOwner(player, waterWalkingPool.gameObject);
                waterWalkingPool.gameObject.SetActive(true);
                waterWalkingPoolVersion++; // Increment version to sync
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