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

        public override void Interact()
        {
            if (Networking.IsOwner(gameObject))
            {
                isAxeVisible = !isAxeVisible;
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
            localAxe = Object.Instantiate(axePrefab);
            localAxe.SetActive(isAxeVisible);
            localAxe.transform.SetParent(transform);
            localAxe.transform.localPosition = Vector3.zero;
            localAxe.transform.localRotation = Quaternion.identity;
        }
        private void OnDisable()
        {
            Destroy(localAxe);
        }

    }
}
