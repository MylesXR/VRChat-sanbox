using UdonSharp;
using UnityEngine;

public class PotionCollisionHandler : UdonSharpBehaviour
{
    [SerializeField] GameObject potionBreakVFX; // Particle effect when the potion breaks
    private GameObject objectToDestroy;
    public DebugMenu debugMenu;

    public void SetObjectToDestroy(GameObject target)
    {
        objectToDestroy = target;
        if (debugMenu != null)
        {
            debugMenu.Log("Object to destroy set to: " + objectToDestroy.name);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (debugMenu != null)
        {
            debugMenu.Log("Potion has collided with: " + collision.gameObject.name);
        }

        if (collision.gameObject == objectToDestroy)
        {
            if (debugMenu != null)
            {
                debugMenu.Log("Potion collided with the destroyable object: " + objectToDestroy.name);
            }
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