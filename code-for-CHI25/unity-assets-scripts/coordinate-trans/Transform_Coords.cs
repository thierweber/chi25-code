using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transform_Coords : MonoBehaviour
{

    private FORMULAS converter; // Reference to the CoordinateConverter script
    public GameObject plane;
    
    void Start()
    {

        // Find the GameObject with the CoordinateConverter script attached
        converter = FindObjectOfType<FORMULAS>();

        // Example usage: convert WGS84 coordinates to LV03 coordinates
        double latitude = 46.97295; // Example latitude
        double longitude = 8.37462; // Example longitude

        Vector2 lv03Coords = converter.WGS84ToLV95(latitude, longitude);
        //Debug.Log("LV03 Coordinates: " + lv03Coords);
        Vector3 unityCoords = converter.lv95_to_unity_feet(lv03Coords.x, lv03Coords.y, 700.0f);
        //Debug.Log("Unity Coordinates: " + unityCoords);
        //plane.transform.position = new Vector3(unityCoords.x, unityCoords.z, unityCoords.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
