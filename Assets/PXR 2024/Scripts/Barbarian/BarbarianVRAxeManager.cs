using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BarbarianVRAxeManager : UdonSharpBehaviour
{
    public GameObject axePrefab;
    private GameObject currentAxe;
    private VRCPlayerApi localPlayer;
    private bool canGrabAxe = true;

    void Start()
    {
        localPlayer = Networking.LocalPlayer;
    }

    void Update()
    {
        if (currentAxe == null && canGrabAxe)
        {
            CheckForGrabInput();
        }
    }

    private void CheckForGrabInput()
    {
        if (localPlayer.IsUserInVR() && Input.GetButtonDown("Oculus_CrossPlatform_PrimaryHandTrigger"))
        {
            Collider[] hitColliders = Physics.OverlapSphere(localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position + new Vector3(0, 0.2f, -0.3f), 0.2f);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.name == "AxeGrabZone")
                {
                    SpawnAxe();
                    break;
                }
            }
        }
    }

    private void SpawnAxe()
    {
        currentAxe = Object.Instantiate(axePrefab);
        currentAxe.transform.position = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position + new Vector3(0, 0.2f, -0.3f);
        canGrabAxe = false;
    }

    public void AxeThrown()
    {
        Destroy(currentAxe, 5f);
        currentAxe = null;
        canGrabAxe = true;
    }
}
