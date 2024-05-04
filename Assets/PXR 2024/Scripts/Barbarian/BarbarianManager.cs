
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BarbarianManager : UdonSharpBehaviour
{
    public BarbarianObjectManager barbarianObjectManager;
    void Start()
    {
        
    }

    private void OnEnable()
    {
        barbarianObjectManager.SetAsBarbarian();
    }

    private void OnDisable()
    {
        barbarianObjectManager.SetAsNotBarbarian();
    }
}
