using UdonSharp;
using UnityEngine;

public class PotionCollisionHandler : UdonSharpBehaviour
{
    [SerializeField] GameObject potionBreakVFX; // Particle effect when the potion breaks
    private GameObject objectToDestroy;

    public void SetObjectToDestroy(GameObject target)
    {
        objectToDestroy = target;
        Debug.Log("Object to destroy set to: " + objectToDestroy.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Potion has collided with: " + collision.gameObject.name);

        if (collision.gameObject == objectToDestroy)
        {
            Debug.Log("Potion collided with the destroyable object: " + objectToDestroy.name);
            Destroy(objectToDestroy);
        }

        if (potionBreakVFX != null)
        {
            GameObject vfxInstance = Instantiate(potionBreakVFX, transform.position, Quaternion.identity);
            Destroy(vfxInstance, 5f);
        }

        Destroy(gameObject);
    }
}
