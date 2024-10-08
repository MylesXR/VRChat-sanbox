using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro; // For working with TextMeshPro text components
using UnityEngine.UI; // For working with UI Image components

public class Realtime_Lighting_Controller : UdonSharpBehaviour
{
    public GameObject spotlight; // Assign your spotlight GameObject in the Inspector (which has the Light component).

    public UnityEngine.UI.Slider sliderHorizontalRotation; // Slider to control horizontal panning (left-right)
    public UnityEngine.UI.Slider sliderVerticalRotation;   // Slider to control vertical tilting (forward-back)

    public Image lightStatusImage; // Image to reflect the on/off status of the light (set in the Inspector)
    public Color lightOnColor = Color.green; // Color when the light is on
    public Color lightOffColor = Color.red;  // Color when the light is off

    public TextMeshProUGUI rangeText;     // Text to display the range
    public TextMeshProUGUI intensityText; // Text to display the intensity
    public TextMeshProUGUI angleText;     // Text to display the spot angle

    private float minVerticalAngle = -60f;  // Minimum vertical tilt (up-down, forward-back)
    private float maxVerticalAngle = 60f;   // Maximum vertical tilt (up-down, forward-back)
    private float minHorizontalAngle = -60f; // Minimum horizontal panning (left-right)
    private float maxHorizontalAngle = 60f;  // Maximum horizontal panning (left-right)

    private float lastHorizontalValue = 0f;
    private float lastVerticalValue = 0f;

    void Start()
    {
        // Ensure the light status image is correct at the start of the game
        UpdateLightStatusImage();

        // Update the text values at the start of the game
        UpdateUIValues();
    }

    void Update()
    {
        // Only update if the sliders are being moved (i.e., values change)
        if (sliderHorizontalRotation.value != lastHorizontalValue || sliderVerticalRotation.value != lastVerticalValue)
        {
            UpdateRotation();
            lastHorizontalValue = sliderHorizontalRotation.value;
            lastVerticalValue = sliderVerticalRotation.value;
        }
    }

    private void UpdateRotation()
    {
        // Horizontal panning (left-right movement) mapped from -60 to +60 degrees
        float horizontalRotation = Mathf.Lerp(minHorizontalAngle, maxHorizontalAngle, sliderHorizontalRotation.value);

        // Vertical tilting (forward-back movement), mapped from -60 to +60 degrees
        float verticalRotation = Mathf.Lerp(minVerticalAngle, maxVerticalAngle, sliderVerticalRotation.value);

        // Apply the panning (left-right) to the Y-axis and tilting (up-down) to the X-axis
        spotlight.transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
    }

    // Function to toggle the spotlight on/off and change the image color
    public void ToggleSpotlight()
    {
        Light spotlightLight = spotlight.GetComponent<Light>(); // Get the Light component from the spotlight GameObject
        spotlightLight.enabled = !spotlightLight.enabled; // Toggle the Light component

        // Update the color of the image based on the spotlight state
        UpdateLightStatusImage();
    }

    // Function to update the light status image color based on the light's state
    private void UpdateLightStatusImage()
    {
        Light spotlightLight = spotlight.GetComponent<Light>();

        if (spotlightLight.enabled)
        {
            lightStatusImage.color = lightOnColor; // Set to "on" color
        }
        else
        {
            lightStatusImage.color = lightOffColor; // Set to "off" color
        }
    }

    // Function to increase the intensity of the spotlight by 1
    public void IncreaseIntensity()
    {
        Light spotlightLight = spotlight.GetComponent<Light>();
        spotlightLight.intensity += 1f; // Increase intensity by 1
        UpdateUIValues(); // Update the text display
    }

    // Function to decrease the intensity of the spotlight by 1
    public void DecreaseIntensity()
    {
        Light spotlightLight = spotlight.GetComponent<Light>();
        spotlightLight.intensity -= 1f; // Decrease intensity by 1
        UpdateUIValues(); // Update the text display
    }

    // Function to increase the spot angle of the spotlight by 1
    public void IncreaseSpotAngle()
    {
        Light spotlightLight = spotlight.GetComponent<Light>();
        spotlightLight.spotAngle = Mathf.Min(spotlightLight.spotAngle + 1f, 179f); // Increase spot angle by 1, max at 179 degrees
        UpdateUIValues(); // Update the text display
    }

    // Function to decrease the spot angle of the spotlight by 1
    public void DecreaseSpotAngle()
    {
        Light spotlightLight = spotlight.GetComponent<Light>();
        spotlightLight.spotAngle = Mathf.Max(spotlightLight.spotAngle - 1f, 1f); // Decrease spot angle by 1, min at 1 degree
        UpdateUIValues(); // Update the text display
    }

    // Function to increase the range of the spotlight by 1
    public void IncreaseRange()
    {
        Light spotlightLight = spotlight.GetComponent<Light>();
        spotlightLight.range += 1f; // Increase range by 1
        UpdateUIValues(); // Update the text display
    }

    // Function to decrease the range of the spotlight by 1
    public void DecreaseRange()
    {
        Light spotlightLight = spotlight.GetComponent<Light>();
        spotlightLight.range = Mathf.Max(spotlightLight.range - 1f, 0f); // Decrease range by 1, min at 0
        UpdateUIValues(); // Update the text display
    }

    // Function to update the UI text for range, intensity, and angle
    private void UpdateUIValues()
    {
        Light spotlightLight = spotlight.GetComponent<Light>();

        // Update text values by setting them directly using .text
        rangeText.text = "Range: " + spotlightLight.range.ToString();
        intensityText.text = "Intensity: " + spotlightLight.intensity.ToString();
        angleText.text = "Spot Angle: " + spotlightLight.spotAngle.ToString();
    }
}
