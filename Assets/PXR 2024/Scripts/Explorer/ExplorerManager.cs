
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ExplorerManager : UdonSharpBehaviour
{
    public GlyphObjectManager GlyphObjectManager;
    public GameObject grapplePC;
    public GameObject grappleVR;
    void Start()
    {

    }
    private void OnEnable()
    {
        GlyphObjectManager.SetAsExplorer();
        grapplePC.SetActive(true);
        grappleVR.SetActive(true);
    }
    private void OnDisable()
    {
        GlyphObjectManager.SetAsNotExplorer();
        grapplePC.SetActive(false);
        grappleVR.SetActive(false);
    }
}
