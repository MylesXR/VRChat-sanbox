using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Puzzle_II_Explorer_Manager : UdonSharpBehaviour
{
    public Puzzle_II_Explorer_TriggerZone[] pressurePlates; // Array of all pressure plates

    public void CheckPuzzleStatus()
    {
      
        Debug.Log("Puzzle is complete! Triggering the next sequence.");
        
    }

}
