using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Player
{
    public class TrackedObject : UdonSharpBehaviour
    {
        [Tooltip("Which tracking point to attach this object to.")]
        public VRCPlayerApi.TrackingDataType trackingType;

        private bool editorMode = true;
        private VRCPlayerApi localPlayer;

        public void Start()
        {
            localPlayer = Networking.LocalPlayer;

            if (localPlayer != null)
            {
                editorMode = false;
            }
        }

        public void Update()
        {
            if (!editorMode && Networking.IsOwner(localPlayer, gameObject))
            {
                VRCPlayerApi.TrackingData data = localPlayer.GetTrackingData(trackingType);
                transform.SetPositionAndRotation(data.position, data.rotation);
            }
        }
    }
}
