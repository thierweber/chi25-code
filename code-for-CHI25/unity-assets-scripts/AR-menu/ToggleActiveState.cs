using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityNpgsqlTypes;

public class ToggleActiveState : MonoBehaviour
{
    public GameObject targetObject;
    public void ToggleActive()
    {
        if (targetObject != null)
        {
            bool newState = !targetObject.activeSelf;
            targetObject.SetActive(newState); // Toggle the active state
            //Debug.Log("Toggled " + targetObject.name + " to " + newState);
        }
    }
}