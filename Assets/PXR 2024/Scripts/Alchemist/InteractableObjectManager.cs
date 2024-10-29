using UdonSharp;
using UnityEngine;
using TMPro;
using VRC.SDKBase;
using VRC.SDK3.Components;

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

    // Track potion active states by player index
    private bool[] isPotionActive = new bool[100]; // Assuming max 100 players


    #region On Player Join and Leave

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (localPlayer == Networking.LocalPlayer)
        {
            int assignedIndex = FindAssignedIndex(player.playerId);

            // Destroy previous potion pool if assigned
            if (assignedIndex != -1)
            {
                ResetAndDeactivatePools(assignedIndex);
                potionPoolPlayerIds[assignedIndex] = -1; // Mark as unassigned
            }

            // Assign new potion pool for the player
            DisablePotionsLocally();
            DeactivateInactivePotionsForNewPlayer(player);
            AssignPotionPool(player);
            debugMenu.Log($"Player {player.displayName} joined, reset and assigned new potion pool.");
        }
    }

    private void DisablePotionsLocally()
    {
        VRCObjectPool[] allPotionPools = new VRCObjectPool[] {
            GetWallBreakerPotionPool(0), GetSuperJumpPotionPool(0), GetWaterWalkingPotionPool(0),
            GetWallBreakerPotionPool(1), GetSuperJumpPotionPool(1), GetWaterWalkingPotionPool(1),
            // Add additional entries as needed for each pool index
        };

        foreach (var pool in allPotionPools)
        {
            if (pool != null)
            {
                foreach (var potion in pool.Pool)
                {
                    if (potion != null && potion.activeSelf)
                    {
                        potion.SetActive(false); // Disable potion locally only
                    }
                }
            }
        }
    }


    // Method to deactivate only inactive potions for newly joined player
    private void DeactivateInactivePotionsForNewPlayer(VRCPlayerApi newPlayer)
    {
        foreach (var pool in new[] { wallBreakerPotionPool, superJumpPotionPool, waterWalkingPotionPool })
        {
            foreach (var potionPool in pool)
            {
                foreach (var potion in potionPool.Pool)
                {
                    // Set inactive potions to be invisible for new player
                    if (!potion.activeSelf)
                    {
                        Networking.SetOwner(newPlayer, potion);
                        potion.SetActive(false);
                        DestroyPotion(potion);
                    }
                }
            }
        }
    }

    public void DestroyPotion(GameObject potion)
    {
        // Get the PotionCollisionHandler from the potion
        PotionCollisionHandler potionHandler = potion.GetComponent<PotionCollisionHandler>();
        if (potionHandler != null)
        {
            potionHandler.TriggerVFXandDestroy();  // Calls both VFX and destruction
        }
        else
        {
            debugMenu.Log("PotionCollisionHandler not found on the potion.");
        }
    }




    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (localPlayer == Networking.LocalPlayer)
        {
            int playerIndex = FindAssignedIndex(player.playerId);

            if (playerIndex != -1)
            {
                potionPoolPlayerIds[playerIndex] = -1;
                ResetAndDeactivatePools(playerIndex);
                debugMenu.Log($"Player {player.displayName} left, cleared pool index {playerIndex}");
            }
            else
            {
                debugMenu.Log($"Player {player.displayName} left but had no assigned pool index.");
            }
        }
    }

    #endregion

    #region Potion Shit

    private void ResetAndDeactivatePools(int index)
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

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
            if (potionPoolPlayerIds[i] == -1)
            {
                potionPoolPlayerIds[i] = player.playerId;
                AssignPlayerPools(player, i);
                debugMenu.Log($"Assigned pool slot {i} to player {player.displayName}");
                return;
            }
        }
    }






    private void AssignPlayerPools(VRCPlayerApi player, int index)
    {
        // Ensure that slot assignment is valid for the player
        if (potionPoolPlayerIds[index] != -1 && potionPoolPlayerIds[index] != player.playerId)
        {
            debugMenu.LogError($"Slot {index} is occupied by another player. Unable to assign to {player.displayName}.");
            return;
        }

        // Update the player ID in the tracking array
        potionPoolPlayerIds[index] = player.playerId;

        // Transfer ownership of each object in the Wall Breaker pool
        VRCObjectPool wallBreakerPool = GetWallBreakerPotionPool(index);
        if (wallBreakerPool != null && !wallBreakerPool.gameObject.activeSelf)
        {
            Networking.SetOwner(player, wallBreakerPool.gameObject);
            wallBreakerPool.gameObject.SetActive(true);
            debugMenu.Log($"Wall Breaker pool assigned to player {player.displayName} at index {index}.");
        }

        // Transfer ownership of each object in the Super Jump pool
        VRCObjectPool superJumpPool = GetSuperJumpPotionPool(index);
        if (superJumpPool != null && !superJumpPool.gameObject.activeSelf)
        {
            Networking.SetOwner(player, superJumpPool.gameObject);
            superJumpPool.gameObject.SetActive(true);
            debugMenu.Log($"Super Jump pool assigned to player {player.displayName} at index {index}.");
        }

        // Transfer ownership of each object in the Water Walking pool
        VRCObjectPool waterWalkingPool = GetWaterWalkingPotionPool(index);
        if (waterWalkingPool != null && !waterWalkingPool.gameObject.activeSelf)
        {
            Networking.SetOwner(player, waterWalkingPool.gameObject);
            waterWalkingPool.gameObject.SetActive(true);
            debugMenu.Log($"Water Walking pool assigned to player {player.displayName} at index {index}.");
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

    #region Return Potion Pool

    public void ReturnPotionPool(VRCPlayerApi player)
    {
        playerIndex = player.playerId % wallBreakerPotionPool.Length;

        if (playerIndex >= wallBreakerPotionPool.Length || playerIndex >= superJumpPotionPool.Length || playerIndex >= waterWalkingPotionPool.Length)
        {
            debugMenu.LogError("Player index exceeds potion pool array length.");
            return;
        }

        // Get local player for ownership transfer
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        // Handle Wall Breaker potion pool
        VRCObjectPool wallBreakerPool = GetWallBreakerPotionPool(playerIndex);
        if (wallBreakerPool != null && wallBreakerPool.gameObject.activeSelf)
        {
            // Transfer ownership of each object in the pool to the local player
            foreach (GameObject obj in wallBreakerPool.Pool)
            {
                if (obj != null)
                {
                    Networking.SetOwner(localPlayer, obj);  // Transfer ownership to the local player
                    obj.SetActive(false);  // Deactivate each object
                }
            }

            // Transfer ownership of the pool itself to the local player and deactivate it
            Networking.SetOwner(localPlayer, wallBreakerPool.gameObject);
            wallBreakerPool.gameObject.SetActive(false);  // Deactivate the pool for reuse
            debugMenu.Log($"Returned and reset Wall Breaker potion pool for player {player.displayName}.");
        }

        // Handle Super Jump potion pool
        VRCObjectPool superJumpPool = GetSuperJumpPotionPool(playerIndex);
        if (superJumpPool != null && superJumpPool.gameObject.activeSelf)
        {
            foreach (GameObject obj in superJumpPool.Pool)
            {
                if (obj != null)
                {
                    Networking.SetOwner(localPlayer, obj);  // Transfer ownership to the local player
                    obj.SetActive(false);  // Deactivate each object
                }
            }

            Networking.SetOwner(localPlayer, superJumpPool.gameObject);  // Transfer ownership of the pool itself
            superJumpPool.gameObject.SetActive(false);  // Deactivate the pool for reuse
            debugMenu.Log($"Returned and reset Super Jump potion pool for player {player.displayName}.");
        }

        // Handle Water Walking potion pool
        VRCObjectPool waterWalkingPool = GetWaterWalkingPotionPool(playerIndex);
        if (waterWalkingPool != null && waterWalkingPool.gameObject.activeSelf)
        {
            foreach (GameObject obj in waterWalkingPool.Pool)
            {
                if (obj != null)
                {
                    Networking.SetOwner(localPlayer, obj);  // Transfer ownership to the local player
                    obj.SetActive(false);  // Deactivate each object
                }
            }

            Networking.SetOwner(localPlayer, waterWalkingPool.gameObject);  // Transfer ownership of the pool itself
            waterWalkingPool.gameObject.SetActive(false);  // Deactivate the pool for reuse
            debugMenu.Log($"Returned and reset Water Walking potion pool for player {player.displayName}.");
        }

        debugMenu.Log("Potion pools have been fully returned, reset, and are now available for reuse.");
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
            //debugMenu.Log("Potion Wall Breaker crafted.");
        }
        else
        {
            CraftPotionWallBreaking = false;
            //debugMenu.Log("Not enough resources to craft the Potion Wall Breaker.");
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
            //debugMenu.Log("Potion Super Jump crafted.");
        }
        else
        {
            CraftPotionSuperJumping = false;
            //debugMenu.Log("Not enough resources to craft Potion Super Jump.");
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
            //debugMenu.Log("Potion Water Walk crafted.");
        }
        else
        {
            CraftPotionWaterWalking = false;
            //debugMenu.Log("Not enough resources to craft Potion Water Walk.");
        }
    }

    #endregion
}