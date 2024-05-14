
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Player
{
    /// <summary>
    /// Basic gameobject toggle on key press script.
    /// </summary>
    public class KeyToggleSynced : UdonSharpBehaviour
    {
        [Tooltip("State of game objects when scene is loaded.")]
        [UdonSynced] //Synchronized variable to keep track of object's active state
        public bool initialState = false;
        [Tooltip("Key that toggles gameobjects.")]
        public KeyCode key;
        [Tooltip("List of game objects to toggle on/off.")]
        public GameObject[] toggleObject;

        public void Start()
        {
            for (int i = 0; i < toggleObject.Length; i++)
            {
                toggleObject[i].SetActive(initialState);
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(key))
            {
                for (int i = 0; i < toggleObject.Length; i++)
                {
                    toggleObject[i].SetActive(!toggleObject[i].activeSelf);
                }
            }
        }
        public override void OnDeserialization() //to do, check if object is synced on deserialization. 
        {
            // Called when the UdonSynced variable is updated
            // Update the object's state based on the synced variable
            if (toggleObject != null)
            {
                for (int i = 0; i < toggleObject.Length; i++)
                {
                    toggleObject[i].SetActive(initialState);
                }
            }
        }
    }
}