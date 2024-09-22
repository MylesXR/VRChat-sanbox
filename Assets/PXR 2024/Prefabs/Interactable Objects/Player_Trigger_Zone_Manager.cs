using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Player_Trigger_Zone_Manager : UdonSharpBehaviour
{
    public Player_Trigger_Zone[] triggerZones; // Array of all the trigger zones in the puzzle
    public GameObject[] objectsToActivate; // Objects to activate when puzzle is complete
    public GameObject[] objectsToDeactivate; // Objects to deactivate when puzzle is complete

    public void OnPlateActivated()
    {
        if (AllPlatesActivated())
        {
            Debug.Log("All trigger zones activated. Puzzle complete!");
            CompletePuzzle(); // Complete the puzzle when all plates are activated
        }
        else
        {
            Debug.Log("A trigger zone was activated, but the puzzle is not yet complete.");
        }
    }

    public void OnPlateDeactivated()
    {
        Debug.Log("A trigger zone was deactivated. Puzzle incomplete.");
        // Optional: Add logic here if something needs to happen when a plate is deactivated
    }

    private bool AllPlatesActivated()
    {
        foreach (Player_Trigger_Zone zone in triggerZones)
        {
            if (!zone.IsPlayerOnPlate())
            {
                return false; // Return false if any trigger zone is not activated
            }
        }
        return true; // All trigger zones are activated
    }

    private void CompletePuzzle()
    {
        // Activate specified objects
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log("Activated object: " + obj.name);
            }
        }

        // Deactivate specified objects
        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                Debug.Log("Deactivated object: " + obj.name);
            }
        }
    }
}
