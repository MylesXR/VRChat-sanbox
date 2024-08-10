using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRC_Head_Attachment : UdonSharpBehaviour
{
    public GameObject modelToAttach; // The model you want to attach

    void Start()
    {
        VRCPlayerApi playerApi = Networking.LocalPlayer; // Get the local player

        if (playerApi != null && modelToAttach != null)
        {
            // Get the head tracking data position and rotation
            Vector3 headPosition = playerApi.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            Quaternion headRotation = playerApi.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;

            // Set the model's position and rotation to match the head's
            modelToAttach.transform.position = headPosition;
            modelToAttach.transform.rotation = headRotation;

            // Optionally, you can add an offset if needed
            // modelToAttach.transform.position += new Vector3(0, 0.2f, 0); // Example offset upwards
        }
    }
}
