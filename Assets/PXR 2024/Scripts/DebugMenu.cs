using TMPro;
using UdonSharp;
using UnityEngine;

public class DebugMenu : UdonSharpBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText; 

    public void Log(string message)
    {
        if (debugText != null)
        {
            debugText.text += message + "\n";  // Display message in the 3D debug console
            Debug.Log(message);  // Also log to Unity's console

        }
    }
}
