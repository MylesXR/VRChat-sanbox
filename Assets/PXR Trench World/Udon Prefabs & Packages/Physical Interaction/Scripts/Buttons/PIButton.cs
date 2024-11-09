using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PIButton : UdonSharpBehaviour
{
    [Tooltip("Game object to toggle")]
    [SerializeField]
    private GameObject toggleObject;

    [Tooltip("More game objects to toggle")]
    [SerializeField]
    private GameObject[] toggleObjects;

    [Space(10)]
    [Tooltip("Target script")]
    [SerializeField]
    private UdonBehaviour target;

    [Tooltip("Toggle boolean property on the target script")]
    [SerializeField]
    private string toggleProperty;

    [Tooltip("Calling method (custom event) on the target script")]
    [SerializeField]
    private string callMethod;

    [Space(10)]
    [Tooltip("0 - Off, 1 - On, else Toggle")]
    [SerializeField]
    private int operation = -1;

    [Space(10)]
    [Tooltip("Audio when button is pressed")]
    [SerializeField]
    private AudioSource pressAudio;

    private VRCPlayerApi _localPlayer;

    private void Start()
    {
        _localPlayer = Networking.LocalPlayer;
    }

    private void OnMouseDown()
    {
        Interact(); // No need for localPlayer check here.
    }

    public override void Interact()
    {
        if (_localPlayer == Networking.LocalPlayer) // Only the local player can interact
        {
            DoInteract();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "NetworkedInteract");
        }
    }

    public void NetworkedInteract()
    {
        if (_localPlayer != Networking.LocalPlayer) // Prevent the local player from replaying the action
        {
            DoInteract();
        }
    }

    public void DoInteract()
    {
        PlayAudio(pressAudio);

        if (toggleObject != null)
        {
            SetActiveState(toggleObject);
        }

        if (toggleObjects != null && toggleObjects.Length > 0)
        {
            foreach (var obj in toggleObjects)
            {
                if (obj != null)
                {
                    SetActiveState(obj);
                }
            }
        }

        if (target != null)
        {
            if (!string.IsNullOrEmpty(toggleProperty))
            {
                bool currentState = (bool)target.GetProgramVariable(toggleProperty);
                bool newState = GetNewState(currentState);
                target.SetProgramVariable(toggleProperty, newState);
            }

            if (!string.IsNullOrEmpty(callMethod))
            {
                target.SendCustomEvent(callMethod);
            }
        }
    }

    private void PlayAudio(AudioSource audio)
    {
        if (audio != null)
        {
            audio.Play();
        }
    }

    private void SetActiveState(GameObject obj)
    {
        bool newState = GetNewState(obj.activeSelf);
        obj.SetActive(newState);
    }

    private bool GetNewState(bool currentState)
    {
        if (operation == 0)
        {
            return false;
        }
        else if (operation == 1)
        {
            return true;
        }
        else
        {
            return !currentState;
        }
    }
}
