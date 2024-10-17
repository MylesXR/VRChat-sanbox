
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class OnTriggerToggleObjectOn : UdonSharpBehaviour
{
    public GameObject objectToToggle;
    void Start()
    {

    }
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            ToggleObject();
        }
    }
    public void ToggleObject()
    {
        objectToToggle.SetActive(true);
    }
}
