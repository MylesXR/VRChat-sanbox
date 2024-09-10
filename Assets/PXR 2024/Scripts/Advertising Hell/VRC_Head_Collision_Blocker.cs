
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRC_Head_Collision_Blocker : UdonSharpBehaviour
{
    public GameObject playerHead; // Assign this in the inspector
    private Collider headCollider;

    void Start()
    {
        // Get the collider attached to the player's head
        headCollider = playerHead.GetComponent<Collider>();

        if (headCollider == null)
        {
            Debug.LogError("No collider found on the player's head!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // This method will be called when the head enters any trigger collider
        if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Debug.Log("Head entered environment layer trigger.");
            // Add any additional logic you want to handle here
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // This method will be called when the head exits the trigger collider
        if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Debug.Log("Head exited environment layer trigger.");
            // Add any additional logic you want to handle here
        }
    }
}
