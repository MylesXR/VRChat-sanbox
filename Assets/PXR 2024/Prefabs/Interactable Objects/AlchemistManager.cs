
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AlchemistManager : UdonSharpBehaviour
{
    public AlchemistObjectManager alchemistObjectManager;

    [Tooltip("Which GameObject to enable if vrEnabled matches which mode we are in.")]
    private VRCPlayerApi localPlayer;



    void Start()
    {
        localPlayer = Networking.LocalPlayer;

    }

    private void OnEnable()
    {
        if (localPlayer != null && localPlayer.isLocal)
        {
            Networking.SetOwner(localPlayer, gameObject);
            alchemistObjectManager.SetAsAlchemist();
        }

    }

    private void OnDisable()
    {
        if (localPlayer != null && localPlayer.isLocal)
        {
            alchemistObjectManager.SetAsNotAlchemist();
        }

    }
}
