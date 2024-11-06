using UdonSharp;
using UnityEngine;
using TMPro;

public class InteractableObjectManager : UdonSharpBehaviour
{
    #region Variables

    #region Inventory Items

    [Space(5)][Header("Inventory Item Amounts")][Space(10)]
    public int HerbsCollected = 0;
    public int FlowersCollected = 0;
    public int GemstonesCollected = 0;
    public int MushroomsCollected = 0;
    public int BerriesCollected = 0;
    public int SticksCollected = 0;
    public int PotionWallBreakingCollected = 0;
    public int PotionWaterWalkingCollected = 0;
    public int PotionSuperJumpingCollected = 0;

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

    #region Potion Crafting Bools

    public bool CraftPotionWallBreaking;
    public bool CraftPotionSuperJumping;
    public bool CraftPotionWaterWalking;

    #endregion

    #endregion

    #region On Start

    private void Start()
    {
        UpdateUI();
    }

    #endregion

    #region Increment Collected Items

    public void IncrementHerbsCollected() { HerbsCollected++; UpdateUI(); }
    public void IncrementFlowersCollected() { FlowersCollected++; UpdateUI(); }
    public void IncrementGemstonesCollected() { GemstonesCollected++; UpdateUI(); }
    public void IncrementMushroomsCollected() { MushroomsCollected++; UpdateUI(); }
    public void IncrementBerriesCollected() { BerriesCollected++; UpdateUI(); }
    public void IncrementSticksCollected() { SticksCollected++; UpdateUI(); }

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
}
