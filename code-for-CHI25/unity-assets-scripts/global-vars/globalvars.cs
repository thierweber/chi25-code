using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class globalvars
{
    //Global variable for the QR-code orienation
    public static Quaternion QR_orient {  get; set; }

    // Static field for 3D coordinates
    public static Vector3 QR_cal_offset { get; set; }

    // Static field for rotations (Euler angles)
    public static Quaternion QR_cal_rot { get; set; }

    public static bool QR_code_active { get; set; }
    //Global variable for the plane orientation
    public static Quaternion plane_orientation { get; set; }
    public static float plane_alt { get; set; }

    public static float terrain_Scale = 1000f; 

    //QR-CODE Tracking
    public static float dist_to_screen = 0.5f;//[m]
    public static float h_half_screen = 0.3f;//[m]
}