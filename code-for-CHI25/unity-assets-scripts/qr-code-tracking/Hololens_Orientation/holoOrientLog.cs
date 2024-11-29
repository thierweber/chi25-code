using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class holoOrientLog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the XR device is available
        
            // Get the device's rotation
            Quaternion deviceRotation = InputTracking.GetLocalRotation(XRNode.Head);

            // Log the device's rotation
            Debug.Log("Device Rotation: " + deviceRotation.eulerAngles);
        

    }
}
