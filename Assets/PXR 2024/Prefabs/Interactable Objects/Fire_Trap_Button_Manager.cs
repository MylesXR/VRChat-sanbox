using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Fire_Trap_Button_Manager : UdonSharpBehaviour
{
    public GameObject[] correctPattern; // Array to define the correct button order
    public GameObject[] objectsToDeactivate; // Array of objects to deactivate upon correct pattern

    private GameObject[] playerPattern; // Array to store the player's input pattern
    private int currentIndex;    // Tracks the current position in the player's pattern
    private int maxGuesses = 3;  // Maximum guesses per turn

    void Start()
    {
        playerPattern = new GameObject[correctPattern.Length]; // Initialize the player's pattern array
        ResetPattern(); // Reset the pattern at the start
    }

    public void RegisterButtonPress(GameObject button)
    {
        if (currentIndex >= maxGuesses)
        {
            Debug.Log("Maximum guesses reached. Pattern check will now occur.");
            CheckPattern();
            return;
        }

        Debug.Log("Registering button press: " + button.name);

        playerPattern[currentIndex] = button;
        currentIndex++;

        Debug.Log("Button " + button.name + " registered as guess " + currentIndex);
        Debug.Log("Current guess " + currentIndex + " out of " + maxGuesses);

        if (currentIndex == maxGuesses)
        {
            Debug.Log("All guesses used. Checking pattern now.");
            CheckPattern();
        }
    }

    private void CheckPattern()
    {
        Debug.Log("Checking player pattern against correct pattern.");

        bool isCorrect = true;

        for (int i = 0; i < correctPattern.Length; i++)
        {
            Debug.Log("Comparing guess " + (i + 1) + ": " + (playerPattern[i] != null ? playerPattern[i].name : "null") +
                      " with correct pattern " + correctPattern[i].name);

            if (playerPattern[i] != correctPattern[i])
            {
                Debug.Log("Pattern mismatch at guess " + (i + 1) + ".");
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            Debug.Log("Correct Pattern! Player has successfully matched the sequence.");
            DeactivateObjects(); // Deactivate the specified objects
            // Trigger any other success events here
        }
        else
        {
            Debug.Log("Incorrect Pattern. Player needs to try again.");
        }

        ResetPattern(); // Reset the pattern after checking
    }

    private void DeactivateObjects()
    {
        if (objectsToDeactivate != null && objectsToDeactivate.Length > 0)
        {
            foreach (GameObject obj in objectsToDeactivate)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    Debug.Log("Deactivated object: " + obj.name);
                }
            }
        }
        else
        {
            Debug.Log("No objects to deactivate or array is empty.");
        }
    }

    private void ResetPattern()
    {
        Debug.Log("Resetting player pattern and guesses.");
        currentIndex = 0; // Reset the index to start checking from the beginning
        for (int i = 0; i < playerPattern.Length; i++)
        {
            playerPattern[i] = null; // Initialize the pattern with nulls
        }
        Debug.Log("Pattern reset. Ready for the next attempt.");
    }
}
