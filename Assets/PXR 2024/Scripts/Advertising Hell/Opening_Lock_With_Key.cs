using UdonSharp;
using UnityEngine;

public class Opening_Lock_With_Key : UdonSharpBehaviour
{
    public GameObject[] keyObjects; // Array to store multiple key objects
    public Animator lockAnimator;
    public string animationTriggerName = "PlayAnimation"; // Name of the animation trigger

    private void OnTriggerEnter(Collider other)
    {
        // Check if lockAnimator is assigned
        if (lockAnimator == null)
        {
            Debug.LogWarning("Lock animator is not assigned.");
            return;
        }

        foreach (GameObject keyObject in keyObjects)
        {
            if (other.gameObject == keyObject)
            {
                lockAnimator.SetTrigger(animationTriggerName);
                keyObject.SetActive(false); 
                break;
            }
        }
    }
}
