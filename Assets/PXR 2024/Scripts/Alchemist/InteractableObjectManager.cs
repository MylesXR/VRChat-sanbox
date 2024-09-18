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
    public int MushroomsCollected = 10;
    public int BerriesCollected = 10;
    public int SticksCollected = 10;
    public int PotionWallBreakingCollected = 10;
    public int PotionWaterWalkingCollected = 10;
    public int PotionSuperJumpingCollected = 10;
    public bool CraftPotionWallBreaking;
    public bool CraftPotionSuperJumping;
    public bool CraftPotionWaterWalking;

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

    [Space(5)][Header("Potions")][Space(10)]
    //[SerializeField] private VRCObjectPool[] playerPotionPools; // Set this in the Unity Inspector
    [SerializeField] private VRCObjectPool[] wallBreakerPotionPool;
    [SerializeField] private VRCObjectPool[] superJumpPotionPool;
    [SerializeField] private VRCObjectPool[] waterWalkingPotionPool;


    private int maxPlayers = 100;
    public GameObject BreakableObject;
    public GameObject WaterWalkingObject;


    #region Debugging

    [Space(5)][Header("Debug Text")][Space(10)]
    [SerializeField] private DebugMenu debugMenu;
    private VRCPlayerApi localPlayer;

    #endregion

    #endregion

    public VRCObjectPool GetWallBreakerPotionPool(int playerIndex)
    {
        return wallBreakerPotionPool[playerIndex];
    }

    public VRCObjectPool GetSuperJumpPotionPool(int playerIndex)
    {
        return superJumpPotionPool[playerIndex];
    }

    public VRCObjectPool GetWaterWalkingPotionPool(int playerIndex)
    {
        return waterWalkingPotionPool[playerIndex];
    }

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

        if (playerIndex >= wallBreakerPotionPool.Length)
        {
            debugMenu.LogError("Player index exceeds potion pool array length.");
            return;
        }

        // Assign Wall Breaker Potion Pool
        VRCObjectPool wallBreakerPool = GetWallBreakerPotionPool(playerIndex);
        if (wallBreakerPool != null)
        {
            Networking.SetOwner(player, wallBreakerPool.gameObject);
            debugMenu.Log($"Assigned Wall Breaker potion pool to player {player.displayName}.");
        }

        // Assign Super Jump Potion Pool
        VRCObjectPool superJumpPool = GetSuperJumpPotionPool(playerIndex);
        if (superJumpPool != null)
        {
            Networking.SetOwner(player, superJumpPool.gameObject);
            debugMenu.Log($"Assigned Super Jump potion pool to player {player.displayName}.");
        }

        // Assign Water Walking Potion Pool
        VRCObjectPool waterWalkingPool = GetWaterWalkingPotionPool(playerIndex);
        if (waterWalkingPool != null)
        {
            Networking.SetOwner(player, waterWalkingPool.gameObject);
            debugMenu.Log($"Assigned Water Walking potion pool to player {player.displayName}.");
        }
    }

    public VRCObjectPool GetPlayerPotionPool(int playerId, string potionType)
    {
        int playerIndex = playerId % maxPlayers;

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
                debugMenu.LogError("Invalid potion type.");
                return null;
        }
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

    public GameObject GetObjectToDestroy()
    {
        debugMenu.Log("Returning breakable object.");
        return BreakableObject;
    }

    public GameObject GetObjectToActivate()
    {
        debugMenu.Log("setting object active perhaps");
        return WaterWalkingObject;
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
            debugMenu.Log("Potion Wall Breaker crafted.");
        }
        else
        {
            CraftPotionWallBreaking = false;
            debugMenu.Log("Not enough resources to craft the Potion Wall Breaker.");
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
            debugMenu.Log("Potion Super Jump crafted.");
        }
        else
        {
            CraftPotionSuperJumping = false;
            debugMenu.Log("Not enough resources to craft Potion Super Jump.");
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
            debugMenu.Log("Potion Water Walk crafted.");
        }
        else
        {
            CraftPotionWaterWalking = false;
            debugMenu.Log("Not enough resources to craft Potion Water Walk.");
        }
    }

    #endregion
}