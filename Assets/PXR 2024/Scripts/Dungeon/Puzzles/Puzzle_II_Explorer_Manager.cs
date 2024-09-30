using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Puzzle_II_Explorer_Manager : UdonSharpBehaviour
{
    public Puzzle_II_Explorer_TriggerZone[] pressurePlates; // Array of all pressure plates

    // This method is called whenever a plate status changes
    public void UpdatePlateStatus()
    {
        bool puzzleComplete = true;

        // Check if all pressure plates have the correct objects
        foreach (Puzzle_II_Explorer_TriggerZone plate in pressurePlates)
        {
            if (!plate.IsCorrectObjectOnPlate())
            {
                puzzleComplete = false;
                break;
            }
        }

        if (puzzleComplete)
        {
            // All plates have the correct objects, puzzle is complete
            CheckPuzzleStatus();
        }
    }

    public void CheckPuzzleStatus()
    {
        // Code to trigger when puzzle is completed
        Debug.LogWarning("Puzzle is complete! Triggering the next sequence.");
        // Place the next sequence of code here to trigger other events
    }
}
