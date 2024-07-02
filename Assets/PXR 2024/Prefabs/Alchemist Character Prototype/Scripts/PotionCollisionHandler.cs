using UdonSharp;
using UnityEngine;

public class PotionCollisionHandler : UdonSharpBehaviour
{
    private GameObject objectToDestroy;

    private void Start()
    {
        // Ensure the Rigidbody is initially kinematic
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    public void SetObjectToDestroy(GameObject target)
    {
        objectToDestroy = target;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only destroy the object if this collision is with the specific object
        if (collision.gameObject == objectToDestroy)
        {
            Debug.Log("Potion collided with the destroyable object.");
            Destroy(objectToDestroy);
        }
    }

    // Method to make the potion non-kinematic when picked up
    public void OnPickup()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}
