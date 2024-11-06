using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon.Common;

public class Bobys_WorldPortalSystem : UdonSharpBehaviour
{
    //Start of added code for Attendee Menu

    #region Variables

    #region Class Settings

    [Header("Class Settings")][Space(10)]
    public string ClassType;
    public GameObject AlchemistMenu;
    public GameObject BarbarianMenu;
    public GameObject ExplorerMenu;
    public GameObject NoClassMenu;

    #endregion

    #region UI Popup Messages

    [Space(5)][Header("UI Popup Messages")][Space(10)]
    [SerializeField] GameObject PopUpMessageCrafting;
    [SerializeField] GameObject PopUpMessageSpawning;
    [SerializeField] GameObject PopUpMessagePotionAlreadySpawned;

    #endregion

    #region Interactable Items

    [Space(5)][Header("Interactable Items")][Space(10)]
    [SerializeField] Transform PotionsSpawnPoint;
    [SerializeField] InteractableObjectManager IOM;

    [Header("Potion Objects (Set in Inspector)")]
    [SerializeField] private GameObject WallBreakingPotion;
    [SerializeField] private GameObject SuperJumpingPotion;
    [SerializeField] private GameObject WaterWalkingPotion;

    #endregion

    #endregion

    #region Set Class 

    public void AlchemistClass() { ClassType = "Alchemist"; }
    public void BarbarianClass() { ClassType = "Barbarian"; }
    public void ExplorerClass() { ClassType = "Explorer"; }

    #endregion  

    #region Wall Breaking Potion 

    public void CraftWallBreakingPotion()
    {
        IOM.CanCraftPotionWallBreaking();
        if (IOM.CraftPotionWallBreaking)
        {
            IOM.PotionWallBreakingCollected++;
            IOM.UpdateUI();
        }
        else
        {
            PopUpMessageCrafting.SetActive(true);
            SendCustomEventDelayedSeconds(nameof(HidePopupMessage), 6f);
        }
    }

    public void SpawnWallBreakingPotion()
    {
        if (WallBreakingPotion.activeSelf)
        {
            PopUpMessagePotionAlreadySpawned.SetActive(true);
            SendCustomEventDelayedSeconds(nameof(HidePopupMessage), 3f);
            return;
        }

        if (IOM.PotionWallBreakingCollected > 0)
        {
            SetPotionTransform(WallBreakingPotion);
            WallBreakingPotion.SetActive(true);
            IOM.PotionWallBreakingCollected--;
            IOM.UpdateUI();
        }
        else
        {
            PopUpMessageSpawning.SetActive(true);
            SendCustomEventDelayedSeconds(nameof(HidePopupMessage), 3f);
        }
    }

    #endregion

    #region Super Jumping Potion

    public void CraftSuperJumpingPotion()
    {
        IOM.CanCraftPotionSuperJumping();
        if (IOM.CraftPotionSuperJumping)
        {
            IOM.PotionSuperJumpingCollected++;
            IOM.UpdateUI();
        }
        else
        {
            PopUpMessageCrafting.SetActive(true);
            SendCustomEventDelayedSeconds(nameof(HidePopupMessage), 6f);
        }
    }

    public void SpawnSuperJumpingPotion()
    {
        if (SuperJumpingPotion.activeSelf)
        {
            PopUpMessagePotionAlreadySpawned.SetActive(true);
            SendCustomEventDelayedSeconds(nameof(HidePopupMessage), 3f);
            return;
        }

        if (IOM.PotionSuperJumpingCollected > 0)
        {
            SetPotionTransform(SuperJumpingPotion);
            SuperJumpingPotion.SetActive(true);
            IOM.PotionSuperJumpingCollected--;
            IOM.UpdateUI();
        }
        else
        {
            PopUpMessageSpawning.SetActive(true);
            SendCustomEventDelayedSeconds(nameof(HidePopupMessage), 3f);
        }
    }

    #endregion

    #region Water Walking Potion

    public void CraftWaterWalkingPotion()
    {
        IOM.CanCraftPotionWaterWalking();
        if (IOM.CraftPotionWaterWalking)
        {
            IOM.PotionWaterWalkingCollected++;
            IOM.UpdateUI();
        }
        else
        {
            PopUpMessageCrafting.SetActive(true);
            SendCustomEventDelayedSeconds(nameof(HidePopupMessage), 6f);
        }
    }

    public void SpawnWaterWalkingPotion()
    {
        if (WaterWalkingPotion.activeSelf)
        {
            PopUpMessagePotionAlreadySpawned.SetActive(true);
            SendCustomEventDelayedSeconds(nameof(HidePopupMessage), 3f);
            return;
        }

        if (IOM.PotionWaterWalkingCollected > 0)
        {
            SetPotionTransform(WaterWalkingPotion);
            WaterWalkingPotion.SetActive(true);
            IOM.PotionWaterWalkingCollected--;
            IOM.UpdateUI();
        }
        else
        {
            PopUpMessageSpawning.SetActive(true);
            SendCustomEventDelayedSeconds(nameof(HidePopupMessage), 3f);
        }
    }

    #endregion

    #region Set Potion Transform

    private void SetPotionTransform(GameObject potion)
    {
        if (PotionsSpawnPoint == null) return;

        // Reset the potion's position, rotation, and make it kinematic to avoid falling on spawn
        potion.transform.position = PotionsSpawnPoint.position;
        potion.transform.rotation = PotionsSpawnPoint.rotation;

        // Set the Rigidbody to kinematic if it exists
        Rigidbody potionRigidbody = potion.GetComponent<Rigidbody>();
        if (potionRigidbody != null)
        {
            potionRigidbody.isKinematic = true;
            potionRigidbody.useGravity = false;
        }
    }

    #endregion

    #region UI Update

    public void HidePopupMessage()
    {
        PopUpMessageCrafting.SetActive(false);
        PopUpMessageSpawning.SetActive(false);
        PopUpMessagePotionAlreadySpawned.SetActive(false);
    }

    #endregion

    // End of added code for Attendee Menu

    #region Core
    public Transform[] Spawns;
    [Space]
    public VRC_Pickup Menu;
    public BoxCollider MenuPickupCollider;
    public RectTransform MenuPickupUIHandle;
    #endregion

    #region Settings
    [Header("Settings")]
    public Transform[] Locations;
    [Space]
    public Sprite[] LocationPreviews;
    public bool PlayerView;
    [Space]
    public Camera PreviewCamera;
    public Material PreviewCamMat;
    public LayerMask PreviewCameraLocationMask = 3926295;
    public LayerMask PreviewCameraPlayerMask = 4188951;
    [Space]
    Transform PreviewRay;
    public RectTransform PreviewRayCanvas;
    public Image PreviewRayImage;
    [Space]
    public SphereCollider PlayerSphereTrigger;
    #endregion

    #region Portal Settings
    [Header("Portals")]
    public WPS_Portal[] Portals;
    [Space]
    public GameObject PortalHolder;
    [Space]
    int UsingPortal;
    [Space]
    public Transform PreviewPortal;
    public GameObject UnavaliablePreviewPortal;
    public GameObject AvaliablePreviewPortal;
    [Space]
    public Transform PreviewPortalUI;
    public Text PreviewPortalText;
    [Space]
    public LayerMask GroundLayers = 1;
    #endregion

    #region UI Settings
    [Header("UI")]
    public Transform StartupInfo;
    public Text StartupInfoText;
    [Space]
    public Transform[] OptionWindows;
    public Button LocationsButton;
    [Space]
    public Toggle AllowPlayerTeleportToggle;
    public Toggle UsePortalsToggle;
    [Space]
    public Slider SummonModeSlider;
    public Text SummonModeDescription;
    [Space]
    public Slider MenuPickupSide;
    public Text MenuPickupSideText;
    [Space]
    public Slider MenuPickupSensitivity; 
    public Text MenuPickupSensitivityText;
    [Space]
    Toggle[] HeaderToggles;
    GameObject[] OptionBodies;
    Toggle[] TeleportButtons;
    Toggle[] PortalButtons;
    #endregion

    #region Local Player Settings
    [Header("Player Settings")]
    public int SummonMode;
    #endregion

    [Header("Networking")]
    public WPS_NetworkManager Net;

    #region Local Vars
    int InputMenu; 
    float InputMenuTimer;
    int _tempInt_1;

    Vector2 InputMove;
    int InputTriggerL; int InputTriggerR;
    int InputTriggerLCount; int InputTriggerRCount;
    Vector2Int TriggerPressCount; 
    float DoubleTapTriggerTimer;
    
    bool RightHandFocus = true;
    int LastOpened = -1;

    bool PlacingPortal; int LocPlaID; bool AvaliableToPlace; float TimeTillAllowed; float PlayerInAreaTimer;
    [HideInInspector] public WPS_PlayerData PlayerData;
    #endregion

    public void Start()
    {

        #region Check for Missing References
        if (Menu == null || OptionWindows.Length == 0 || Net == null)
        {
            Debug.LogError("<b>[<color=yellow>Boby's World Portal System</color>]</b> Either the Menu, OptionWindows, and/or WPS_Networking aren't provided!");
            enabled = false; return;
        }
        if (PreviewCamera == null || PreviewCamMat == null)
        {
            Debug.LogError("<b>[<color=yellow>Boby's World Portal System</color>]</b> Either the Preview Camera and/or Preview Camera Material aren't provided!");
            enabled = false; return;
        }
        if (Spawns.Length == 0)
        { 
            Debug.LogError("<b>[<color=yellow>Boby's World Portal System</color>]</b> Please provide the spawn transforms in your world!");
            enabled = false; return;
        }
        for (int s = 0; s < Spawns.Length; s++)
        {
            if (Spawns[s] == null)
            {
                Debug.LogError("<b>[<color=yellow>Boby's World Portal System</color>]</b> One of the provided Spawn transforms is null!");
                enabled = false; return;
            }
        }
        #endregion

        #region Generate References to UI Menu
        HeaderToggles = new Toggle[OptionWindows.Length];
        OptionBodies = new GameObject[OptionWindows.Length];
        TeleportButtons = new Toggle[OptionWindows.Length];
        PortalButtons = new Toggle[OptionWindows.Length];
        for (int i = 0; i < OptionWindows.Length; i++)
        {
            HeaderToggles[i] = OptionWindows[i].GetChild(0).GetChild(0).GetComponent<Toggle>();
            OptionBodies[i] = OptionWindows[i].GetChild(1).gameObject;
            TeleportButtons[i] = OptionWindows[i].GetChild(1).GetChild(2).GetChild(0).GetComponent<Toggle>();
            PortalButtons[i] = OptionWindows[i].GetChild(1).GetChild(2).GetChild(1).GetComponent<Toggle>();
        }
        #endregion

        if (PreviewRayCanvas != null)
        { PreviewRay = PreviewRayCanvas.transform.parent; }

        SendCustomEventDelayedSeconds(nameof(_AvatarSizeCheck), 10, VRC.Udon.Common.Enums.EventTiming.LateUpdate);
        _SwitchToLocationView();
        _CheckMenuSummonMode();
        _CheckPickupMenuSide();
        _CheckPickupMenuSensitivity();
        _HidePortalMenu();

        #region Setup Startup Info
        if (StartupInfoText != null)
        {
            string SummonText =
                !Networking.LocalPlayer.IsUserInVR() ? "Press <i><b>Tab</b></i>" :
                SummonMode == 0 ? "Press <i><b>Both Triggers</b></i>" :
                SummonMode == 1 ? "Double Tap a <i><b>Trigger</b></i>" :
                SummonMode == 2 ? $"Grab from your <i><b>{(MenuPickupSide.value == 1 ? "Right" : "Left")} Shoulder</b></i>" :
                SummonMode == 3 ? $"Grab from the <i><b>{(MenuPickupSide.value == 1 ? "Right" : "Left")} Side of your Hip</b></i>" :
                "Press <b>UNKNOWN SUMMON MODE!<b> <i>Please contact the creator!</i>";

            StartupInfoText.text = SummonText;
        }
        #endregion
    }

    void LateUpdate()
    {
        #region Startup Info Positioning
        if (StartupInfo != null)
        {
            if (StartupInfo.gameObject.activeSelf)
            {
                VRCPlayerApi.TrackingData HeadTrack = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

                StartupInfo.SetPositionAndRotation(HeadTrack.position + HeadTrack.rotation * new Vector3(0, -.3f, 1), 
                    HeadTrack.rotation);
            }
        }
        #endregion

        #region Menu Input
        #region Get Trigger Input
        if (InputTriggerData[0] || InputTriggerData[1])
        {
            if (InputTriggerData[0])
            { 
                if (InputTriggerL < 2)
                { InputTriggerL += 1; }
            }
            if (InputTriggerData[1])
            { 
                if (InputTriggerR < 2)
                { InputTriggerR += 1; }
            }
        }
        else
        {
            if (!InputTriggerData[0])
            { InputTriggerL = 0; }
            if (!InputTriggerData[1])
            { InputTriggerR = 0; }
        }
        #endregion

        #region Detect Double Tap Trigger
        if (SummonMode == 1)
        {
            DoubleTapTriggerTimer = DoubleTapTriggerTimer > 0 ? DoubleTapTriggerTimer - Time.deltaTime : 0;
            if (DoubleTapTriggerTimer == 0)
            { InputTriggerLCount = 0; InputTriggerRCount = 0; }

            if (InputTriggerL == 1)
            {
                InputTriggerRCount = 0;
                if (InputTriggerLCount < 1)
                { DoubleTapTriggerTimer = .5f; }

                InputTriggerLCount = (InputTriggerLCount + 1) % 3;
            }
            if (InputTriggerR == 1)
            {
                InputTriggerLCount = 0;
                if (InputTriggerRCount < 1)
                { DoubleTapTriggerTimer = .5f; }

                InputTriggerRCount = (InputTriggerRCount + 1) % 3;
            }
        }
        #endregion

        bool VRCheckSummon =
            SummonMode == 0 ? InputTriggerL > 0 & InputTriggerR > 0 :
            SummonMode == 1 ? InputTriggerLCount == 2 || InputTriggerRCount == 2 :
            SummonMode == 2 || SummonMode == 3 ? Menu.IsHeld :
            false;
        
        if (Input.GetKeyDown(KeyCode.Tab) || VRCheckSummon)
            //Changed from here to the next comment

        { InputMenu = InputMenu < 2 ? InputMenu + 1 : InputMenu; }
            //End of changes
        else
        { InputMenu = 0; }
        #endregion
        
        #region Menu Summon and Hide
        if (InputMove.magnitude >= .35f & !Menu.IsHeld & Menu.transform.GetChild(0).gameObject.activeInHierarchy)
        { _HidePortalMenu(); }

        #region Go to Sholder/Hip
        if (SummonMode >= 2 & !Menu.IsHeld & !Menu.transform.GetChild(0).gameObject.activeInHierarchy & Networking.LocalPlayer.IsUserInVR())
        {
            Quaternion PlayerRot = Networking.LocalPlayer.GetRotation();
            if (SummonMode == 2)
            {
                VRCPlayerApi.TrackingData HeadTrack = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
                
                Menu.transform.SetPositionAndRotation(HeadTrack.position + HeadTrack.rotation * new Vector3(.25f * (MenuPickupSide.value == 0 ? -1 : 1), -.05f) * playerSize / 1.75f,
                    Quaternion.LookRotation(HeadTrack.rotation * Vector3.up, HeadTrack.rotation * Vector3.back));
            }
            else if (SummonMode == 3)
            {
                Vector3 HipPos = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.Hips);
                if (HipPos == Vector3.zero)
                { HipPos = Networking.LocalPlayer.GetPosition() + Vector3.up * .5f * playerSize / 1.75f; }

                Menu.transform.SetPositionAndRotation(HipPos + PlayerRot * Vector3.right * .25f * (MenuPickupSide.value == 0 ? -1 : 1) * playerSize / 1.75f,
                    Quaternion.LookRotation(PlayerRot * Vector3.down, PlayerRot * Vector3.forward));
            }

            VRCPlayerApi.TrackingData HandLTack = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand);
            VRCPlayerApi.TrackingData HandRTack = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand);
            Vector3 MenuPickupPos = Menu.transform.position;

            float SensitiveDistance = .1f * MenuPickupSensitivity.value;
            Menu.pickupable = Vector3.Distance(MenuPickupPos, HandLTack.position) <= SensitiveDistance || Vector3.Distance(MenuPickupPos, HandRTack.position) <= SensitiveDistance
                || !Networking.LocalPlayer.IsUserInVR();
        }
        #endregion

        if ((InputMenu == 1 || Menu.IsHeld) & !Menu.transform.GetChild(0).gameObject.activeInHierarchy)
        { _SummonPortalMenu(); }
        #endregion
        //Commented out the code below to reduce update, the code below
        //handles the portal spawning etc so it is not needed for the attendee menu
        /*
        #region Check and Update Using Portal ID
        if (UsingPortal >= 0 & UsingPortal < Portals.Length)
        {
            if (Portals[UsingPortal].LifeTimer < 0 & !PlacingPortal)
            { UsingPortal = -1; }
        }
        #endregion

        #region Turn off Player Check Trigger & Preview Ray when not Placing a Portal
        if (!PlacingPortal)
        {
            PlayerSphereTrigger.enabled = false;
            if (PreviewRayCanvas != null)
            { PreviewRayCanvas.gameObject.SetActive(false); }
            return; 
        }
        #endregion

        #region Placing Portal
        if (InputMenu > 0 || (RightHandFocus && InputTriggerL > 0 || !RightHandFocus && InputTriggerR > 0))
        { PlacingPortal = false; PreviewPortal.gameObject.SetActive(false); InputMenu = 0; InputMenuTimer = 0; return; }

        TimeTillAllowed = TimeTillAllowed > 0 ? TimeTillAllowed - Time.deltaTime : 0;

        Vector3 Dist = Vector3.forward; AvaliableToPlace = false; float RayDistance = 100;
        string CancelMessage = "\n<b>Tab</b> to cancel.";
        #region VR
        if (Networking.LocalPlayer.IsUserInVR())
        {
            CancelMessage = $"\nPress <b>{(!RightHandFocus ? "Right" : "Left")} Trigger</b> to cancel.";

            PreviewPortalText.text = $"<b>{(RightHandFocus ? "Right" : "Left")} Trigger</b> to place Portal." + CancelMessage;
            VRCPlayerApi.TrackingDataType currentHand = RightHandFocus ? VRCPlayerApi.TrackingDataType.RightHand : VRCPlayerApi.TrackingDataType.LeftHand;
            VRCPlayerApi.TrackingData HandTrack = Networking.LocalPlayer.GetTrackingData(currentHand);
            Vector3 HandDir = HandTrack.rotation * new Vector3(1, 0, 1).normalized;

            RaycastHit hit; 
            if (Physics.Raycast(HandTrack.position, HandDir, out hit, 100, GroundLayers, QueryTriggerInteraction.Ignore))
            {
                RayDistance = hit.distance;
                if (Physics.Raycast(hit.point + Vector3.up - (HandDir - Vector3.up * HandDir.y).normalized * .1f, Vector3.down, out hit, 20, GroundLayers, QueryTriggerInteraction.Ignore))
                {
                    if (Vector3.Distance(hit.point, Networking.LocalPlayer.GetPosition()) <= 10)
                    { 
                        PreviewPortal.position = hit.point;
                        PreviewPortal.up = hit.normal;

                        if (Vector3.Dot(Vector3.up, hit.normal) < .75f)
                        { PreviewPortalText.text = $"<i>Too <b>Steep</b>!</i>" + CancelMessage; }
                        else
                        { AvaliableToPlace = true; }
                    }
                    else
                    { PreviewPortalText.text = $"<i>Too <b>Far Away</b>!</i>" + CancelMessage; }
                }
                else
                { PreviewPortalText.text = $"<i><b>No Ground</b> to Place Portal!</i>" + CancelMessage; }
            }
            else
            { PreviewPortalText.text = $"<i><b>No Ground</b> to Place Portal!</i>" + CancelMessage; }

            #region Preview Ray (VR)
            if (PreviewRayCanvas != null & PreviewRay != null)
            { 
                PreviewRayCanvas.gameObject.SetActive(true);
                PreviewRay.position = HandTrack.position; PreviewRay.forward = HandDir;
                PreviewRayCanvas.sizeDelta = new Vector2(1000 * RayDistance, 100);
            }
            else if (PreviewRayCanvas != null)
            { PreviewRayCanvas.gameObject.SetActive(false); }
            #endregion
        }
        #endregion
        #region Desktop
        else
        {
            PreviewPortalText.text = "<b>Left Click</b> to place Portal.\n<b>Tab</b> to cancel.";
            VRCPlayerApi.TrackingData HeadTrack = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            Vector3 HeadDir = HeadTrack.rotation * Vector3.forward;
            
            RaycastHit hit;
            if (Physics.Raycast(HeadTrack.position, HeadDir, out hit, 100, GroundLayers, QueryTriggerInteraction.Ignore))
            { 
                RayDistance = hit.distance;
                if (Physics.Raycast(hit.point + Vector3.up - (HeadDir - Vector3.up * HeadDir.y).normalized * .1f, Vector3.down, out hit, 20, GroundLayers, QueryTriggerInteraction.Ignore))
                {
                    if (Vector3.Distance(hit.point, Networking.LocalPlayer.GetPosition()) <= 10)
                    { 
                        PreviewPortal.position = hit.point;
                        PreviewPortal.up = hit.normal;
                        
                        if (Vector3.Dot(Vector3.up, hit.normal) < .75f)
                        { PreviewPortalText.text = $"<i>Too <b>Steep</b>!</i>\n<b>Tab</b> to cancel."; }
                        else
                        { AvaliableToPlace = true; }
                    }
                    else
                    { PreviewPortalText.text = $"<i>Too <b>Far Away</b>!</i>\n<b>Tab</b> to cancel."; }
                }
                else
                { PreviewPortalText.text = $"<i><b>No Ground</b> to Place Portal!</i>\n<b>Tab</b> to cancel."; }
            }
            else
            { PreviewPortalText.text = $"<i><b>No Ground</b> to Place Portal!</i>\n<b>Tab</b> to cancel."; }

            #region Preview Ray (Desktop)
            if (PreviewRayCanvas != null & PreviewRay != null)
            {
                PreviewRayCanvas.gameObject.SetActive(true);
                PreviewRay.position = HeadTrack.position - Vector3.up * .1f; PreviewRay.forward = HeadDir;
                PreviewRayCanvas.sizeDelta = new Vector2(1000 * RayDistance, 100);
            }
            else if (PreviewRayCanvas != null)
            { PreviewRayCanvas.gameObject.SetActive(false); }
            #endregion
        }
        #endregion

        PlayerSphereTrigger.enabled = true;
        PlayerSphereTrigger.center = transform.InverseTransformPoint(PreviewPortal.position);

        for (int p = 0; p < Portals.Length; p++)
        {
            if (p == UsingPortal)
            { continue; }

            if (Portals[p].LifeTimer < 0)
            { continue; }

            if (Vector3.Distance(Portals[p].PortalPos, PreviewPortal.position) <= 2)
            {
                AvaliableToPlace = false;
                if (Networking.LocalPlayer.IsUserInVR())
                { PreviewPortalText.text = $"<i>Too close to \n<b>Another Portal</b>!</i>" + CancelMessage; }
                else
                { PreviewPortalText.text = "<i>Too close to \n<b>Another Portal</b>!</i>\n<b>Tab</b> to cancel."; }
            }
        }

        for (int l = 0; l < Locations.Length; l++)
        {
            if (Locations[l] != null)
            {
                if (Vector3.Distance(Locations[l].position, PreviewPortal.position) <= 2)
                {
                    AvaliableToPlace = false;
                    if (Networking.LocalPlayer.IsUserInVR())
                    { PreviewPortalText.text = $"<i>Too close to \n<b>{Locations[l].name}</b>!</i>" + CancelMessage; }
                    else
                    { PreviewPortalText.text = $"<i>Too close to \n<b>{Locations[l].name}</b>!</i>\n<b>Tab</b> to cancel."; }
                }
            }
        }

        for (int s = 0; s < Spawns.Length; s++)
        {
            if (Spawns[s] != null)
            { 
                if (Vector3.Distance(Spawns[s].position, PreviewPortal.position) <= 2)
                { 
                    AvaliableToPlace = false;
                    PreviewPortalText.text = $"<i>Too close to a <b>Spawn Point</b>!</i>\n<b>{(Networking.LocalPlayer.IsUserInVR() ? "Menu" : "Tab")}</b> to cancel.";
                    if (Networking.LocalPlayer.IsUserInVR())
                    { PreviewPortalText.text = $"<i>Too close to a <b>Spawn Point</b>!</i>" + CancelMessage; }
                    else
                    { PreviewPortalText.text = "<i>Too close to a <b>Spawn Point</b>!</i>\n<b>Tab</b> to cancel."; }
                }      
            }
        }

        PlayerInAreaTimer = PlayerInAreaTimer > 0 ? PlayerInAreaTimer - Time.deltaTime : 0;
        if (PlayerInAreaTimer > 0)
        {
            AvaliableToPlace = false;
            if (Networking.LocalPlayer.IsUserInVR())
            { PreviewPortalText.text = $"<i>Too close to a <b>Player</b>!</i>" + CancelMessage; }
            else
            { PreviewPortalText.text = "<i>Too close to a <b>Player</b>!</i>\n<b>Tab</b> to cancel."; }
        }

        PreviewPortalUI.forward = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation * Vector3.forward;
        UnavaliablePreviewPortal.SetActive(!AvaliableToPlace);
        AvaliablePreviewPortal.SetActive(AvaliableToPlace);

        if (PreviewRayImage != null)
        { PreviewRayImage.color = AvaliableToPlace ? new Color(0, 1, 0, .5f) : new Color(1, 0, 0, .5f); }
        #endregion
        */
    }
    
    #region Startup Info Hide
    public void _HideStartupInfo()
    {
        if (StartupInfo == null)
        { return; }

        StartupInfo.gameObject.SetActive(false);
    }
    #endregion

    #region Summon & Hide Portal Menu
    public void _SummonPortalMenu()
    {
        _HideStartupInfo();

        // Start of added methods for Attendee Menu     
        //This section controls what menu the player will see when they hit TAB or the VR Trigger
        //The class type is set in the inspector as a string
        //extra logic can be added to the Class Methods
        
        switch (ClassType)
        {
            case "Alchemist":
                NoClassMenu.SetActive(false);
                BarbarianMenu.SetActive(false);
                ExplorerMenu.SetActive(false);
                AlchemistMenu.SetActive(true);
                Debug.Log("Alchemist");
                break;

            case "Barbarian":
                NoClassMenu.SetActive(false);
                AlchemistMenu.SetActive(false);
                ExplorerMenu.SetActive(false);
                BarbarianMenu.SetActive(true);
                Debug.Log("Barbarian");
                break;

            case "Explorer":
                NoClassMenu.SetActive(false);
                AlchemistMenu.SetActive(false);
                BarbarianMenu.SetActive(false);
                ExplorerMenu.SetActive(true);
                Debug.Log("Explorer");
                break;

            default:
                Debug.LogWarning("Unknown class type.");
                break;
        }

        
        // End of added methods for Attendee Menu


        MenuPickupCollider.size = new Vector3(.025f, 1, .025f);
        MenuPickupCollider.center = Vector3.right * .525f * (MenuPickupSide.value == 1 ? 1 : -1);
        MenuPickupUIHandle.anchoredPosition = Vector2.right * 525 * (MenuPickupSide.value == 1 ? 1 : -1);

        Menu.pickupable = true;

        if ((SummonMode < 2 || Input.GetKey(KeyCode.Tab)) & !Menu.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            VRCPlayerApi.TrackingData HeadTracking = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

            RaycastHit hit;
            if (Physics.Raycast(HeadTracking.position, HeadTracking.rotation * Vector3.forward, out hit, .5f, GroundLayers, QueryTriggerInteraction.Ignore))
            { Menu.transform.position = hit.point + hit.normal * (.225f * Vector3.Dot(HeadTracking.rotation * Vector3.up, hit.normal) + .05f); }
            else
            { Menu.transform.position = (HeadTracking.rotation * Vector3.forward * .5f) + HeadTracking.rotation * Vector3.down * (Networking.LocalPlayer.IsUserInVR() ? .1f : 0) + HeadTracking.position; }

            Menu.transform.forward = HeadTracking.rotation * Vector3.forward;
        }

        Menu.gameObject.SetActive(true);
        Menu.transform.GetChild(0).gameObject.SetActive(true);

        _RefreshOptionsList();
        
    }

    public void _ForceSummonMenuFront()
    {
        int temp = SummonMode;
        SummonMode = 0;
        _SummonPortalMenu();
        SummonMode = temp;
    }

    public void _HidePortalMenu()
    {
        if (SummonMode < 2)
        { Menu.gameObject.SetActive(false); }
        else if (!Menu.IsHeld)
        { Menu.gameObject.SetActive(true); Menu.transform.GetChild(0).gameObject.SetActive(false); }

        MenuPickupCollider.size = new Vector3(.025f, .1f, .025f);
        MenuPickupCollider.center = Vector3.zero;
    }
    #endregion

    #region Portal Menu UI
    public void _RefreshOptionsList()
    {
        LocationsButton.gameObject.SetActive(Locations.Length > 0);

        bool SetLastOpened = false;
        for (int i = 0; i < OptionWindows.Length; i++)
        {
            OptionWindows[i].gameObject.SetActive(false);                    
            
            TeleportButtons[i].interactable = true;
            PortalButtons[i].interactable = true;

            if (HeaderToggles[i].isOn)
            {
                if (i == LastOpened || SetLastOpened)
                { HeaderToggles[i].SetIsOnWithoutNotify(false); OptionBodies[i].SetActive(false); LastOpened = i == LastOpened ? -1 : LastOpened; }
                else
                { LastOpened = i; SetLastOpened = true; }
            }

            #region View Locations
            if (!PlayerView)
            {
                if (i >= Locations.Length)
                { continue; }
                if (Locations[i] != null)
                {
                    OptionWindows[i].gameObject.SetActive(true);
                    OptionWindows[i].GetChild(0).GetChild(1).GetComponent<Text>().text = Locations[i].name;

                    OptionWindows[i].GetChild(1).GetChild(1).gameObject.SetActive(false);

                    if (HeaderToggles[i].isOn)
                    {
                        bool PrevAvaliable = false;
                        if (i < LocationPreviews.Length)
                        { PrevAvaliable = LocationPreviews[i] != null; }

                        PreviewCamera.fieldOfView = 60;

                        OptionWindows[i].GetChild(1).GetChild(1).gameObject.SetActive(true);
                        if (!PrevAvaliable)
                        {
                            PreviewCamera.transform.SetPositionAndRotation(Locations[i].position + Locations[i].up * 1.5f, Locations[i].rotation);
                            Menu.gameObject.SetActive(false);
                            PreviewCamera.Render();
                            Menu.gameObject.SetActive(true);

                            OptionWindows[i].GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = null;
                            OptionWindows[i].GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().material = PreviewCamMat;
                        }
                        else if (i < LocationPreviews.Length)
                        {
                            OptionWindows[i].GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = LocationPreviews[i];
                            OptionWindows[i].GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().material = null;
                        }
                    }
                }
            }
            #endregion
            #region View Players
            else
            {
                if (i >= VRCPlayerApi.GetPlayerCount())
                { continue; }

                VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
                VRCPlayerApi.GetPlayers(players);

                if (players[i] != null)
                {
                    OptionWindows[i].gameObject.SetActive(true);
                    OptionWindows[i].GetChild(0).GetChild(1).GetComponent<Text>().text = players[i].displayName;

                    OptionWindows[i].GetChild(1).GetChild(1).gameObject.SetActive(false);
                    if (HeaderToggles[i].isOn)
                    {
                        PreviewCamera.fieldOfView = 25;

                        OptionWindows[i].GetChild(1).GetChild(1).gameObject.SetActive(true);

                        VRCPlayerApi.TrackingData HeadTracking = players[i].GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

                        if (HeadTracking.position == Vector3.zero)
                        { PreviewCamera.transform.SetPositionAndRotation(players[i].GetPosition() + players[i].GetRotation() * (Vector3.forward * 1.5f) + Vector3.up, Quaternion.LookRotation(players[i].GetRotation() * Vector3.back)); }
                        else
                        { PreviewCamera.transform.SetPositionAndRotation(HeadTracking.position + HeadTracking.rotation * (Vector3.forward * 1.5f), Quaternion.LookRotation(HeadTracking.rotation * Vector3.back)); }

                        Menu.gameObject.SetActive(false);
                        PreviewCamera.Render();
                        Menu.gameObject.SetActive(true);

                        OptionWindows[i].GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = null;
                        OptionWindows[i].GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().material = PreviewCamMat;
                    }
                    
                    for (int n = 0; n < Net.PlayerData.Length; n++)
                    {
                        if (Networking.IsOwner(players[i], Net.PlayerData[n].gameObject))
                        {
                            TeleportButtons[i].interactable = Net.PlayerData[n].AllowPlayerTeleport;
                            PortalButtons[i].interactable = Net.PlayerData[n].AllowPlayerTeleport;
                            break; 
                        }
                    }
                }
            }
            #endregion
        }

        if (!SetLastOpened)
        { LastOpened = -1; }

        if (OptionWindows.Length == 0)
        { return; }
        if (OptionWindows[0] == null)
        { return; }

        LayoutRebuilder.ForceRebuildLayoutImmediate(OptionWindows[0].parent.GetComponent<RectTransform>());
    }

    public void _SwitchToLocationView()
    {
        PlayerView = false;
        for (int i = 0; i < HeaderToggles.Length; i++)
        {
            HeaderToggles[i].SetIsOnWithoutNotify(false);
            OptionBodies[i].SetActive(false);
        }
        LastOpened = -1;

        PreviewCamera.cullingMask = PreviewCameraLocationMask;
        _RefreshOptionsList();
    }

    public void _SwitchToPlayerView()
    {
        PlayerView = true;
        for (int i = 0; i < HeaderToggles.Length; i++)
        { 
            HeaderToggles[i].SetIsOnWithoutNotify(false);
            OptionBodies[i].SetActive(false);
        }
        LastOpened = -1;

        PreviewCamera.cullingMask = PreviewCameraPlayerMask;
        _RefreshOptionsList();
    }

    public void _CheckTeleportButtons()
    {
        bool PressedButton = false;
        for (int i = 0; i < TeleportButtons.Length; i++)
        {
            if (TeleportButtons[i].isOn)
            {
                TeleportButtons[i].SetIsOnWithoutNotify(false);
                #region Location
                if (!PlayerView)
                {
                    if (i < Locations.Length)
                    {
                        if (Locations[i] != null)
                        { Networking.LocalPlayer.TeleportTo(Locations[i].position, Locations[i].rotation); PressedButton = true; }
                    }
                }
                #endregion
                #region Player
                else
                {
                    if (i < VRCPlayerApi.GetPlayerCount())
                    {
                        VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
                        VRCPlayerApi.GetPlayers(players);
                        if (players[i] != null)
                        { Networking.LocalPlayer.TeleportTo(players[i].GetPosition(), players[i].GetRotation()); PressedButton = true; }
                    }
                }
                #endregion
            }
        }

        if (Menu.gameObject.activeSelf & PressedButton)
        { Menu.gameObject.SetActive(false); }
    }

    public void _CheckPortalButtons()
    {
        bool PressedButton = false;
        for (int i = 0; i < PortalButtons.Length; i++)
        {
            if (PortalButtons[i].isOn)
            {
                PortalButtons[i].SetIsOnWithoutNotify(false);

                int PortalID = GetPortal();
                if (PortalID == -1)
                { return; }
                UsingPortal = PortalID;
                LocPlaID = i;

                PlacingPortal = true; TimeTillAllowed = .1f;
                PreviewPortal.gameObject.SetActive(true);

                PressedButton = true;
            }
        }

        if (Menu.transform.GetChild(0).gameObject.activeInHierarchy & PressedButton)
        { Menu.gameObject.SetActive(false); Menu.transform.GetChild(0).gameObject.SetActive(false); }
    }
    #endregion

    #region Portal Management
    public int GetPortal()
    {
        if (UsingPortal >= 0 & UsingPortal < Portals.Length)
        { 
            if (Portals[UsingPortal].LifeTimer > 0 & Networking.IsOwner(Networking.LocalPlayer, Portals[UsingPortal].gameObject))
            { return UsingPortal; }
        }

        for (int i = 0; i < Portals.Length; i++)
        {
            if (Portals[i].LifeTimer < 0)
            { Networking.SetOwner(Networking.LocalPlayer, Portals[i].gameObject); return i; }
        }
        return -1;
    }

    public void PlacePortal()
    {
        PlacingPortal = false;
        PreviewPortal.gameObject.SetActive(false);

        Portals[UsingPortal].PortalPos = PreviewPortal.position;
        Portals[UsingPortal].PortalUp = PreviewPortal.up;

        Portals[UsingPortal].UsePlayerID = PlayerView;
        if (!PlayerView)
        { Portals[UsingPortal].LocationID = LocPlaID; }
        else
        {
            VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            VRCPlayerApi.GetPlayers(players);
            Portals[UsingPortal].TargertPlayerID = players[LocPlaID].playerId;
        }

        Portals[UsingPortal].LifeTimer = 30;
        Portals[UsingPortal].gameObject.SetActive(true);
        Portals[UsingPortal].RequestSerialization();

        SendCustomEventDelayedSeconds("_CheckIfPlayerPortalAvaliable", 2);
    }

    public void _CheckIfPlayerPortalAvaliable()
    {
        if (UsingPortal < 0 || UsingPortal >= Portals.Length)
        { return; }

        if (!Portals[UsingPortal].UsePlayerID || Portals[UsingPortal].TargertPlayerID < 0 || Portals[UsingPortal].LifeTimer < 0)
        { return; }
        
        for (int n = 0; n < Net.PlayerData.Length; n++)
        {
            if (Networking.IsOwner(VRCPlayerApi.GetPlayerById(Portals[UsingPortal].TargertPlayerID), Net.PlayerData[n].gameObject))
            {
                if (!Net.PlayerData[n].AllowPlayerTeleport)
                { Portals[UsingPortal].LifeTimer = 0; UsingPortal = -1; }
                break;
            }
        }

        if (UsingPortal >= 0 || UsingPortal < Portals.Length)
        { SendCustomEventDelayedSeconds("_CheckIfPlayerPortalAvaliable", 2); }
    }
    #endregion

    #region UI Settings
    public void _TogglePortals()
    {
        if (UsingPortal >= 0 & UsingPortal < Portals.Length)
        {
            Portals[UsingPortal].LifeTimer = -1;
            Portals[UsingPortal].RequestSerialization();
            UsingPortal = -1;
        }

        PortalHolder.SetActive(UsePortalsToggle.isOn);

        for (int i = 0; i < PortalButtons.Length; i++)
        {
            PortalButtons[i].interactable = UsePortalsToggle.isOn;
        }
    }

    public void _TogglePlayerTeleport()
    {
        if (PlayerData == null)
        { return; }

        PlayerData.AllowPlayerTeleport = AllowPlayerTeleportToggle.isOn;
        PlayerData.RequestSerialization();
    }

    public void _CheckMenuSummonMode()
    {
        SummonMode = Mathf.FloorToInt(SummonModeSlider.value);
        MenuPickupSensitivity.interactable = SummonMode >= 0;

        if (!Networking.LocalPlayer.IsUserInVR())
        { SummonModeDescription.text = "(PC) Press Tab"; SummonModeSlider.interactable = false; return; }

        SummonModeDescription.text =
            SummonMode == 0 ? "Press Both Triggers" :
            SummonMode == 1 ? "Double Tap a Trigger" :
            SummonMode == 2 ? "Grab from a Sholder" :
            SummonMode == 3 ? "Grab from Hips" :
            "UNKNOWN SUMMON MODE TYPE!";
    }

    public void _CheckPickupMenuSide()
    {
        MenuPickupSideText.text =
            MenuPickupSide.value == 0 ? "Left" :
            MenuPickupSide.value == 1 ? "Right" :
            "None";

        MenuPickupCollider.center = Vector3.right * (Mathf.Abs(MenuPickupCollider.center.x) * (MenuPickupSide.value == 1 ? 1 : -1));
        MenuPickupUIHandle.anchoredPosition = Vector2.right * 525 * (MenuPickupSide.value == 1 ? 1 : -1);
    }

    public void _CheckPickupMenuSensitivity()
    {
        if (.9f < MenuPickupSensitivity.value & MenuPickupSensitivity.value < 1.1f)
        { MenuPickupSensitivity.SetValueWithoutNotify(1); }
        else if (1.9f < MenuPickupSensitivity.value & MenuPickupSensitivity.value < 2.1f)
        { MenuPickupSensitivity.SetValueWithoutNotify(2); }
        else if (2.9f < MenuPickupSensitivity.value & MenuPickupSensitivity.value < 3.1f)
        { MenuPickupSensitivity.SetValueWithoutNotify(3); }

        MenuPickupSensitivityText.text = $"{Mathf.Round(MenuPickupSensitivity.value * 100) / 100}x";

        if (!Networking.LocalPlayer.IsUserInVR())
        { Menu.proximity = 1; }
    }
    #endregion

    #region Check for Players in way of Portal
    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        PlayerInAreaTimer = .5f;
    }
    #endregion

    #region Avatar Size Check
    float playerSize = 1.75f;
    public void _AvatarSizeCheck()
    {
        SendCustomEventDelayedSeconds(nameof(_AvatarSizeCheck), 5, VRC.Udon.Common.Enums.EventTiming.LateUpdate);
        VRCPlayerApi localP = Networking.LocalPlayer;
        if (localP == null) return;
        playerSize = localP.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position.y - localP.GetPosition().y;

        if (localP.GetBonePosition(HumanBodyBones.Head) == Vector3.zero && localP.GetBoneRotation(HumanBodyBones.Head) == Quaternion.identity) return;

        playerSize =
            Vector3.Distance(localP.GetBonePosition(HumanBodyBones.RightFoot), localP.GetBonePosition(HumanBodyBones.RightLowerLeg)) + // Foot to Lower Leg
            Vector3.Distance(localP.GetBonePosition(HumanBodyBones.RightLowerLeg), localP.GetBonePosition(HumanBodyBones.RightUpperLeg)) + // Lower to Upper Leg
            Vector3.Distance(localP.GetBonePosition(HumanBodyBones.RightUpperLeg), localP.GetBonePosition(HumanBodyBones.Hips)) + // Upper Leg to Hips
            Vector3.Distance(localP.GetBonePosition(HumanBodyBones.Hips), localP.GetBonePosition(HumanBodyBones.Spine)) + // Hips to Spine
            Vector3.Distance(localP.GetBonePosition(HumanBodyBones.Spine), localP.GetBonePosition(HumanBodyBones.Chest)) + // Spine to Chest
            Vector3.Distance(localP.GetBonePosition(HumanBodyBones.Chest), localP.GetBonePosition(HumanBodyBones.Neck)) + // Chest to Neck
            Vector3.Distance(localP.GetBonePosition(HumanBodyBones.Neck), localP.GetBonePosition(HumanBodyBones.Head)); // Neck to Head
    }
    #endregion

    #region Input
    bool[] InputTriggerData = new bool[2];
    public override void InputUse(bool value, UdonInputEventArgs args)
    {
        _tempInt_1 = 0;
        if (args.handType == HandType.RIGHT) _tempInt_1 = 1;

        InputTriggerData[_tempInt_1] = value;

        if (!PlacingPortal & value)
        { RightHandFocus = args.handType == HandType.RIGHT; }

        if (!PlacingPortal || !value || !AvaliableToPlace || TimeTillAllowed > 0)
        { return; }

        if (Networking.LocalPlayer.IsUserInVR())
        {
            if (args.handType == HandType.RIGHT && RightHandFocus || args.handType == HandType.LEFT && !RightHandFocus)
            { PlacePortal(); }
        }
        else
        { PlacePortal(); }
    }

    #region Input Movement
    public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
    {
        InputMove.x = value;
    }
    public override void InputMoveVertical(float value, UdonInputEventArgs args)
    {
        InputMove.y = value;
    }
    #endregion
    #endregion
}