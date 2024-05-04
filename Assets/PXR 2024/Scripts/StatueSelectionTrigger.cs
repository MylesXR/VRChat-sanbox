using UdonSharp;
using UnityEngine;

public class StatueSelectionTrigger : UdonSharpBehaviour
{
    public GameObject targetObject1; // The GameObject to be toggled
    public GameObject targetObject2; // The GameObject to be toggled
    public GameObject targetObject3; // The GameObject to be toggled
    public GlyphObjectManager GlyphObjectManager;
    public int thisObjectValue;

    private void OnPlayerTriggerEnter()
    {
        ToggleObject();
    }

    public void ToggleObject()
    {
        if (thisObjectValue == 1)
        {
            targetObject1.SetActive(true);
            targetObject2.SetActive(false);
            targetObject3.SetActive(false);
            //GlyphObjectManager.SetAsExplorer();
        }
        if (thisObjectValue == 2)
        {
            targetObject1.SetActive(false);
            targetObject2.SetActive(true);
            targetObject3.SetActive(false);
            //GlyphObjectManager.SetAsNotExplorer();
        }
        if (thisObjectValue == 3)
        {
            targetObject1.SetActive(false);
            targetObject2.SetActive(false);
            targetObject3.SetActive(true);
            //GlyphObjectManager.SetAsNotExplorer();
        }
    }
}
