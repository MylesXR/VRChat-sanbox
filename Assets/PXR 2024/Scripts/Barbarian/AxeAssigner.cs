using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using UnityEngine.UI;

public class AxeAssigner : UdonSharpBehaviour
{
    public VRCObjectPool axePool; // Reference to the VRCObjectPool component
    private int currentIndex = 0; // To keep track of the axe index
    public Text debugText; // Reference to the UI Text component for debug logs

    void Start()
    {
        if (axePool == null)
        {
            LogError("Axe pool is not assigned.");
            return;
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        #region head Tracker Ownership
        Log($"Player joined: {player.displayName}");

        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            LogWarning("Non-owner attempted to spawn object from AxeManager");
            return;
        }

        GameObject axeToAssign = axePool.TryToSpawn();
        if (axeToAssign != null)
        {
            // Ensure the axe and its children are inactive before assignment
            axeToAssign.SetActive(false);
            foreach (Transform child in axeToAssign.transform)
            {
                child.gameObject.SetActive(false);
            }

            // Assign ownership to the player
            Networking.SetOwner(player, axeToAssign);
            Log($"Set ownership of axe {axeToAssign.name} to player {player.displayName}");

            // Ensure the child objects also have the correct ownership
            foreach (Transform child in axeToAssign.transform)
            {
                Networking.SetOwner(player, child.gameObject);
                Log($"Set ownership of child {child.gameObject.name} to player {player.displayName}");
                child.gameObject.SetActive(false);
            }

            // Assign axe index to the BarbarianThrowAxe component
            BarbarianThrowAxe throwAxeScript = axeToAssign.GetComponent<BarbarianThrowAxe>();
            if (throwAxeScript != null)
            {
                throwAxeScript.axeIndex = currentIndex; // Use the current index
                throwAxeScript.axeManager = this; // Set reference to AxeManager
                throwAxeScript.ownerPlayer = player; // Set the owner player
                Log($"Assigned axe index {currentIndex} to {player.displayName}");
            }

            // Increment the index for the next axe
            currentIndex++;

            // Activate the axe object but ensure its children remain inactive
            axeToAssign.SetActive(true);

            #endregion

            #region pc and vr axe ownership

            if (axeToAssign.transform.childCount > -1)
            {


                //the PC axe in the head tracker prefab
                GameObject pcAxeChild = axeToAssign.transform.GetChild(0).gameObject;
                pcAxeChild.SetActive(false);
                Log($"Deactivated child game object: {pcAxeChild.name}");



                //The Return point for the VRaxe in the head tracker prefab
                GameObject returnPointChild = axeToAssign.transform.GetChild(1).gameObject;
                returnPointChild.SetActive(true);
                Log($"activated child game object: {returnPointChild.name}");

                //the VR axe in the head tracker prefab
                GameObject vrAxeChild = axeToAssign.transform.GetChild(2).gameObject;
                vrAxeChild.SetActive(false);
                Log($"Deactivated child game object: {vrAxeChild.name}");

                #endregion


                #region pc Tether Ownership

                //The parent of the pc tether objects in the head tracker prefab
                GameObject pcTetherPartent = axeToAssign.transform.GetChild(3).gameObject;
                pcTetherPartent.SetActive(false);
                Log($"activated child game object: {pcTetherPartent.name}");
                //Set ownership of all the pc tether objects children
                foreach (Transform pcTetherLeftHandChild in pcTetherPartent.transform)
                {
                    Networking.SetOwner(player, pcTetherLeftHandChild.gameObject);
                    Log($"Set ownership of child {pcTetherLeftHandChild.gameObject.name} to player {player.displayName}");
                    pcTetherLeftHandChild.gameObject.SetActive(true);
                }
                //Set ownership of the pc tether objects left hand grandchildren
                GameObject pcTetherLeftHandParent = pcTetherPartent.transform.GetChild(0).gameObject;
                GameObject pcLeftTetherRingParent = pcTetherLeftHandParent.transform.GetChild(2).gameObject;
                foreach (Transform pcLeftTetherRingGrandChild in pcLeftTetherRingParent.transform)
                {
                    Networking.SetOwner(player, pcLeftTetherRingGrandChild.gameObject);
                    Log($"Set ownership of grandchild {pcLeftTetherRingGrandChild.gameObject.name} to player {player.displayName}");
                    pcLeftTetherRingGrandChild.gameObject.SetActive(true);
                }
                //Set ownership of the pc tether object right hand grandchildren
                GameObject pcTetherRightHandParent = pcTetherPartent.transform.GetChild(1).gameObject;
                GameObject pcTetherRightRingParent = pcTetherRightHandParent.transform.GetChild(2).gameObject;
                foreach (Transform pcTetherRightRingGrandChild in pcTetherRightRingParent.transform)
                {
                    Networking.SetOwner(player, pcTetherRightRingGrandChild.gameObject);
                    Log($"Set ownership of grandchild {pcTetherRightRingGrandChild.gameObject.name} to player {player.displayName}");
                    pcTetherRightRingGrandChild.gameObject.SetActive(true);
                }

                #endregion

                #region vrTetherParent

                //define the parent of the vr tether objects in head tracker (tetherVRPickUpParent 1)
                GameObject vrTetherParent = axeToAssign.transform.GetChild(4).gameObject;
                vrTetherParent.SetActive(false);
                Log($"Deactivated child game object: {vrTetherParent.name}");

                //Set ownership of all the pc tether objects children (left and right pickup 1)
                foreach (Transform vrTetherHands in vrTetherParent.transform)
                {
                    Networking.SetOwner(player, vrTetherHands.gameObject);
                    Log($"Set ownership of grandchild {vrTetherHands.gameObject.name} to player {player.displayName}");
                    vrTetherHands.gameObject.SetActive(true);
                }

                #endregion

                #region vr Tether Lefthand Ownership

                //define the parent of the vr tether objects left hand 2
                GameObject vrTetherHandsLeft = vrTetherParent.transform.GetChild(0).gameObject;
                //set ownership of the tether object left hand 2
                Networking.SetOwner(player, vrTetherHandsLeft.gameObject);

                //define the parent of the left hands child (forward) 3
                GameObject vrTetherLeftHandChildForward = vrTetherHandsLeft.transform.GetChild(0).gameObject; //forward
                Networking.SetOwner(player, vrTetherLeftHandChildForward.gameObject);

                //set ownership of the vr tether objects left hand children (forward) 3
                foreach (Transform vrTetherLeftHandChildren in vrTetherLeftHandChildForward.transform)
                {
                    Networking.SetOwner(player, vrTetherLeftHandChildren.gameObject);
                    Log($"Set ownership of grandchild {vrTetherLeftHandChildren.gameObject.name} to player {player.displayName}");
                    vrTetherLeftHandChildren.gameObject.SetActive(true);
                }

                //define the child of the vr tether left hand (forward) 4
                GameObject vrTetherLeftHandGrandchildrenParent = vrTetherLeftHandChildForward.transform.GetChild(2).gameObject; // tetherRing

                //set ownership for the great grand childrend of tether left hand 4
                foreach (Transform vrTetherLeftHandGreatGrandchildren in vrTetherLeftHandGrandchildrenParent.transform)
                {
                    Networking.SetOwner(player, vrTetherLeftHandGreatGrandchildren.gameObject);
                    Log($"Set ownership of great grandchild {vrTetherLeftHandGreatGrandchildren.gameObject.name} to player {player.displayName}");
                    vrTetherLeftHandGreatGrandchildren.gameObject.SetActive(true);
                }

                #endregion

                #region VR tether Righthand Ownership

                //the grandchild of the vr tether left hand 


                //define the parent of the vr tether objects left hand 2
                GameObject vrTetherHandsRight = vrTetherParent.transform.GetChild(1).gameObject;
                //set ownership of the tether object right hand 2
                Networking.SetOwner(player, vrTetherHandsRight.gameObject);

                //define the parent of the right hands child (forward) 3
                GameObject vrTetherRightHandChildForward = vrTetherHandsRight.transform.GetChild(0).gameObject; //forward
                Networking.SetOwner(player, vrTetherRightHandChildForward.gameObject);

                //set ownership of the vr tether objects right hand children (forward) 3
                foreach (Transform vrTetherRightHandChildren in vrTetherRightHandChildForward.transform)
                {
                    Networking.SetOwner(player, vrTetherRightHandChildren.gameObject);
                    Log($"Set ownership of grandchild {vrTetherRightHandChildren.gameObject.name} to player {player.displayName}");
                    vrTetherRightHandChildren.gameObject.SetActive(true);
                }

                //define the child of the vr tether right hand (forward) 4
                GameObject vrTetherRightHandGrandchildrenParent = vrTetherLeftHandChildForward.transform.GetChild(2).gameObject; // tetherRing

                //set ownership for the great grand childrend of tether right hand 4
                foreach (Transform vrTetherRightHandGreatGrandchildren in vrTetherRightHandGrandchildrenParent.transform)
                {
                    Networking.SetOwner(player, vrTetherRightHandGreatGrandchildren.gameObject);
                    Log($"Set ownership of great grandchild {vrTetherRightHandGreatGrandchildren.gameObject.name} to player {player.displayName}");
                    vrTetherRightHandGreatGrandchildren.gameObject.SetActive(true);
                }

                #endregion

                #region VR Tether Return Points
                GameObject vrTetherLeftReturnPoint = axeToAssign.transform.GetChild(5).gameObject;
                vrTetherLeftReturnPoint.SetActive(true);
                Log($"Deactivated child game object: {vrTetherParent.name}");

                GameObject vrTetherRightReturnPoint = axeToAssign.transform.GetChild(6).gameObject;
                vrTetherRightReturnPoint.SetActive(true);
                Log($"Deactivated child game object: {vrTetherParent.name}");
               
            }
        }
        #endregion
        else
        {
            LogWarning("No available axes in the pool.");
        }
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        // Handle player leaving logic if needed
    }

    private void Log(string message)
    {
        Debug.Log(message);
        if (debugText != null)
        {
            debugText.text += "\n" + message;
        }
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning(message);
        if (debugText != null)
        {
            debugText.text += "\nWARNING: " + message;
        }
    }

    private void LogError(string message)
    {
        Debug.LogError(message);
        if (debugText != null)
        {
            debugText.text += "\nERROR: " + message;
        }
    }
}