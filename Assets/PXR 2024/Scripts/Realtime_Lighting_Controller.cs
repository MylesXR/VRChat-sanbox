using UdonSharp;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Realtime_Lighting_Controller : UdonSharpBehaviour
{
    public GameObject spotlight; // Assign your spotlight GameObject in the Inspector (which has the Light component).

    public Slider sliderHorizontalRotation; // Slider to control horizontal panning (left-right)
    public Slider sliderVerticalRotation;   // Slider to control vertical tilting (forward-back)

    public Image lightStatusImage; // Image to reflect the on/off status of the light
    public Image lightColorImage;  // Image to reflect the light's color
    public Color lightOnColor = Color.green; // Color when the light is on
    public Color lightOffColor = Color.red;  // Color when the light is off

    public TextMeshProUGUI rangeText;     // Text to display the range
    public TextMeshProUGUI intensityText; // Text to display the intensity
    public TextMeshProUGUI angleText;     // Text to display the spot angle

    public Color[] buttonColors;    // Array of colors assigned in the Inspector
    public Image[] buttonImages;    // Array of button images assigned in the Inspector

    private float minVerticalAngle = -60f;  // Minimum vertical tilt (up-down, forward-back)
    private float maxVerticalAngle = 60f;   // Maximum vertical tilt (up-down, forward-back)
    private float minHorizontalAngle = -60f; // Minimum horizontal panning (left-right)
    private float maxHorizontalAngle = 60f;  // Maximum horizontal panning (left-right)

    private float lastHorizontalValue = 0f;
    private float lastVerticalValue = 0f;

    private Light spotlightLight;

    void Start()
    {
        spotlightLight = spotlight.GetComponent<Light>();
        Debug.Log("[Lighting Controller] Starting...");

        // Initialize button colors and images
        InitializeButtonColors();

        // Ensure the light status image is correct at the start of the game
        UpdateLightStatusImage();

        // Update the text values at the start of the game
        UpdateUIValues();
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

    void Update()
    {
        if (sliderHorizontalRotation.value != lastHorizontalValue || sliderVerticalRotation.value != lastVerticalValue)
        {
            UpdateRotation();
            lastHorizontalValue = sliderHorizontalRotation.value;
            lastVerticalValue = sliderVerticalRotation.value;
        }
    }

    private void UpdateRotation()
    {
        float horizontalRotation = Mathf.Lerp(minHorizontalAngle, maxHorizontalAngle, sliderHorizontalRotation.value);
        float verticalRotation = Mathf.Lerp(minVerticalAngle, maxVerticalAngle, sliderVerticalRotation.value);
        spotlight.transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
    }

    public void ToggleSpotlight()
    {
        spotlightLight.enabled = !spotlightLight.enabled;
        Debug.Log("[Lighting Controller] Spotlight toggled: " + (spotlightLight.enabled ? "On" : "Off"));

        UpdateLightStatusImage();
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

    // Functions to change the light color and button image based on the button index
    public void SetColor_0() { ChangeLightColor(0); }
    public void SetColor_1() { ChangeLightColor(1); }
    public void SetColor_2() { ChangeLightColor(2); }
    public void SetColor_3() { ChangeLightColor(3); }
    public void SetColor_4() { ChangeLightColor(4); }
    public void SetColor_5() { ChangeLightColor(5); }
    public void SetColor_6() { ChangeLightColor(6); }
    public void SetColor_7() { ChangeLightColor(7); }
    public void SetColor_8() { ChangeLightColor(8); }

    // Function to change the light color and button image
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

    private void UpdateUIValues()
    {
        rangeText.text = Mathf.FloorToInt(spotlightLight.range).ToString();
        intensityText.text = Mathf.FloorToInt(spotlightLight.intensity).ToString();
        angleText.text = Mathf.FloorToInt(spotlightLight.spotAngle).ToString();
        Debug.Log("[Lighting Controller] UI values updated.");
    }
}
