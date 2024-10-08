using UdonSharp;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class Realtime_Lighting_Controller : UdonSharpBehaviour
{
    #region Variables

    [Space(5)][Header("UI Elements")][Space(10)]   
    public Slider sliderHorizontalRotation; // Slider to control horizontal panning (left-right)
    public Slider sliderVerticalRotation;   // Slider to control vertical tilting (forward-back)
    public Image lightStatusImage; // Image to reflect the on/off status of the light
    public Image lightColorImage;  // Image to reflect the light's color
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
        localPlayer = Networking.LocalPlayer;  // Ensure localPlayer is set
        if (localPlayer == null)
        {
            Debug.LogError("[Lighting Controller] Local player is null!");
        }

        spotlightLight = spotlight.GetComponent<Light>();
        lastHorizontalValue = sliderHorizontalRotation.value;
        lastVerticalValue = sliderVerticalRotation.value;

        // Save the initial rotation of the spotlight
        initialRotation = spotlight.transform.localRotation;

        // Set the spotlight's rotation to match the initial slider values
        ApplyInitialRotationFromSliders();

        InitializeButtonColors();
        UpdateLightStatusImage();
        UpdateUIValues();
    }

    void Update()
    {
        // Only the owner should be able to sync and move the sliders
        if (Networking.IsOwner(localPlayer, gameObject))
        {
            // If the player is interacting with the slider
            if (isInteracting)
            {
                // Sync when horizontal slider values change
                if (Mathf.Abs(sliderHorizontalRotation.value - lastHorizontalValue) > 0.01f)
                {
                    syncedHorizontalRotation = sliderHorizontalRotation.value;
                    RequestSerialization();  // Sync across the network
                    lastHorizontalValue = sliderHorizontalRotation.value;
                }

                // Sync when vertical slider values change
                if (Mathf.Abs(sliderVerticalRotation.value - lastVerticalValue) > 0.01f)
                {
                    syncedVerticalRotation = sliderVerticalRotation.value;
                    RequestSerialization();  // Sync across the network
                    lastVerticalValue = sliderVerticalRotation.value;
                }
            }

            // When the player lets go of the slider, relinquish ownership
            if (!IsPlayerInteractingWithSlider())
            {
                RelinquishOwnership(); // Release ownership when no longer interacting
            }
        }

        // Update the slider values regardless of ownership to keep everything in sync
        sliderHorizontalRotation.value = syncedHorizontalRotation;
        sliderVerticalRotation.value = syncedVerticalRotation;

        // Apply the synced values to rotate the spotlight
        ApplyRotation();
    }




    #endregion

    #region Ownership Management

    private void TakeOwnershipIfNecessary()
    {
        if (!isInteracting)
        {
            isInteracting = true;
            Networking.SetOwner(localPlayer, gameObject);  // Transfer ownership to the interacting player
            RequestSerialization();  // Sync interaction state across the network
            Debug.Log("[Lighting Controller] Ownership taken by: " + localPlayer.displayName);
        }
    }

    private void RelinquishOwnership()
    {
        if (isInteracting && Networking.IsOwner(localPlayer, gameObject))
        {
            isInteracting = false;  // Mark interaction as stopped
            RequestSerialization();  // Sync interaction state across the network
            Debug.Log("[Lighting Controller] Ownership relinquished.");
        }
    }

    public void OnHorizontalSliderChanged()
    {
        TakeOwnershipIfNecessary();
        syncedHorizontalRotation = sliderHorizontalRotation.value;
        RequestSerialization();  // Sync across the network
    }

    public void OnVerticalSliderChanged()
    {
        TakeOwnershipIfNecessary();
        syncedVerticalRotation = sliderVerticalRotation.value;
        RequestSerialization();  // Sync across the network
    }

    public void ReleaseInteraction()
    {
        RelinquishOwnership();
    }

    public void OnBeginDrag()
    {
        TakeOwnershipIfNecessary();
    }

    public void OnEndDrag()
    {
        RelinquishOwnership();
    }

    public bool IsPlayerInteractingWithSlider()
    {
        // Use the isInteracting variable to determine if the player is currently interacting with the slider
        return isInteracting;
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

    public override void OnDeserialization()
    {
        ApplyRotation();
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
        // Set the initial rotation based on the slider values
        float horizontalRotation = Mathf.Lerp(minHorizontalAngle, maxHorizontalAngle, sliderHorizontalRotation.value);
        float verticalRotation = Mathf.Lerp(minVerticalAngle, maxVerticalAngle, sliderVerticalRotation.value);

        // Apply the rotation based on initial slider values
        Quaternion initialSliderRotation = Quaternion.Euler(verticalRotation, horizontalRotation, initialRotation.eulerAngles.z);
        spotlight.transform.localRotation = initialSliderRotation;

        // Update synced values to start with the correct rotation
        syncedHorizontalRotation = sliderHorizontalRotation.value;
        syncedVerticalRotation = sliderVerticalRotation.value;

        // Sync the starting rotation across the network
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
