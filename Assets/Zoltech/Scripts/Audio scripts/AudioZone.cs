using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace labthe3rd
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class AudioZone : UdonSharpBehaviour
    {
        [Header("Voice Values for Players Inside the Zone")]
        public float targetNear = 5;
        public float targetFar = 10;
        public float targetGain = 15;

        [Space]

        [Header("Default Voice Values in Your World")]
        public float defaultNear = 0;
        public float defaultFar = 25;
        public float defaultGain = 15;

        private VRCPlayerApi localplayer;
        private int localPlayerID;

        private int[] playersInZone = new int[50];
        private int playersInZoneCount = 0;

        private VRCPlayerApi[] allPlayers;

        void Start()
        {
            if (Utilities.IsValid(Networking.LocalPlayer))
            {
                localplayer = Networking.LocalPlayer;
                localPlayerID = Networking.LocalPlayer.playerId;
            }

            allPlayers = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            VRCPlayerApi.GetPlayers(allPlayers);
        }

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (Utilities.IsValid(player) && player.isLocal)
            {
                AddPlayerToZone(player.playerId);
                UpdateAudioSettings();
                RequestSerialization();
            }
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (Utilities.IsValid(player) && player.isLocal)
            {
                RemovePlayerFromZone(player.playerId);
                UpdateAudioSettings();
                RequestSerialization();
            }
        }

        private void AddPlayerToZone(int playerId)
        {
            for (int i = 0; i < playersInZoneCount; i++)
            {
                if (playersInZone[i] == playerId)
                {
                    return; // Player already in zone
                }
            }

            playersInZone[playersInZoneCount++] = playerId;
        }

        private void RemovePlayerFromZone(int playerId)
        {
            for (int i = 0; i < playersInZoneCount; i++)
            {
                if (playersInZone[i] == playerId)
                {
                    playersInZone[i] = playersInZone[--playersInZoneCount];
                    return;
                }
            }
        }

        private void UpdateAudioSettings()
        {
            // Get all players in the world
            VRCPlayerApi.GetPlayers(allPlayers);

            // Update settings for players inside the zone
            for (int i = 0; i < playersInZoneCount; i++)
            {
                VRCPlayerApi playerInZone = VRCPlayerApi.GetPlayerById(playersInZone[i]);
                if (Utilities.IsValid(playerInZone))
                {
                    playerInZone.SetVoiceDistanceFar(targetFar);
                    playerInZone.SetVoiceDistanceNear(targetNear);
                    playerInZone.SetVoiceGain(targetGain);
                }
            }

            if (IsPlayerInZone(localPlayerID))
            {
                // Set the voice distance far to 0.1 for outside players from the perspective of the local player inside the zone
                for (int i = 0; i < allPlayers.Length; i++)
                {
                    VRCPlayerApi playerOutsideZone = allPlayers[i];
                    if (!IsPlayerInZone(playerOutsideZone.playerId))
                    {
                        playerOutsideZone.SetVoiceDistanceFar(10f);
                    }
                }
            }
            else
            {
                // Reset settings for players outside the zone when the local player leaves the zone
                for (int i = 0; i < allPlayers.Length; i++)
                {
                    VRCPlayerApi player = allPlayers[i];
                    if (!IsPlayerInZone(player.playerId))
                    {
                        player.SetVoiceDistanceFar(defaultFar);
                        player.SetVoiceDistanceNear(defaultNear);
                        player.SetVoiceGain(defaultGain);
                    }
                }
            }
        }

        private bool IsPlayerInZone(int playerId)
        {
            for (int i = 0; i < playersInZoneCount; i++)
            {
                if (playersInZone[i] == playerId)
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            allPlayers = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            VRCPlayerApi.GetPlayers(allPlayers);

            UpdateAudioSettings();
            RequestSerialization();
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            RemovePlayerFromZone(player.playerId);
            allPlayers = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            VRCPlayerApi.GetPlayers(allPlayers);

            UpdateAudioSettings();
            RequestSerialization();
        }

        public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
        {
            return true;
        }
    }
}
