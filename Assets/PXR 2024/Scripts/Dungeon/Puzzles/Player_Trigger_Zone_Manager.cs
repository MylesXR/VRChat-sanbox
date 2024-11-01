using UdonSharp;
using UnityEngine;
using VRC.Udon.Common.Interfaces;

public class Player_Trigger_Zone_Manager : UdonSharpBehaviour
{
    public Player_Trigger_Zone[] triggerZones; // Array of all the trigger zones
    public Animator MrSoupsGoldenLadleAnimator; // Animator for the animation

    public int requiredPlayers; // Set the number of players required to complete the puzzle
    public int requiredTriggers; // Set the number of triggers required to complete the puzzle

    [UdonSynced] private int syncedPlayersInTrigger = 0; // Synced count of players in trigger zones
    [UdonSynced] private bool puzzleComplete = false; // Sync puzzle state across all players
    private int localPlayersInTrigger = 0; // Local count of players currently in the trigger zones

    public void OnPlateActivated() // When a player enters the trigger
    {
        localPlayersInTrigger++;
        if (AllPlatesActivated())
        {
            if (!puzzleComplete)
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, "CompletePuzzle");
            }
        }
    }
    
    public void OnPlateDeactivated() // When a player leaves the trigger
    {
        localPlayersInTrigger--;
        
        if (puzzleComplete) // If puzzle was complete but a player leaves, reset the puzzle
        {
            puzzleComplete = false;
        }
    }


    private bool AllPlatesActivated() // Check if all plates are activated and the correct number of players are in their triggers
    {
        int activatedTriggers = 0;

        foreach (Player_Trigger_Zone zone in triggerZones)
        {
            if (zone.IsPlayerOnPlate())
            {
                activatedTriggers++;
            }
        }
        return activatedTriggers >= requiredTriggers && localPlayersInTrigger >= requiredPlayers;
    }

    public void CompletePuzzle()
    {
        if (!puzzleComplete)
        {
            puzzleComplete = true;
            syncedPlayersInTrigger = localPlayersInTrigger; // Sync the number of players in trigger zones
            SendCustomNetworkEvent(NetworkEventTarget.All, "PlayAnimation");  
        }
    }

    public void PlayAnimation()
    {
        if (MrSoupsGoldenLadleAnimator != null)
        {
            MrSoupsGoldenLadleAnimator.SetTrigger("PlayAnimation"); // Play the animation
        }
    }
}