
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GlyphObjectManager : UdonSharpBehaviour
{
    public bool isExplorer;
    public GameObject[] nonReadable;
    public GameObject[] readable;

    void Start()
    {
        isExplorer = false;
        ChangeGlyph();
    }

    public void SetAsExplorer()
    {
        isExplorer = true;
        ChangeGlyph();
    }

    public void SetAsNotExplorer()
    {
        isExplorer = false;
        ChangeGlyph();
    }

    public void ChangeGlyph()
    {
        if(isExplorer == true)
        {
            foreach (GameObject obj in readable)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in nonReadable)
            {
                obj.SetActive(false);
            }
        }
        if(isExplorer == false)
        {
            foreach (GameObject obj in readable)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in nonReadable)
            {
                obj.SetActive(true);
            }
        }
    }
}
