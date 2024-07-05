using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Player
{
    public class atendeetoggleon : UdonSharpBehaviour
    {
        [Tooltip("State of game objects when scene is loaded.")]
        public bool initialState = false;
        [Tooltip("Key that toggles gameobjects.")]
        public KeyCode key;
        [Tooltip("Menu game object to toggle on/off.")]
        public GameObject menuObject;
        [Tooltip("Return point game object.")]
        public GameObject returnPointObject;
        [Tooltip("Maximum distance before menu returns to the return point.")]
        public float maxDistance = 10.0f;

        private Vector3 initialPosition;
        private Quaternion initialRotation;

        private void Start()
        {
            initialPosition = menuObject.transform.position;
            initialRotation = menuObject.transform.rotation;

            menuObject.SetActive(initialState);
            returnPointObject.SetActive(initialState);
        }

        private void Update()
        {
            if (Input.GetKeyDown(key))
            {
                bool isActive = !menuObject.activeSelf;
                returnPointObject.SetActive(isActive);

                if (isActive)
                {
                    menuObject.SetActive(true);
                    // Move the menu to the return point
                    menuObject.transform.position = returnPointObject.transform.position;
                    menuObject.transform.rotation = returnPointObject.transform.rotation;

                    // Detach menu from parent
                    menuObject.transform.SetParent(null);
                }
                else
                {
                    menuObject.SetActive(false);
                }
            }

            CheckDistanceAndToggleOff();
        }

        private void CheckDistanceAndToggleOff()
        {
            if (menuObject.activeSelf)
            {
                float distance = Vector3.Distance(menuObject.transform.position, returnPointObject.transform.position);
                if (distance > maxDistance)
                {
                    menuObject.SetActive(false);
                    menuObject.transform.position = initialPosition;
                    menuObject.transform.rotation = initialRotation;
                    returnPointObject.SetActive(false);
                }
            }
        }
    }
}
