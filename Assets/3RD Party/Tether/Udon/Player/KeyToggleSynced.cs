using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Player
{
    public class KeyToggleSynced : UdonSharpBehaviour
    {
        [Tooltip("Key that toggles gameobjects.")]
        public KeyCode key;

        [Tooltip("Prefab of the axe to instantiate for each player.")]
        public GameObject axePrefab;

        private GameObject playerAxe;
        private bool isAxeVisible = false;

        private VRCPlayerApi localPlayer;

        public void Start()
        {
            localPlayer = Networking.LocalPlayer;

            // Instantiate the axe for the local player
            playerAxe = Object.Instantiate(axePrefab);
            //playerAxe.transform.SetParent(localPlayer.gameObject.transform, false);
            playerAxe.SetActive(isAxeVisible);
        }

        public void Update()
        {
            if (localPlayer != null && localPlayer.IsUserInVR() && Input.GetKeyDown(key))
            {
                isAxeVisible = !isAxeVisible;
                RequestSerialization(); // Sync the change across the network
                UpdateAxeVisibility();
            }
        }

        public override void OnDeserialization()
        {
            // Update the object's state based on the synced variable
            UpdateAxeVisibility();
        }

        private void UpdateAxeVisibility()
        {
            if (playerAxe != null)
            {
                playerAxe.SetActive(isAxeVisible);
            }
        }
    }
}
