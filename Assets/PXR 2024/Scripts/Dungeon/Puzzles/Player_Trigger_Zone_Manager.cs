using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)] // Manual sync for networking
public class Player_Trigger_Zone_Manager : UdonSharpBehaviour
{
    public Player_Trigger_Zone[] triggerZones; // Array of all the trigger zones in the puzzle
    public Animator MrSoupsGoldenLadleAnimator; // Animator controlling the animation

    [UdonSynced] private bool puzzleComplete = false; // Sync the puzzle state across all players
    private int totalPlayers; // Total number of players
    private int playersInTrigger = 0; // Number of players currently in the trigger

    void Start()
    {
        if (MrSoupsGoldenLadleAnimator == null)
        {
            Debug.LogWarning("Animator not assigned!");
            return;
        }

        totalPlayers = VRCPlayerApi.GetPlayerCount(); // Get the number of players in the instance
    }

    // Call this when a player activates a trigger zone
    public void OnPlateActivated()
    {
        Debug.LogWarning("A trigger zone was activated.");
        if (AllPlatesActivated())
        {
            Debug.LogWarning("All trigger zones activated. Puzzle complete!");
            if (!puzzleComplete)
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, "CompletePuzzle"); // Broadcast completion to all
            }
        }
    }

    // Call this when a player deactivates a trigger zone
    public void OnPlateDeactivated()
    {
        Debug.LogWarning("A trigger zone was deactivated.");
    }

    // Call this method when a player enters a trigger zone
    public void OnPlayerEnterTriggerZone()
    {
        playersInTrigger++;
        Debug.LogWarning(playersInTrigger + " out of " + totalPlayers + " players are in the trigger.");

        // Check if all players are in the trigger zones
        if (playersInTrigger >= totalPlayers)
        {
            Debug.LogWarning("All players are in the trigger zones.");
            if (!puzzleComplete)
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, "CompletePuzzle"); // Trigger puzzle completion for all
            }
        }
    }

    // Call this method when a player exits a trigger zone
    public void OnPlayerExitTriggerZone()
    {
        playersInTrigger--;
        Debug.LogWarning(playersInTrigger + " players are still in the trigger.");
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

    // This is called via SendCustomNetworkEvent across all clients
    public void CompletePuzzle()
    {
        if (!puzzleComplete)
        {
            puzzleComplete = true; // Mark the puzzle as complete
            RequestSerialization(); // Sync the puzzle state across all players
            PlayAnimation(); // Trigger animation for all players
        }
    }

    // Trigger the animation using SendCustomNetworkEvent and SetTrigger, as per your working example
    public void PlayAnimation()
    {
        if (MrSoupsGoldenLadleAnimator != null)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "TriggerAnimation"); // Send event to all players to play the animation
        }
    }

    // This method is triggered by SendCustomNetworkEvent to set the animation trigger
    public void TriggerAnimation()
    {
        if (MrSoupsGoldenLadleAnimator != null)
        {
            MrSoupsGoldenLadleAnimator.SetTrigger("PlayAnimation"); // Set the trigger for the animation
            Debug.LogWarning("Triggered Mr-Soups-Golden-Ladle animation.");
        }
    }

    public override void OnDeserialization()
    {
        // If the puzzle is complete but the animation hasn't played, ensure it plays for late joiners
        if (puzzleComplete)
        {
            PlayAnimation();
        }
    }
}
