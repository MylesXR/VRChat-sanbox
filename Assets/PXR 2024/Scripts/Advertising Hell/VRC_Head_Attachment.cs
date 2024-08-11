using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRC_Head_Attachment : UdonSharpBehaviour
{
    public GameObject Tag; // The object to attach to the head
    public float heightOffset = 0.55f; // Public variable to adjust the height in the Inspector

    Vector3 initTagPos;
    private int playerId;

    void Start()
    {
        initTagPos = Tag.transform.position;
        if (Networking.LocalPlayer != null)
        {
            Debug.Log("Local player is not null");
        }

        playerId = VRCPlayerApi.GetPlayerId(Networking.LocalPlayer);
    }

    void Update()
    {
        if (VRCPlayerApi.GetPlayerById(playerId) != null)
        {
            Vector3 pos = VRCPlayerApi.GetPlayerById(playerId).GetBonePosition(HumanBodyBones.Head);

            // Apply the height offset here
            pos = new Vector3(pos.x, pos.y + heightOffset, pos.z);

            Tag.transform.position = pos;
        }
        else
        {
            Tag.transform.position = initTagPos;
        }
    }
}
