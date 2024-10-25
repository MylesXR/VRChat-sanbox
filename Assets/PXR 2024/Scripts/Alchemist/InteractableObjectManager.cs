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

    #region On Start & Player Join

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        UpdateUI();
        UpdateOwner();
        DeactivateAllPotionPools();

        // Initialize the potionPoolPlayerIds array with a fixed size
        int maxPlayers = 32; // Example max player count; adjust as needed
        potionPoolPlayerIds = new int[maxPlayers];
        for (int i = 0; i < potionPoolPlayerIds.Length; i++)
        {
            potionPoolPlayerIds[i] = -1; // Set unused slots to -1
        }

        debugMenu.Log("All potion pools have been deactivated for local player at scene start.");
    }


    private void DeactivateAllPotionPools()
    {
        foreach (var pool in wallBreakerPotionPool)
        {
            if (pool != null)
            {
                pool.gameObject.SetActive(false);
            }
        }

        foreach (var pool in superJumpPotionPool)
        {
            if (pool != null)
            {
                pool.gameObject.SetActive(false);
            }
        }

        foreach (var pool in waterWalkingPotionPool)
        {
            if (pool != null)
            {
                pool.gameObject.SetActive(false);
            }
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (localPlayer == Networking.LocalPlayer)
        {
            debugMenu.Log($"Player joined: {player.displayName}, ID: {player.playerId}");
            AssignPotionPool(player);
            return;

        }     
    }


    public override void OnOwnershipTransferred(VRCPlayerApi newOwner)
    {
        UpdateOwner();
    }

    private void UpdateOwner()
    {
        isOwner = Networking.IsOwner(gameObject);
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (localPlayer == Networking.LocalPlayer)
        {
            debugMenu.Log($"Player left: {player.displayName}, ID: {player.playerId}");

            // Loop through the array to find and release the leaving player's pools
            for (int i = 0; i < potionPoolPlayerIds.Length; i++)
            {
                if (potionPoolPlayerIds[i] == player.playerId)
                {
                    // Reset the player ID in the array to -1
                    potionPoolPlayerIds[i] = -1;
                    debugMenu.Log($"Freed up potion pool slot {i} for player {player.displayName}.");

                    // Deactivate the pools for the leaving player
                    DeactivatePlayerPotionPools(i, player);
                    break;
                }
            }
        }
    }

    // Helper method to deactivate pools only for the specified player index
    private void DeactivatePlayerPotionPools(int index, VRCPlayerApi player)
    {
        // Get local player for ownership transfer
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        // Handle Wall Breaker potion pool
        VRCObjectPool wallBreakerPool = GetWallBreakerPotionPool(index);
        if (wallBreakerPool != null && wallBreakerPool.gameObject.activeSelf)
        {
            foreach (GameObject obj in wallBreakerPool.Pool)
            {
                if (obj != null)
                {
                    Networking.SetOwner(localPlayer, obj);  // Transfer ownership to the local player
                    obj.SetActive(false);  // Deactivate each object
                }
            }
            Networking.SetOwner(localPlayer, wallBreakerPool.gameObject);
            wallBreakerPool.gameObject.SetActive(false);  // Deactivate the pool
        }

        // Handle Super Jump potion pool
        VRCObjectPool superJumpPool = GetSuperJumpPotionPool(index);
        if (superJumpPool != null && superJumpPool.gameObject.activeSelf)
        {
            foreach (GameObject obj in superJumpPool.Pool)
            {
                if (obj != null)
                {
                    Networking.SetOwner(localPlayer, obj);
                    obj.SetActive(false);
                }
            }
            Networking.SetOwner(localPlayer, superJumpPool.gameObject);
            superJumpPool.gameObject.SetActive(false);
        }

        // Handle Water Walking potion pool
        VRCObjectPool waterWalkingPool = GetWaterWalkingPotionPool(index);
        if (waterWalkingPool != null && waterWalkingPool.gameObject.activeSelf)
        {
            foreach (GameObject obj in waterWalkingPool.Pool)
            {
                if (obj != null)
                {
                    Networking.SetOwner(localPlayer, obj);
                    obj.SetActive(false);
                }
            }
            Networking.SetOwner(localPlayer, waterWalkingPool.gameObject);
            waterWalkingPool.gameObject.SetActive(false);
        }

        debugMenu.Log($"Potion pools deactivated and reset for player {player.displayName}.");
    }



    #endregion

    #region Assign Potion Pool

    private void AssignPotionPool(VRCPlayerApi player)
{
    int maxRetries = wallBreakerPotionPool.Length; // To prevent an infinite loop

    // Keep trying the next index if the pool is already active
    for (int i = 0; i < maxRetries; i++)
    {
        // Calculate the next playerIndex based on playerId and number of pools
        playerIndex = (player.playerId + i) % wallBreakerPotionPool.Length;

        debugMenu.Log($"Trying to assign potion pools for player {player.displayName} with playerIndex {playerIndex}");

        // Check if the player's index exceeds the number of available pools
        if (playerIndex >= wallBreakerPotionPool.Length || playerIndex >= superJumpPotionPool.Length || playerIndex >= waterWalkingPotionPool.Length)
        {
            debugMenu.LogError($"Not enough potion pools for player {player.displayName}.");
            return;  // No pools available, stop here
        }

        // Check if the pool is already in use by another player
        VRCObjectPool wallBreakerPool = GetWallBreakerPotionPool(playerIndex);
        if (wallBreakerPool != null && !wallBreakerPool.gameObject.activeSelf)
        {
            // Assign the pools to the player
            AssignPlayerPools(player, playerIndex);
            return;  // Exit the loop once a valid pool is found
        }
    }

    debugMenu.LogError($"All potion pools are currently in use. Could not assign a pool to player {player.displayName}.");
}

    private void AssignPlayerPools(VRCPlayerApi player, int index)
    {
        // Assign playerId to potionPoolPlayerIds at playerIndex
        if (index < potionPoolPlayerIds.Length)
        {
            potionPoolPlayerIds[index] = player.playerId; // Store the player's ID
            debugMenu.Log($"Player {player.displayName} assigned to potion pool slot {index}.");
        }
        else
        {
            debugMenu.LogError("Player index exceeds the fixed potionPoolPlayerIds array length.");
            return;
        }

        // Assign and activate Wall Breaker potion pool
        VRCObjectPool wallBreakerPool = GetWallBreakerPotionPool(index);
        if (wallBreakerPool != null && !wallBreakerPool.gameObject.activeSelf)
        {
            wallBreakerPool.gameObject.SetActive(true);
            debugMenu.Log($"Activated Wall Breaker potion pool for player {player.displayName}.");
        }

        // Assign and activate Super Jump potion pool
        VRCObjectPool superJumpPool = GetSuperJumpPotionPool(index);
        if (superJumpPool != null && !superJumpPool.gameObject.activeSelf)
        {
            superJumpPool.gameObject.SetActive(true);
            debugMenu.Log($"Activated Super Jump potion pool for player {player.displayName}.");
        }

        // Assign and activate Water Walking potion pool
        VRCObjectPool waterWalkingPool = GetWaterWalkingPotionPool(index);
        if (waterWalkingPool != null && !waterWalkingPool.gameObject.activeSelf)
        {
            waterWalkingPool.gameObject.SetActive(true);
            debugMenu.Log($"Activated Water Walking potion pool for player {player.displayName}.");
        }
    }







    #endregion

    #region Get Potion Pool ID

    public VRCObjectPool GetPlayerPotionPool(int playerId, string potionType)
    {
         playerIndex = playerId;

        if (playerIndex >= wallBreakerPotionPool.Length)
        {
            debugMenu.LogError("Player index exceeds potion pool array length.");
            return null;
        }

        switch (potionType)
        {
            case "SuperJump":
                return superJumpPotionPool[playerIndex];
            case "WaterWalk":
                return waterWalkingPotionPool[playerIndex];
            case "WallBreaker":
                return wallBreakerPotionPool[playerIndex];
            default:
                return null;
        }
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