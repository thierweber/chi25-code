using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
   
   
    public float fieldOfViewOverride = 60.0f;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        Debug.AssertFormat(cam != null, "No {0} component on game object named {1} as expected.", typeof(Camera), gameObject.name);
        cam.projectionMatrix = Matrix4x4.Perspective(fieldOfViewOverride, cam.aspect, cam.nearClipPlane, cam.farClipPlane);
    }
    



    // Update is called once per frame
    void Update()
    {
        //Debug.Log(cam.fieldOfView);
    }
}
