
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace labthe3rd
{
    public class PlayerSpeakAudioZones : UdonSharpBehaviour
    {
        public GameObject toggleTest;
        public VoiceBooster voiceBooster;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (player.isLocal)
            {
            voiceBooster.Trigger();
            toggleTest.SetActive(true);
            }
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (player.isLocal)
            {
            voiceBooster.Trigger();
            toggleTest.SetActive(false);
            }
        }
    }
}

