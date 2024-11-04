using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class Fire_Trap_Manager : UdonSharpBehaviour
{
    [Tooltip("Top traps that always stay on and never turn off.")]
    public GameObject[] TopFireTraps; // Traps that always stay on

    [Tooltip("Bottom traps that flash on and off during the sequence.")]
    public GameObject[] BottomFireTraps; // 8 traps that flash on and off

    [Tooltip("Time between flashes for the bottom traps.")]
    public float flashInterval = 2.0f; // Time between flashes

    [Tooltip("How long the trap stays active (on) during a flash.")]
    public float flashDuration = 1.0f; // Time the trap stays active during flash

    [Tooltip("Total time the flashing sequence lasts before all traps are reset and stay on.")]
    public float flashResetTime = 10.0f; // Total time before resetting the flash cycle

    [UdonSynced]
    private bool isFlashing = false; // Sync flashing state across all players

    [UdonSynced]
    private bool buttonLocked = false; // Lock to prevent button presses during flashing

    private void Start()
    {
        // Ensure top traps always stay on
        for (int i = 0; i < TopFireTraps.Length; i++)
        {
            if (TopFireTraps[i] != null)
            {
                TopFireTraps[i].SetActive(true); // Top traps always active
            }
        }

        // Bottom traps start on but will flash
        ResetBottomTraps();
    }

    public override void Interact()
    {
        // Prevent button interaction if flashing is already in progress
        if (buttonLocked)
        {
            //Debug.LogWarning("[Fire_Trap_Manager] Button press ignored: traps are flashing.");
            return;
        }

        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        // Lock the button to prevent further presses during flashing
        buttonLocked = true;
        //Debug.LogWarning("[Fire_Trap_Manager] Button locked: flashing sequence started.");

        // Toggle flashing state
        isFlashing = !isFlashing;

        // Sync the flashing state across all players
        RequestSerialization();
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "StartFlashing");
    }

    // Start or stop the flashing sequence for bottom traps
    public void StartFlashing()
    {
        if (isFlashing)
        {
            FlashBottomTraps(); // Start the flashing cycle
        }
        else
        {
            //ResetBottomTraps(); // Reset bottom traps to fully on
            buttonLocked = false; // Unlock the button once reset
            //Debug.LogWarning("[Fire_Trap_Manager] Traps reset: button unlocked.");
        }
    }

    // Flash all bottom traps for the duration and interval
    public void FlashBottomTraps()
    {
        //Debug.LogWarning("[Fire_Trap_Manager] Flashing sequence started.");

        for (int i = 0; i < BottomFireTraps.Length; i++)
        {
            if (BottomFireTraps[i] != null)
            {
                FlashTrap(i); // Flash each trap
            }
        }

        // After the flash reset time, set all bottom traps to fully on and stop flashing
        //SendCustomEventDelayedSeconds("StopFlashingAndReset", flashResetTime);
    }

    // Flash a specific trap: turn on for flashDuration, then off for flashInterval
    private void FlashTrap(int trapIndex)
    {
        if (BottomFireTraps[trapIndex] != null && isFlashing)
        {
            BottomFireTraps[trapIndex].SetActive(true); // Turn trap on
            //Debug.LogWarning("[Fire_Trap_Manager] Trap " + trapIndex + " turned on.");
            SendCustomEventDelayedSeconds("TurnOffTrap_" + trapIndex, flashDuration); // Turn off after flashDuration
        }
    }

    // Turn off the specific trap after the duration
    public void TurnOffTrap_0() { TurnOffTrap(0); }
    public void TurnOffTrap_1() { TurnOffTrap(1); }
    public void TurnOffTrap_2() { TurnOffTrap(2); }
    public void TurnOffTrap_3() { TurnOffTrap(3); }
    public void TurnOffTrap_4() { TurnOffTrap(4); }
    public void TurnOffTrap_5() { TurnOffTrap(5); }
    public void TurnOffTrap_6() { TurnOffTrap(6); }
    public void TurnOffTrap_7() { TurnOffTrap(7); }

    private void TurnOffTrap(int trapIndex)
    {
        if (BottomFireTraps[trapIndex] != null && isFlashing)
        {
            BottomFireTraps[trapIndex].SetActive(false); // Turn trap off
            //Debug.LogWarning("[Fire_Trap_Manager] Trap " + trapIndex + " turned off.");
            SendCustomEventDelayedSeconds("TurnOnTrap_" + trapIndex, flashInterval); // Turn it back on after flashInterval
        }
    }

    // Turn on the trap again after the interval
    public void TurnOnTrap_0() { TurnOnTrap(0); }
    public void TurnOnTrap_1() { TurnOnTrap(1); }
    public void TurnOnTrap_2() { TurnOnTrap(2); }
    public void TurnOnTrap_3() { TurnOnTrap(3); }
    public void TurnOnTrap_4() { TurnOnTrap(4); }
    public void TurnOnTrap_5() { TurnOnTrap(5); }
    public void TurnOnTrap_6() { TurnOnTrap(6); }
    public void TurnOnTrap_7() { TurnOnTrap(7); }

    private void TurnOnTrap(int trapIndex)
    {
        if (BottomFireTraps[trapIndex] != null && isFlashing)
        {
            BottomFireTraps[trapIndex].SetActive(true); // Turn trap back on
            //Debug.LogWarning("[Fire_Trap_Manager] Trap " + trapIndex + " turned on.");
            SendCustomEventDelayedSeconds("TurnOffTrap_" + trapIndex, flashDuration); // Turn it off after the duration
        }
    }

    // After flashing for the reset time, stop flashing and reset all bottom traps to fully on
    public void StopFlashingAndReset()
    {
        isFlashing = false; // Stop the flashing state
        ResetBottomTraps(); // Make all bottom traps fully on

        // Unlock the button, allowing the player to press it again
        buttonLocked = false;
        //Debug.LogWarning("[Fire_Trap_Manager] Flashing finished: button unlocked.");
    }

    // Resets bottom traps to their default state (all on)
    private void ResetBottomTraps()
    {
        for (int i = 0; i < BottomFireTraps.Length; i++)
        {
            if (BottomFireTraps[i] != null)
            {
                BottomFireTraps[i].SetActive(true); // Reset bottom traps to fully on
                //Debug.LogWarning("[Fire_Trap_Manager] Trap " + i + " reset to fully on.");
            }
        }
    }
}
