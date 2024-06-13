using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Player
{
    public class BarbarianAxeController : UdonSharpBehaviour
    {
        [Tooltip("Key that toggles the visibility of the axe.")]
        public KeyCode toggleKey;

        [Tooltip("Prefab of the axe.")]
        public GameObject axePrefab;

        private GameObject localAxe;
        private bool isAxeVisible = false;

        public void Update()
        {
            if (Networking.LocalPlayer != null && Networking.LocalPlayer.isLocal)
            {
                if (Input.GetKeyDown(toggleKey))
                {
                    ToggleAxeVisibility();
                }
            }
        }

        private void ToggleAxeVisibility()
        {
            isAxeVisible = !isAxeVisible;
            UpdateAxeVisibility();
        }

        private void UpdateAxeVisibility()
        {
            if (localAxe != null)
            {
                localAxe.SetActive(isAxeVisible);
            }
        }

        private void OnEnable()
        {
            if (Networking.LocalPlayer != null && Networking.LocalPlayer.isLocal)
            {
                if (localAxe == null)
                {
                    localAxe = Object.Instantiate(axePrefab);
                    localAxe.transform.SetParent(transform);
                    localAxe.transform.localPosition = Vector3.zero;
                    localAxe.transform.localRotation = Quaternion.identity;
                    UpdateAxeVisibility();
                }
            }
        }

        private void OnDisable()
        {
            if (localAxe != null)
            {
                Destroy(localAxe);
                localAxe = null;
            }
        }
    }
}
