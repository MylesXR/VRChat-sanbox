using UdonSharp;
using UnityEngine;

public class StatueSelectionTrigger : UdonSharpBehaviour
{
    public GameObject targetObject1; // explorer
    public GameObject targetObject2; // barbarian
    public GameObject targetObject3; // alchemist

    public int thisObjectValue; //value of this trigger, changes what class the trigger effects

    private void OnPlayerTriggerEnter()
    {
        ToggleObject();
    }

    public void ToggleObject()
    {
        if (thisObjectValue == 1) //explorer on
        {
            targetObject1.SetActive(true);
            targetObject2.SetActive(false);
            targetObject3.SetActive(false);
            
        }
        if (thisObjectValue == 2) // barbarian on 
        {
            targetObject1.SetActive(false);
            targetObject2.SetActive(true);
            targetObject3.SetActive(false);
            
        }
        if (thisObjectValue == 3) // alchemist on
        {
            targetObject1.SetActive(false);
            targetObject2.SetActive(false);
            targetObject3.SetActive(true);
            
        }
    }
}
