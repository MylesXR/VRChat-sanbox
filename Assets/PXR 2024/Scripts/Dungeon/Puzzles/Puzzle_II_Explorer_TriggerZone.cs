using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Puzzle_II_Explorer_TriggerZone : UdonSharpBehaviour
{
    public string correctObjectName; // The name of the correct object that needs to be placed on this pressure plate

    private void OnTriggerEnter(Collider other)
    {
        // Make sure the other collider is not null
        if (other == null || other.gameObject == null)
        {
            Debug.LogError("The collider or game object is null.");
            return;
        }

        // Check if the object entering the trigger has the correct name
        if (other.gameObject.name == correctObjectName)
        {
            Debug.Log("Correct object placed on pressure plate: " + other.gameObject.name);
            // Call a method or trigger an event here
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Make sure the other collider is not null
        if (other == null || other.gameObject == null)
        {
            Debug.LogError("The collider or game object is null.");
            return;
        }

        // Handle logic if the object is removed from the pressure plate
        if (other.gameObject.name == correctObjectName)
        {
            Debug.Log("Object removed from pressure plate: " + other.gameObject.name);
            // Call a method or trigger an event here
        }
    }
}
