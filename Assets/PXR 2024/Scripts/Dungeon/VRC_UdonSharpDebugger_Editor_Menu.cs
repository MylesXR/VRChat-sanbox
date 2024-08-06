/*using UnityEngine;
using UnityEditor;
using VRC.Udon;
using UdonSharp;

public class VRC_UdonSharpDebugger_Editor_Menu : EditorWindow
{
    private string instanceIdToSearch = "";

    [MenuItem("Tools/Enhanced UdonSharp Debugger")]
    public static void ShowWindow()
    {
        GetWindow<VRC_UdonSharpDebugger_Editor_Menu>("Enhanced UdonSharp Debugger");
    }

    private void OnGUI()
    {
        instanceIdToSearch = EditorGUILayout.TextField("Instance ID to search for:", instanceIdToSearch);

        if (GUILayout.Button("Check All UdonSharp References in Scene"))
        {
            CheckAllReferencesInScene();
        }

        if (GUILayout.Button("Find Specific Instance ID in Scene"))
        {
            if (int.TryParse(instanceIdToSearch, out int instanceId))
            {
                FindSpecificInstanceIDInScene(instanceId);
            }
            else
            {
                Debug.LogError("Please enter a valid instance ID.");
            }
        }
    }

    private void CheckAllReferencesInScene()
    {
        Debug.Log("Starting to check all UdonSharp references in the scene...");

        // Check active and inactive objects in the current scene
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.hideFlags == HideFlags.NotEditable || obj.hideFlags == HideFlags.HideAndDontSave)
                continue;

            UdonBehaviour[] udonBehaviours = obj.GetComponents<UdonBehaviour>();
            foreach (UdonBehaviour behaviour in udonBehaviours)
            {
                if (behaviour.programSource == null)
                {
                    Debug.LogError($"GameObject '{obj.name}' (Instance ID: {obj.GetInstanceID()}) has a null UdonSharpProgramAsset reference.");
                }
                else
                {
                    Debug.Log($"GameObject '{obj.name}' (Instance ID: {obj.GetInstanceID()}) has UdonSharpProgramAsset '{behaviour.programSource.name}' assigned.");
                }
            }
        }

        Debug.Log("Finished listing all instance IDs in the scene.");
    }

    private void FindSpecificInstanceIDInScene(int instanceId)
    {
        Debug.Log($"Starting search for instance ID: {instanceId} in the scene");

        // Check active and inactive objects in the current scene
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.hideFlags == HideFlags.NotEditable || obj.hideFlags == HideFlags.HideAndDontSave)
                continue;

            if (LogSpecificGameObjectInstanceIDs(obj, instanceId))
            {
                return;
            }
        }

        Debug.Log($"Instance ID: {instanceId} not found in the scene.");
    }

    private bool LogSpecificGameObjectInstanceIDs(GameObject obj, int instanceId)
    {
        if (obj.GetInstanceID() == instanceId)
        {
            UdonBehaviour[] udonBehaviours = obj.GetComponents<UdonBehaviour>();
            foreach (UdonBehaviour behaviour in udonBehaviours)
            {
                if (behaviour.programSource == null)
                {
                    Debug.LogError($"Found GameObject '{obj.name}' (Instance ID: {instanceId}) with a null UdonSharpProgramAsset reference.");
                }
                else
                {
                    Debug.Log($"Found GameObject '{obj.name}' (Instance ID: {instanceId}) with UdonSharpProgramAsset '{behaviour.programSource.name}' assigned.");
                }
            }
            return true;
        }
        foreach (Transform child in obj.transform)
        {
            if (LogSpecificGameObjectInstanceIDs(child.gameObject, instanceId))
            {
                return true;
            }
        }
        return false;
    }
}
*/