using System;
using UnityEngine;
using UnityEngine.Windows;

public class FORMULAS : MonoBehaviour
{
    // Constants for coordinate system transformation
    private const double E_OFFSET = 2600072.37f;
    private const double N_OFFSET = 1200147.07f;
    private const double E_SCALING = 211455.93f;
    private const double N_SCALING = 308807.95f;
    private const double E_CROSS_TERM = 10938.51f;
    private const double N_CROSS_TERM = 3745.25f;
    private const double E_QUADRATIC_TERM = 0.36f;
    private const double N_QUADRATIC_TERM = 76.63f;
    private const double E_CUBIC_TERM = 44.54f;
    private const double N_CUBIC_TERM = 194.56f;
    private const double PHI_QUADRATIC_TERM = 3745.25f;
    private const double PHI_CUBIC_TERM = 119.79f;
    private const double LAMBDA_QUADRATIC_TERM = 76.63f;
    private const double LAMBDA_CUBIC_TERM = 194.56f;
    private const double HEIGHT_OFFSET = 49.55f;
    private const double HEIGHT_LAMBDA_TERM = 2.73f;
    private const double HEIGHT_PHI_TERM = 6.94f;

    //Used for conversion to unity coordinates
    public  float offsetH; // = 430.0000f;
    public float offsetX; //= 2664022.00000f;
    public float offsetY; //= 1197812.00000f;
    public float offsetXmax; // = 2685712.00000f;
    public float offsetYmax; // = 1213400.00000f;

    // Function to convert WGS84 coordinates to LV03 coordinates
    public Vector2 WGS84ToLV95(double latitude, double longitude)
    {
        double phi = ((latitude * 3600.0f) - 169028.66f) / 10000.0f;
        double lambda = ((longitude * 3600.0f) - 26782.5f) / 10000.0f;

        double e = (E_OFFSET + E_SCALING * lambda) -
                   (E_CROSS_TERM * lambda * phi) -
                   (E_QUADRATIC_TERM * lambda * phi * phi) -
                   (E_CUBIC_TERM * lambda * lambda * lambda);

        double n = (N_OFFSET + N_SCALING * phi) +
                   (N_CROSS_TERM * lambda * lambda) +
                   (PHI_QUADRATIC_TERM * phi * phi) +
                   (N_QUADRATIC_TERM * lambda * lambda) -
                   (PHI_CUBIC_TERM * phi * phi * phi) +
                   (N_CUBIC_TERM * lambda * lambda * phi);

        //LV95 into LV03
        double x = n - 1000000.0;
        double y = e - 2000000.0;
        //Debug.Log("IN FUNCTION: " +"E: " + e + "N: " + n);
        return new Vector2((float)e, (float)n);
    }

    // Function to convert WGS84 height to LV03 height [m]
    public double WGS84HeightToLV95Height(double height)
    {
        double phi = (height - HEIGHT_OFFSET) +
                     HEIGHT_LAMBDA_TERM +
                     HEIGHT_PHI_TERM;

        return phi;
    }

    //Conversion into Unity Project coordinates

    public Vector3 lv95_to_unity_meter(float x, float y, float h)
    {


        float newX = x - offsetX;
        float newY = y - offsetY;
        float newH = h - (offsetH); 


        /*
        Debug.Log("Transformed coordinates:");
        Debug.Log("X: " + newX);
        Debug.Log("Y: " + newY);
        Debug.Log("H: " + newH);
        */
        return new Vector3((newX / globalvars.terrain_Scale), (newY / globalvars.terrain_Scale), (newH / globalvars.terrain_Scale));
       
    }

    public Vector3 lv95_to_unity_feet(float x, float y, float h)
    {


        float newX = x - offsetX;
        float newY = y - offsetY;
        float newH = (float)WGS84HeightToLV95Height((double)(h*0.33f)) - (offsetH ); //Height h given in Feet


        /*
        Debug.Log("Transformed coordinates:");
        Debug.Log("X: " + newX);
        Debug.Log("Y: " + newY);
        
        Debug.Log("Height in Feet: " + h);
        Debug.Log("H: " + newH/1000f);
        */
        //Debug.Log("H: " + newH / globalvars.terrain_Scale);

        return new Vector3((newX/ globalvars.terrain_Scale), (newY/ globalvars.terrain_Scale), (newH/ globalvars.terrain_Scale));

    }




}