using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
/// <summary>
/// 
/// Please credit Reimajo if you use this source code
/// 
/// You need to put this script on an empty GameObject under the vehicle, else your vehicle won't animate.
/// The box collider also needs to be alone on an seperated empty gameobject and set as trigger.
/// 
/// </summary>
public class VehicleMovesPlayer : UdonSharpBehaviour
{
    //a panel with a text script on it, to write debug stuff onto
    //remove this before publishing, it is for debugging only
    ///public GameObject textPanelObject = null;
    //walls that will be activated when the player is inside the vehicle, not needed actually
    ///public GameObject activatedWhenPlayerIsInVehicle = null;
    //an empty(!) gameObject under the vehicle that has a box collider on it (set as trigger) 
    //to mark an area in which the player is "inside" the vehicle
    public GameObject vehicleBoundsBoxCollider = null;
    //position of vehicle from the last frame
    Vector3 lastFramePosition;
    //rotation of vehicle from the last frame
    Quaternion lastFrameRotation;
    //rotation of vehicle since last frame
    Quaternion stationRotation;
    //movement of vehicle since last frame
    Vector3 stationMovement;
    //new player rotation
    Quaternion newPlayerRotation;
    //current player rotation before changes
    Vector3 currentPlayerPosition;
    //new player position
    Vector3 newPlayerPosition;
    //used to store the vehicle box collider
    Collider vehicleCollider;
    //reference to the local player
    VRCPlayerApi localPlayer;
    /// <summary>
    /// This function will be called once when the world loads
    /// </summary>
    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        lastFramePosition = transform.position; //just to prevent nullpointer exceptions
        lastFrameRotation = transform.rotation; //just to prevent nullpointer exceptions
        vehicleCollider = vehicleBoundsBoxCollider.GetComponent<Collider>(); //getting the box collider from the vehicle
        ///activatedWhenPlayerIsInVehicle.SetActive(false); //disable vehicle walls
        //remove this before publishing, it is for debugging only
        ///textPanelObject.GetComponent<Text>().text = "Script loaded successfully"; //just to confirm that the script was loaded
    }
    /// <summary>
    /// This function will be called once every frame
    /// </summary>
    private void Update()
    {
        //read the current player position and store it
        currentPlayerPosition = localPlayer.GetPosition();
        //just for debugging, uncomment before publishing
        ///textPanelObject.GetComponent<Text>().text = "x:"+currentPlayerPosition.x.ToString()+", y:"+ currentPlayerPosition.y.ToString() + ", z:" + currentPlayerPosition.z.ToString();
        {
            if (vehicleCollider.bounds.Contains(currentPlayerPosition))
            {
                //remove this before publishing, it is for debugging only
                ///textPanelObject.GetComponent<Text>().text = "player in station";
                //enables the vehicle walls
                ///activatedWhenPlayerIsInVehicle.SetActive(true);
                //calculate the station movement since last frame
                stationMovement = transform.position - lastFramePosition;
                //calculate the station rotation since last frame
                stationRotation = transform.rotation * Quaternion.Inverse(lastFrameRotation);
                //moving the player with the vehicle movement
                newPlayerPosition = currentPlayerPosition + stationMovement;
                //preserving the player rotation
                newPlayerRotation = localPlayer.GetRotation() * Quaternion.Inverse(stationRotation);
                //teleport player since you can't change the position/rotation directly afaik
                
                localPlayer.TeleportTo(newPlayerPosition, newPlayerRotation, VRC_SceneDescriptor.SpawnOrientation.Default, false);
                //Yes, could bypass all of the code above and the variable declarations by writing the teleportation in one line of code.
                //I'm not doing this since it is harder to read and doesn't really give me any benefits as long as I don't use local variables.
                //Anyway, this is how it would look like:
                //localPlayer.TeleportTo(localPlayer.GetPosition() + transform.position - lastFramePosition, localPlayer.GetRotation() * Quaternion.Inverse(stationRotation));
            }
            else
            {
                //disables the vehicle walls
                ///activatedWhenPlayerIsInVehicle.SetActive(false);
            }
            //storing vehicle position from this frame
            lastFramePosition = transform.position;
            //storing vehicle rotation from this frame
            lastFrameRotation = transform.rotation;
        }

    }
}

//Todo: Calculate the rotation for an offset point on which the player is actually standing
//      to improve the position preserving when the vehicle rotates (offset to the middle)
