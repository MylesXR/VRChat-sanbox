
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ExplorerManager : UdonSharpBehaviour
{
    public GlyphObjectManager GlyphObjectManager;
    void Start()
    {
        
    }
    private void OnEnable()
    {
        GlyphObjectManager.SetAsExplorer();
    }
    private void OnDisable()
    {
        GlyphObjectManager.SetAsNotExplorer();
    }
}
