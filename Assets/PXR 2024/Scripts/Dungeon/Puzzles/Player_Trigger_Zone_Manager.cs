using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)] // Manual sync for networking
public class Player_Trigger_Zone_Manager : UdonSharpBehaviour
{
    public Player_Trigger_Zone[] triggerZones; // Array of all the trigger zones
    public Animator MrSoupsGoldenLadleAnimator; // Animator for the animation

    public int requiredPlayers; // Set the number of players required to complete the puzzle
    public int requiredTriggers; // Set the number of triggers required to complete the puzzle

    [UdonSynced] private int syncedPlayersInTrigger = 0; // Synced count of players in trigger zones
    [UdonSynced] private bool puzzleComplete = false; // Sync puzzle state across all players
    private int localPlayersInTrigger = 0; // Local count of players currently in the trigger zones

    /*
    void Start()
    {
        Debug.LogWarning("Total trigger zones in scene: " + triggerZones.Length);
        Debug.LogWarning("Required players to complete puzzle: " + requiredPlayers);
        Debug.LogWarning("Required triggers to complete puzzle: " + requiredTriggers);
    }
    */

    // Call this when a player activates a trigger
    public void OnPlateActivated()
    {
        localPlayersInTrigger++;
        //Debug.LogWarning("Local player entered trigger zone. Local players in trigger: " + localPlayersInTrigger);

        // If enough players are in triggers, sync this event across all players
        if (AllPlatesActivated())
        {
            if (!puzzleComplete)
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, "CompletePuzzle"); // Broadcast puzzle completion
            }
        }
    }

    // Call this when a player deactivates a trigger
    public void OnPlateDeactivated()
    {
        localPlayersInTrigger--;
        //Debug.LogWarning("Local player left trigger zone. Local players in trigger: " + localPlayersInTrigger);

        // If puzzle was complete but a player leaves, reset the puzzle
        if (puzzleComplete)
        {
            puzzleComplete = false;
            RequestSerialization(); // Sync the reset state across the network
            //Debug.LogWarning("Puzzle reset because a player left a trigger zone.");
        }
    }

    // Check if all plates are activated and the correct number of players are in their triggers
    private bool AllPlatesActivated()
    {
        int activatedTriggers = 0;

        // Count how many triggers have players in them
        foreach (Player_Trigger_Zone zone in triggerZones)
        {
            if (zone.IsPlayerOnPlate())
            {
                activatedTriggers++;
            }
        }

        //Debug.LogWarning("Triggers activated: " + activatedTriggers);

        // Ensure the required number of triggers and players are present
        return activatedTriggers >= requiredTriggers && localPlayersInTrigger >= requiredPlayers;
    }

    // Complete the puzzle and sync it across the network
    public void CompletePuzzle()
    {
        if (!puzzleComplete)
        {
            puzzleComplete = true; // Mark the puzzle as complete
            syncedPlayersInTrigger = localPlayersInTrigger; // Sync the number of players in trigger zones
            RequestSerialization(); // Sync the state across players
            PlayAnimation(); // Trigger the animation
            //Debug.LogWarning("Puzzle completed and synced.");
        }
    }

    // Trigger the animation
    public void PlayAnimation()
    {
        if (MrSoupsGoldenLadleAnimator != null)
        {
            MrSoupsGoldenLadleAnimator.SetTrigger("PlayAnimation"); // Play the animation
            //Debug.LogWarning("Playing Mr-Soups-Golden-Ladle animation.");
        }
    }

    public override void OnDeserialization()
    {
        // Ensure late joiners also see the animation if the puzzle is complete
        if (puzzleComplete)
        {
            PlayAnimation();
            //Debug.LogWarning("Deserialization triggered: Playing animation for late joiners.");
        }
    }
}
