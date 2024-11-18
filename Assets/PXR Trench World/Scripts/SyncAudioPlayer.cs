
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;


public class SyncAudioPlayer : UdonSharpBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] playList;
    
    [Tooltip("How often master syncs audio in seconds")]
    public int syncDuration = 5;
    
    [Tooltip("How far out of sync in seconds client can be before forced resync")]
    public int syncThreshold = 3;

    [UdonSynced] private string masterSyncInfo;
    private string prevMasterSyncInfo;
    private int masterSongIndex = 0;
    private float masterSongTimestamp = 0.0f;
    
    private float localSongTimestamp;
    private float localSongLength;
    private int localSongIndex = 0;
    
    private float nextActionTime = 0.0f; // Sync timer kicks off immediately
    private bool audioInitialized = false;

    /*
     * Changes tracks after a song finishes (auto loops playlist)
     * Also master updates sync data
     */

    private void Update()
    {
        if (Networking.GetOwner(gameObject) != Networking.LocalPlayer)
            return;

        if (audioSource == null || playList == null || playList.Length == 0)
            return;

        if (Time.time >= nextActionTime)
        {
            nextActionTime = Time.time + syncDuration;

            localSongTimestamp = audioSource.time;

            if (Networking.IsMaster)
            {
                masterSongTimestamp = localSongTimestamp;  // Updating master timestamp here
                string netData = localSongIndex + "|" + masterSongTimestamp;
                masterSyncInfo = netData;
                //Debug.Log("Update - Master Timestamp (Master): " + masterSongTimestamp);
            }
            else
            {
                //Debug.Log("Update - Local Timestamp (Client): " + localSongTimestamp);
            }
        }

        // Only proceed if audio is initialized
        if (!audioInitialized)
            return;

        // Check if the audio has stopped playing
        if (!audioSource.isPlaying)
        {
            localSongIndex = (localSongIndex + 1) % playList.Length;
            audioSource.clip = playList[localSongIndex];
            audioSource.Play();
        }
    }

    public override void OnDeserialization()
    {
        if (Networking.IsMaster || masterSyncInfo == null)
            return;

        if (prevMasterSyncInfo == masterSyncInfo)
            return;

        string[] masterSyncInfoArray = masterSyncInfo.Split('|');
        if (masterSyncInfoArray.Length != 2)
            return;

        if (!int.TryParse(masterSyncInfoArray[0], out masterSongIndex))
            return;

        if (!float.TryParse(masterSyncInfoArray[1], out masterSongTimestamp))
            return;

        prevMasterSyncInfo = masterSyncInfo;

        //Debug.Log("OnDeserialization - Master Timestamp: " + masterSongTimestamp);  // Debug for master timestamp

        localSongTimestamp = audioSource.time;

        int syncCase = -1;

        if (masterSongIndex != localSongIndex)
        {
            localSongLength = playList[localSongIndex].length;

            if (localSongLength - localSongTimestamp > syncThreshold)
            {
                syncCase = 1;
            }
        }
        else if (Mathf.Abs(masterSongTimestamp - localSongTimestamp) > syncThreshold)
        {
            syncCase = 1;
        }

        if (syncCase == 1)
        {
            localSongIndex = masterSongIndex;
            audioSource.clip = playList[masterSongIndex];
            audioSource.time = masterSongTimestamp;
            audioSource.Play();
            audioInitialized = true;
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.IsMaster)
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        if (audioSource == null || playList == null || playList.Length == 0 || Networking.LocalPlayer != player)
            return;

        if (!audioInitialized)
        {
            if (Networking.IsMaster)
            {
                audioSource.clip = playList[0];
                audioSource.Play();
                audioInitialized = true;
            }
        }
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //BELOW THIS WILL CONTAIN CODE FOR THE BUTTONS USED IN THE TRENCH WORLD

    //[SerializeField] private GameObject mirrorGameObject;
    private bool isMirrorActive; // Holds the current state of the mirror
    private float toggleCooldown = 0.5f; // Time in seconds before the mirror can be toggled again
    private float lastToggleTime; // Stores the time the mirror was last toggled

    private void Start()
    {
        isMirrorActive = false; // Initialize mirror state to inactive
        ////mirrorGameObject.SetActive(isMirrorActive); // Set the GameObject to the initial state
        lastToggleTime = Time.time; // Initialize last toggle time

        if (Networking.IsMaster)
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
    }

    public void ToggleMirror() // Public as it might be called from other scripts or UI elements
    {
        // Check if enough time has passed since the last toggle
        if (Time.time - lastToggleTime < toggleCooldown)
        {
            return; // Exit if not enough time has passed
        }

        isMirrorActive = !isMirrorActive; // Toggle the state
        //mirrorGameObject.SetActive(isMirrorActive); // Update the GameObject's active state

        lastToggleTime = Time.time; // Update the last toggled time
    }

    public void FullVolume()
    {
        audioSource.volume = 0.8f;
        
    }

    public void MediumVolume()
    {
        audioSource.volume = 0.4f;
    }

    public void LowVolume()
    {
        audioSource.volume = 0.1f;
    }

    public void RestartAllAudioSources()
    {
        //Debug.Log($"Master Sync Info: {masterSyncInfo}");
        //Debug.Log("RestartAllAudioSources - Master Timestamp: " + masterSongTimestamp);  // Debug for master timestamp
        //Debug.Log("RestartAllAudioSources - Local Timestamp: " + audioSource.time);  // Debug for local timestamp
        //Debug.Log($"Is Master: {Networking.IsMaster}");
        

        // Save the current time position of the audio
        float savedTime = audioSource.time;

        // Synchronize audio clip with the master's
        if (masterSongIndex != localSongIndex && masterSongIndex >= 0 && masterSongIndex < playList.Length)
        {
            audioSource.Stop(); // Stop the audio if the clip has changed
            audioSource.clip = playList[masterSongIndex];
            localSongIndex = masterSongIndex;
            audioSource.time = masterSongTimestamp;
            audioSource.Play(); // Restart the audio if the clip has changed
        }
        else
        {
            // If the clip hasn't changed, just adjust the time to sync with the master
            if (Mathf.Abs(savedTime - masterSongTimestamp) > 0.1f)
            {
                audioSource.time = masterSongTimestamp;
            }
        }

        // Flag as initialized
        audioInitialized = true;
    }

    public override void Interact()
    {
        RestartAllAudioSources();
    }

}