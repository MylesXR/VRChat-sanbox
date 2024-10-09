using UdonSharp;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)] // Manual sync for controlled networking
public class Realtime_Lighting_Controller : UdonSharpBehaviour
{
    #region Variables

    [Space(5)][Header("UI Elements")][Space(10)]   
    public Slider sliderHorizontalRotation; // Slider to control horizontal panning (left-right)
    public Slider sliderVerticalRotation;   // Slider to control vertical tilting (forward-back)
    public Image lightStatusImage; // Image to reflect the on/off status of the light
    public Image lightColorImage;  // Image to reflect the light's color
    public Image ownershipButtonImage;
    public TextMeshProUGUI ownerDisplayText;  // Text to display the current owner's name
    public TextMeshProUGUI rangeText;     // Text to display the range
    public TextMeshProUGUI intensityText; // Text to display the intensity
    public TextMeshProUGUI angleText;     // Text to display the spot angle

    [Space(5)][Header("Game Objects")][Space(10)]
    public GameObject spotlight; // Spotlight GameObject
    public Color[] buttonColors;    // Array of colors assigned in the Inspector
    public Image[] buttonImages;    // Array of button images assigned in the Inspector

    [UdonSynced] private float syncedHorizontalRotation;
    [UdonSynced] private float syncedVerticalRotation;
    [UdonSynced] private bool isInteracting = false;  // Track if the slider is currently being interacted with

    private Color lightOnColor = Color.green; // Color when the light is on
    private Color lightOffColor = Color.red;  // Color when the light is off
    private float minVerticalAngle = -60f;  // Minimum vertical tilt (up-down, forward-back)
    private float maxVerticalAngle = 60f;   // Maximum vertical tilt (up-down, forward-back)
    private float minHorizontalAngle = -60f; // Minimum horizontal panning (left-right)
    private float maxHorizontalAngle = 60f;  // Maximum horizontal panning (left-right)
    private float lastHorizontalValue = 0f;
    private float lastVerticalValue = 0f;
    private Light spotlightLight;
    private Quaternion initialRotation; // To store the original rotation of the spotlight
    private VRCPlayerApi localPlayer;

    #endregion

    #region Start and Update

    void Start()
    {
        spotlightLight = spotlight.GetComponent<Light>();
        lastHorizontalValue = sliderHorizontalRotation.value;
        lastVerticalValue = sliderVerticalRotation.value;
        initialRotation = spotlight.transform.localRotation;
        ownershipButtonImage.color = lightOffColor;
        ownerDisplayText.text = "Owner: " + Networking.LocalPlayer.displayName;
        sliderHorizontalRotation.interactable = false;
        sliderVerticalRotation.interactable = false;

        UpdateUIValues();
        ApplyInitialRotationFromSliders();
        InitializeButtonColors();
        UpdateLightStatusImage();
        UpdateUIValues();
    }

    void Update()
    {
        // Only the owner should be able to interact with the sliders
        if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            sliderHorizontalRotation.interactable = true;
            sliderVerticalRotation.interactable = true;

            // Sync horizontal slider value if changed
            if (Mathf.Abs(sliderHorizontalRotation.value - syncedHorizontalRotation) > 0.01f)
            {
                syncedHorizontalRotation = sliderHorizontalRotation.value;
                RequestSerialization();  // Sync across the network
            }

            // Sync vertical slider value if changed
            if (Mathf.Abs(sliderVerticalRotation.value - syncedVerticalRotation) > 0.01f)
            {
                syncedVerticalRotation = sliderVerticalRotation.value;
                RequestSerialization();  // Sync across the network
            }
        }
        else
        {
            // Non-owners just see the current synced values from the network
            sliderHorizontalRotation.value = syncedHorizontalRotation;
            sliderVerticalRotation.value = syncedVerticalRotation;

            // Ensure sliders are disabled for non-owners
            sliderHorizontalRotation.interactable = false;
            sliderVerticalRotation.interactable = false;
        }

        // Apply the synced values to rotate the spotlight
        ApplyRotation();
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        // Request ownership, slider values, and light settings from the current owner
        RequestSerialization();
        Debug.Log("[Lighting Controller] Player joined, ownership and settings requested.");
    }



    #endregion

    #region Slider Interaction

    public void OnHorizontalSliderChanged()
    {
        MaintainOwnership(); // Ensure ownership is maintained

        // Update the synced horizontal rotation value
        syncedHorizontalRotation = sliderHorizontalRotation.value;
        RequestSerialization();  // Sync across the network
        ApplyRotation();
    }

    public void OnVerticalSliderChanged()
    {
        MaintainOwnership(); // Ensure ownership is maintained

        // Update the synced vertical rotation value
        syncedVerticalRotation = sliderVerticalRotation.value;
        RequestSerialization();  // Sync across the network
        ApplyRotation();
    }








    void DisableSliderInteractionForOthers()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            sliderHorizontalRotation.interactable = false;
            sliderVerticalRotation.interactable = false;
            Debug.Log("[Lighting Controller] Sliders disabled for non-owner: " + Networking.LocalPlayer.displayName);
        }
        else
        {
            sliderHorizontalRotation.interactable = true;
            sliderVerticalRotation.interactable = true;
            Debug.Log("[Lighting Controller] Sliders enabled for owner: " + Networking.LocalPlayer.displayName);
        }
    }



    #endregion

    #region Ownership Management

    public void TakeOwnershipButtonPressed()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Debug.Log("[Lighting Controller] Ownership transferred to: " + Networking.LocalPlayer.displayName);

            // Serialize the ownership state across the network
            RequestSerialization();
            UpdateOwnershipUI();
        }
    }

    public void RelinquishOwnershipButtonPressed()
    {
        if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            Debug.Log("[Lighting Controller] Ownership relinquished by: " + Networking.LocalPlayer.displayName);

            // Disable interaction since no owner is assigned
            sliderHorizontalRotation.interactable = false;
            sliderVerticalRotation.interactable = false;

            // Update UI to reflect "No Owner"
            ownerDisplayText.text = "No Owner";
            ownershipButtonImage.color = Color.red;

            // Sync the relinquishment across the network
            RequestSerialization();
        }
    }


    public void UpdateOwnershipUI()
    {
        if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            // If the local player owns it, display their name and allow interaction
            ownerDisplayText.text = "Owner: " + Networking.LocalPlayer.displayName;
            ownershipButtonImage.color = Color.green;  // Green for ownership
            sliderHorizontalRotation.interactable = true;
            sliderVerticalRotation.interactable = true;
        }
        else
        {
            // If no one owns it or another player owns it, show "No Owner" and disable interaction
            ownerDisplayText.text = "No Owner";
            ownershipButtonImage.color = Color.red;    // Red for no ownership
            sliderHorizontalRotation.interactable = false;
            sliderVerticalRotation.interactable = false;
        }

        Debug.Log("[Lighting Controller] Ownership UI updated.");
    }

    public override void OnOwnershipTransferred(VRCPlayerApi newOwner)
    {
        Debug.Log("[Lighting Controller] Ownership transferred to: " + newOwner.displayName);

        // Update the UI after ownership transfer for all players
        UpdateOwnershipUI();
    }

    public override void OnDeserialization()
    {
        // Apply synced settings and ownership state
        ApplyRotation();
        UpdateOwnershipUI();
        Debug.Log("[Lighting Controller] State synchronized after deserialization.");
    }



    void MaintainOwnership()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Debug.Log("[Lighting Controller] Ensuring ownership remains with: " + Networking.LocalPlayer.displayName);
        }
    }




    #endregion

    #region Update UI Elements 

    private void UpdateUIValues()
    {
        rangeText.text = Mathf.FloorToInt(spotlightLight.range).ToString();
        intensityText.text = Mathf.FloorToInt(spotlightLight.intensity).ToString();
        angleText.text = Mathf.FloorToInt(spotlightLight.spotAngle).ToString();
        Debug.Log("[Lighting Controller] UI values updated.");
    }

    void InitializeButtonColors()
    {
        if (buttonColors.Length != buttonImages.Length)
        {
            Debug.LogError("[Lighting Controller] Button color array and button image array size mismatch!");
            return;
        }

        // Set the initial color for each button's image based on the color array
        for (int i = 0; i < buttonImages.Length; i++)
        {
            if (buttonImages[i] != null)
            {
                Color buttonColor = buttonColors[i];

                // Ensure the alpha value is fully opaque
                buttonColor.a = 1f;

                // Assign the color to the button image
                buttonImages[i].color = buttonColor;
                Debug.Log("[Lighting Controller] Initialized button " + i + " color to: " + buttonColors[i]);
            }
            else
            {
                Debug.LogError("[Lighting Controller] Button image at index " + i + " is null!");
            }
        }
    }

    private void UpdateLightStatusImage()
    {
        if (spotlightLight.enabled)
        {
            lightStatusImage.color = lightOnColor;
        }
        else
        {
            lightStatusImage.color = lightOffColor;
        }

        Debug.Log("[Lighting Controller] Light status image color updated.");
    }

    #endregion

    #region Light Rotation

    public void RotateHorizontal_Networked()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "RotateHorizontal");
    }

    public void RotateVertical_Networked()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "RotateVertical");
    }

    public void RotateHorizontal()
    {
        float horizontalRotation = Mathf.Lerp(minHorizontalAngle, maxHorizontalAngle, sliderHorizontalRotation.value);
        spotlight.transform.localRotation = Quaternion.Euler(spotlight.transform.localRotation.eulerAngles.x, horizontalRotation, 0f);
        lastHorizontalValue = sliderHorizontalRotation.value;
    }

    public void RotateVertical()
    {
        float verticalRotation = Mathf.Lerp(minVerticalAngle, maxVerticalAngle, sliderVerticalRotation.value);
        spotlight.transform.localRotation = Quaternion.Euler(verticalRotation, spotlight.transform.localRotation.eulerAngles.y, 0f);
        lastVerticalValue = sliderVerticalRotation.value;
    }

    public void ApplyRotation_Networked()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ApplyRotationl");
    }

    public void ApplyRotation()
    {
        // Retain the original rotation on all axes except for the horizontal and vertical
        float horizontalRotation = Mathf.Lerp(minHorizontalAngle, maxHorizontalAngle, syncedHorizontalRotation);
        float verticalRotation = Mathf.Lerp(minVerticalAngle, maxVerticalAngle, syncedVerticalRotation);

        // Apply only horizontal (Y) and vertical (X) rotations, and preserve the initial rotation's Z-axis
        Quaternion newRotation = Quaternion.Euler(verticalRotation, horizontalRotation, initialRotation.eulerAngles.z);

        // Apply the new rotation, ensuring we keep the original rotation on the Z axis
        spotlight.transform.localRotation = newRotation;
    }

    private void ApplyInitialRotationFromSliders()
    {
        float horizontalRotation = Mathf.Lerp(minHorizontalAngle, maxHorizontalAngle, sliderHorizontalRotation.value);
        float verticalRotation = Mathf.Lerp(minVerticalAngle, maxVerticalAngle, sliderVerticalRotation.value);
        Quaternion initialSliderRotation = Quaternion.Euler(verticalRotation, horizontalRotation, initialRotation.eulerAngles.z);
        spotlight.transform.localRotation = initialSliderRotation;

        syncedHorizontalRotation = sliderHorizontalRotation.value;
        syncedVerticalRotation = sliderVerticalRotation.value;

        RequestSerialization();
    }



    #endregion

    #region Light Colour

    // Button click functions that trigger networked color change events
    public void SetColor_0_Networked() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ChangeLightColor_0"); }
    public void SetColor_1_Networked() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ChangeLightColor_1"); }
    public void SetColor_2_Networked() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ChangeLightColor_2"); }
    public void SetColor_3_Networked() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ChangeLightColor_3"); }
    public void SetColor_4_Networked() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ChangeLightColor_4"); }
    public void SetColor_5_Networked() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ChangeLightColor_5"); }
    public void SetColor_6_Networked() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ChangeLightColor_6"); }
    public void SetColor_7_Networked() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ChangeLightColor_7"); }
    public void SetColor_8_Networked() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ChangeLightColor_8"); }

    public void ChangeLightColor_0() { ChangeLightColor(0); }
    public void ChangeLightColor_1() { ChangeLightColor(1); }
    public void ChangeLightColor_2() { ChangeLightColor(2); }
    public void ChangeLightColor_3() { ChangeLightColor(3); }
    public void ChangeLightColor_4() { ChangeLightColor(4); }
    public void ChangeLightColor_5() { ChangeLightColor(5); }
    public void ChangeLightColor_6() { ChangeLightColor(6); }
    public void ChangeLightColor_7() { ChangeLightColor(7); }
    public void ChangeLightColor_8() { ChangeLightColor(8); }

    private void ChangeLightColor(int index)
    {
        if (index >= 0 && index < buttonColors.Length && index < buttonImages.Length)
        {
            Color newColor = buttonColors[index];

            // Change the light color of the spotlight
            spotlightLight.color = newColor;

            // Ensure the alpha value of the button image remains 1 (fully opaque)
            newColor.a = 1f;

            // Update the UI image (sprite on the button) to reflect the new light color
            buttonImages[index].color = newColor;

            // Update the light color image in the menu to reflect the new color
            lightColorImage.color = newColor;

            Debug.Log("[Lighting Controller] Light and image color updated to: " + newColor);
        }
        else
        {
            Debug.LogWarning("[Lighting Controller] Invalid button index or missing button image: " + index);
        }
    }

    #endregion

    #region Light Intensity

    public void IncreaseIntensity_Networked()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "IncreaseIntensity");
    }

    public void DecreaseIntensity_Networked()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DecreaseIntensity");
    }

    public void IncreaseIntensity()
    {
        spotlightLight.intensity += 1f;
        UpdateUIValues();
        Debug.Log("[Lighting Controller] Intensity increased to: " + spotlightLight.intensity);
    }

    public void DecreaseIntensity()
    {
        spotlightLight.intensity -= 1f;
        UpdateUIValues();
        Debug.Log("[Lighting Controller] Intensity decreased to: " + spotlightLight.intensity);
    }

    #endregion

    #region Light Angle

    public void IncreaseSpotAngle_Networked()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "IncreaseSpotAngle");
    }

    public void DecreaseSpotAngle_Networked()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DecreaseSpotAngle");
    }

    public void IncreaseSpotAngle()
    {
        spotlightLight.spotAngle = Mathf.Min(spotlightLight.spotAngle + 1f, 179f);
        UpdateUIValues();
        Debug.Log("[Lighting Controller] Spot angle increased to: " + spotlightLight.spotAngle);
    }

    public void DecreaseSpotAngle()
    {
        spotlightLight.spotAngle = Mathf.Max(spotlightLight.spotAngle - 1f, 1f);
        UpdateUIValues();
        Debug.Log("[Lighting Controller] Spot angle decreased to: " + spotlightLight.spotAngle);
    }

    #endregion

    #region Light Range

    public void IncreaseRange_Networked()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "IncreaseRange");
    }


    public void DecreaseRange_Networked()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DecreaseRange");
    }

    public void IncreaseRange()
    {
        spotlightLight.range += 1f;
        UpdateUIValues();
        Debug.Log("[Lighting Controller] Range increased to: " + spotlightLight.range);
    }

    public void DecreaseRange()
    {
        spotlightLight.range = Mathf.Max(spotlightLight.range - 1f, 0f);
        UpdateUIValues();
        Debug.Log("[Lighting Controller] Range decreased to: " + spotlightLight.range);
    }

    #endregion

    #region Light Toggle

    public void ToggleSpotlight_Networked()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ToggleSpotlight");
    }

    public void ToggleSpotlight()
    {
        spotlightLight.enabled = !spotlightLight.enabled;
        Debug.Log("[Lighting Controller] Spotlight toggled: " + (spotlightLight.enabled ? "On" : "Off"));

        UpdateLightStatusImage();
    }

    #endregion
}
