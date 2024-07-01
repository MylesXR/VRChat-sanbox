
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ExplorerManager : UdonSharpBehaviour
{
    public GlyphObjectManager GlyphObjectManager;
    private VRCPlayerApi localPlayer;
    //public GameObject grapplePC;
    //public GameObject grappleVR;
    void Start()
    {
        localPlayer = Networking.LocalPlayer;
    }
    private void OnEnable()
    {
        if (localPlayer != null && localPlayer.isLocal)
        {
            Networking.SetOwner(localPlayer, gameObject);
            GlyphObjectManager.SetAsExplorer();
        }
        //GlyphObjectManager.SetAsExplorer();
        //grapplePC.SetActive(true);
        //grappleVR.SetActive(true);
    }
    private void OnDisable()
    {
        if (localPlayer != null && localPlayer.isLocal)
        {
            Networking.SetOwner(localPlayer, gameObject);
            GlyphObjectManager.SetAsNotExplorer();
        }
        //GlyphObjectManager.SetAsNotExplorer();
        //grapplePC.SetActive(false);
        //grappleVR.SetActive(false);
    }
}
