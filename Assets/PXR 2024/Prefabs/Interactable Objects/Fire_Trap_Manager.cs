
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Fire_Trap_Manager : UdonSharpBehaviour
{
    public GameObject[] FireTraps;

    public void Interact()
    {
        if (FireTraps == null || FireTraps.Length == 0)
        {
            Debug.LogError("FireTraps array is not set or is empty.");
            return;
        }

        for (int i = 0; i < FireTraps.Length; i++)
        {
            if (FireTraps[i] != null)
            {
                FireTraps[i].SetActive(false);
            }
            else
            {
                Debug.LogWarning($"FireTrap at index {i} is null.");
            }
        }
    }
}
