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

        [UdonSynced] private bool isAxeVisible = false;

        public override void Interact()
        {
            if (Networking.IsOwner(gameObject))
            {
                isAxeVisible = !isAxeVisible;
                RequestSerialization();
                UpdateAxeVisibility();
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                Interact();
            }
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
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }

            if (localAxe == null)
            {
                localAxe = VRCInstantiate(axePrefab);
                localAxe.transform.SetParent(transform);
                localAxe.transform.localPosition = Vector3.zero;
                localAxe.transform.localRotation = Quaternion.identity;
                UpdateAxeVisibility();
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

        public override void OnDeserialization()
        {
            UpdateAxeVisibility();
        }
    }
}
