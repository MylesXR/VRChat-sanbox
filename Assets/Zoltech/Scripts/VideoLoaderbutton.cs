
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonSharp.Video
{
    [DefaultExecutionOrder(10)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    [AddComponentMenu("Udon Sharp/Video/UI/Video Control Handler")]
    public class VideoLoaderbutton : UdonSharpBehaviour
    {

        public VideoControlHandler videoControlHandler;
        public string videoURLString;
        public VRCUrlInputField urlInputField;
        public VRC.SDK3.Components.VRCUrlInputField videoURL;
        
        public override void Interact()
        {
            Debug.LogError("interact");
            if (videoControlHandler != null && videoControlHandler.urlField != null)
            {
                urlInputField.textComponent.text = videoURLString;
                Debug.Log("string entered");
                videoControlHandler.OnURLInput();
            }
            else
            {
                Debug.LogError("VideoControlHandler or urlField is null. Make sure to assign the references in the Inspector.");
            }
        }
    }
}