using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BarbarianManager : UdonSharpBehaviour
{
    public BarbarianObjectManager barbarianObjectManager;
    [Tooltip("Whether this TrackedObject should be active if you are in desktop mode or VR mode.")]
    public bool vrEnabled;
    [Tooltip("Which GameObject to enable if vrEnabled matches which mode we are in.")]
    private VRCPlayerApi localPlayer;

    public GameObject vrEnabledObject;
    public GameObject pcEnabledObject;


    void Start()
    {
        vrEnabledObject.SetActive(false);
        pcEnabledObject.SetActive(false);

        localPlayer = Networking.LocalPlayer;
        vrEnabled = localPlayer.IsUserInVR();
    }

    private void OnEnable()
    {
        if (localPlayer != null && localPlayer.isLocal)
        {
            Networking.SetOwner(localPlayer, gameObject);
            //barbarianObjectManager.SetAsBarbarian();
        }
        //if (localPlayer != null && vrEnabled == true && localPlayer.isLocal)
        //{
        //    vrEnabledObject.SetActive(true);
        //}
        //if (localPlayer != null && vrEnabled == false && localPlayer.isLocal)
        //{
        //    pcEnabledObject.SetActive(true);
        //}
    }

    private void OnDisable()
    {
        if (localPlayer != null && localPlayer.isLocal)
        {
            //barbarianObjectManager.SetAsNotBarbarian();
        }
        //if (localPlayer != null && localPlayer.isLocal)
        //{
        //    pcEnabledObject.SetActive(false);
        //    vrEnabledObject.SetActive(false);
        //}
    }
}
