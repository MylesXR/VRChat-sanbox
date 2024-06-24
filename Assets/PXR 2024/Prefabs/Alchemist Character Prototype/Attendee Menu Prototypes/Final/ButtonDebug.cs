using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ButtonDebug : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    private RectTransform debugRectTransform;
    private Image debugImage;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

        // Create the debug image
        GameObject debugImageObject = new GameObject("DebugImage");
        debugImageObject.transform.SetParent(transform, false);

        debugImage = debugImageObject.AddComponent<Image>();
        debugImage.color = new Color(1, 0, 0, 0.3f); // Semi-transparent red

        debugRectTransform = debugImage.GetComponent<RectTransform>();
        debugRectTransform.SetParent(transform, false);
    }

    void Update()
    {
        UpdateDebugImage();
    }

    private void UpdateDebugImage()
    {
        if (debugRectTransform != null)
        {
            Rect rect = textMeshPro.rectTransform.rect;
            Vector2 size = new Vector2(rect.width, rect.height);

            // Adjust the size and position of the debug image
            debugRectTransform.sizeDelta = size;
            debugRectTransform.position = textMeshPro.rectTransform.position;
            debugRectTransform.rotation = textMeshPro.rectTransform.rotation;
        }
    }
}
