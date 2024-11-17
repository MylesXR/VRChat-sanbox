using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Advertisment_Changing_to_Zoltech : UdonSharpBehaviour
{
    [Header("Objects to Change")]
    [Tooltip("List of objects whose materials will be changed.")]
    public Renderer[] targetObjects;

    [Header("Material to Apply")]
    [Tooltip("The material to apply to all target objects.")]
    public Material newMaterial;

    [UdonSynced(UdonSyncMode.None)] // Syncs the material application across the network
    private bool isMaterialApplied;

    private void Start()
    {
        ApplyMaterial(isMaterialApplied); // Ensure the correct material state on instance join
    }

    public override void Interact()
    {
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject); // Take ownership to make changes
        }

        isMaterialApplied = true; // Indicate that the material is applied
        ApplyMaterial(isMaterialApplied);
        RequestSerialization(); // Sync the material application across the network
    }

    private void ApplyMaterial(bool apply)
    {
        if (!apply || newMaterial == null)
        {
            Debug.LogWarning("Material application skipped: either no material assigned or apply flag is false.");
            return;
        }

        foreach (Renderer renderer in targetObjects)
        {
            if (renderer != null)
            {
                Material[] currentMaterials = renderer.materials; // Get current materials
                for (int i = 0; i < currentMaterials.Length; i++)
                {
                    currentMaterials[i] = newMaterial; // Replace each material with the new one
                }
                renderer.materials = currentMaterials; // Apply changes back to the renderer
            }
        }
    }

    public override void OnDeserialization()
    {
        // Update the material state for everyone when the synced variable changes
        ApplyMaterial(isMaterialApplied);
    }
}