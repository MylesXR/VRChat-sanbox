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
            debugText.text += message + "\n";  
            Debug.Log(message);  
        }
    }


    public void LogWarning(string message)
    {
        if (debugText != null)
        {
            debugText.text += "\nWARNING: " + message; 
            Debug.Log(message);  
        }
    }


    public void LogError(string message)
    {
        if (debugText != null)
        {
            debugText.text += "\nERROR: " + message;  
            Debug.Log(message);  
        }
    }
}
