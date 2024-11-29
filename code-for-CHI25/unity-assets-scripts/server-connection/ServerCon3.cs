using System;
using System.Collections;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.MPE;

//using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityNpgsql;

public class ServerCon3 : MonoBehaviour
{
    private string connectionString = "Host=XXXX;Port=XXXX;Database=XXXXX;Username=XXXXX;Password=XXXX";
    // Define NpgsqlConnection 
    public NpgsqlConnection connection;
   


    private FORMULAS converter; // Reference to the CoordinateConverter script
    public GameObject plane;
    public GameObject camera;

    private float yDifference;
    float diff_old;
    float old_orient_plane;
    float old_orient_qr;


    async void Start()
    {
        // Find the GameObject with the CoordinateConverter script attached
        converter = FindObjectOfType<FORMULAS>();
        await OpenConnection(); 


        //ConnectToDatabase();
    }

    private async Task ConnectToDatabase()
    {
        try
        {


            //Debug.Log("CONNECTION NOT OPEN! CONNECTION STATE " + connection.State);
            // Example: executing a simple query
            string query = "SELECT * FROM simconnect.simconnect3 ORDER BY id DESC LIMIT 1"; //"SELECT * FROM simconnect.simconnect3";


            if (connection.State == ConnectionState.Open)
            {
                await ExecuteQuery(connection, query);
            }
            
            
        }
        catch (NpgsqlException e)
        {
            Debug.LogError("Error connecting to PostgreSQL database: " + e.Message);
        }
    }

    private async void Update()
    {
        
        await ConnectToDatabase ();
    }

    private async Task OpenConnection()
    {
        connection = new NpgsqlConnection(connectionString);
        try
        {
            await connection.OpenAsync();
            Debug.Log("Connection opened successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error opening connection: " + ex.Message);
        }
    }

    private async Task ExecuteQuery(NpgsqlConnection connection, string query)
    {
        try
        {
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        
                        //Debug.Log("ID: " + reader.GetInt32(0) + " Lat: " + reader.GetDouble(1) + " Lon: " + reader.GetDouble(2) + " alt: " + reader.GetDouble(3) + " heading: " + reader.GetDouble(4) + " pitch: " + reader.GetDouble(5) + " bank: " + reader.GetDouble(6) + " speed: " + reader.GetDouble(7));
                        Vector2 lv95Coords =  converter.WGS84ToLV95(reader.GetDouble(1), reader.GetDouble(2));
                        //Debug.Log("WGS84: " + " Lat: " + reader.GetDouble(1) + " Lon: " + reader.GetDouble(2));
                        //Debug.Log("LV95 Coordinates: " + lv95Coords);
                        
                        Vector3 unityCoords = converter.lv95_to_unity_feet(lv95Coords.x, lv95Coords.y, (float)reader.GetDouble(3));
                        globalvars.plane_alt = (float)reader.GetDouble(3);
                        //Debug.Log("Unity Coordinates: " + unityCoords);
                        //Adjust Position incl. Height of camera
                        //Debug.Log(" alt: " + reader.GetDouble(3) + " feet");
                        //Debug.Log("OFFSET: " + globalvars.QR_cal_offset);
                        Vector3 pos = new Vector3(unityCoords.x, unityCoords.z, unityCoords.y);
                        Vector3 cal_pos = pos - globalvars.QR_cal_offset;
                        plane.transform.position = pos;  //new Vector3(unityCoords.x, unityCoords.z, unityCoords.y);
                        //Adjust Orientation of camera
                        double pitch = reader.GetDouble(5);
                        double yaw = reader.GetDouble(4);
                        double roll = reader.GetDouble(6);

                       

                        //Debug.Log("Pitch " + pitch + " roll " + roll + " yaw " + yaw);

                        globalvars.plane_orientation = Quaternion.Euler((float)pitch, ((float)yaw), (float)roll);
                       
                        float rotationY = (globalvars.QR_orient.eulerAngles.y - 90f) - globalvars.plane_orientation.eulerAngles.y;
                        // Get current rotation in Euler angles
                        Vector3 currentEuler = globalvars.plane_orientation.eulerAngles;
                        //Debug.Log("CURRENT EULER: " + currentEuler);

                        //QR-Code Position Offset: correct angle with occurs due to QR-Code placed on top of the screen and not center
                        float d_new = Math.Abs((float)Math.Sin(((roll)*(Math.PI/180))) * globalvars.h_half_screen);
                        float qr_offset = (float) Math.Atan2(d_new, globalvars.dist_to_screen);
 

                        yDifference = await getDiffernce(globalvars.QR_orient.y, globalvars.plane_orientation.y, qr_offset, roll);

                        
                        //Detecting Jumps in the orientation differences
                        if (Math.Abs((yDifference - diff_old)) > 20f)
                        {
                            Debug.Log("1. JUMP DETECTED: " + ((yDifference - diff_old)) + "Diff old: " + diff_old + "Diff new: " + yDifference + "\n" +
                                     " 2. OLD ORIENT QR: " + old_orient_qr + " CURRENT ORIENT QR: " + (globalvars.QR_orient.y * (180 / Math.PI)) + "\n" +
                                     " 3. OLD ORIENT PLANE: " + old_orient_plane + " CURRENT ORIENT PLANE: " + (globalvars.plane_orientation.y * (180 / Math.PI)));
                        }

                        diff_old = yDifference;
                        old_orient_plane = (float)(globalvars.plane_orientation.y * (180 / Math.PI));
                        old_orient_qr = (float)(globalvars.QR_orient.y * (180 / Math.PI));
                       


                        if (yDifference > 180)
                        {
                            currentEuler.y -= (yDifference - 180f);
                        }
                        else
                        {
                            currentEuler.y -= (yDifference - 0f);
                            // currentEuler.z += 5f;

                            // currentEuler.x += 5f;
                        }


                        if (Math.Abs(rotationY) > 180f)
                        {
                            rotationY -= 360f;
                        }


                        //Debug.Log("ROTATION Y after: " + rotationY);

                        currentEuler.x -= 5f;
                        
                   
              
                        
                        await setOrient(currentEuler);





                    }
                }
            }
        }
        catch (NpgsqlException e)
        {
            Debug.LogError("Error executing query: " + e.Message);
        }
    }
    void OnDestroy()
    {
        // Ensure to close and dispose the connection when the GameObject is destroyed
        if (connection != null)
        {
            connection.Close();
            connection.Dispose();
            Debug.Log("Connection closed and disposed.");
        }
    }
    IEnumerator DelayCoroutine()
    {
        // Output to console
        Debug.Log("Starting delay...");

        // Wait for 500 milliseconds
        yield return new WaitForSeconds(1.5f);

        // Output to console
        Debug.Log("Delay complete!");
    }

    async Task<float> getDiffernce(float y1, float y2, float qr_offset, double roll)
    {
        //Transform orientations into Degree
        float orient_qr = (float)(y1 * (180 / Math.PI));
        float orient_plane = (float)(y2 * (180 / Math.PI));
        float qr_off = (float)(qr_offset * (180 / Math.PI));


        float diff =  y1 - y2; //orient_qr - orient_plane;
        float diff_corr; 


        //Correct displacement of QR-Code due to bank-angle
        if (roll > 0) //left turn
        {
            diff_corr = diff - qr_offset;
        }
        else //right turn
        {
            diff_corr = diff + qr_offset;
        }
        

        //Debug.Log("Difference: " + diff);

        return diff_corr; //diff_corr;  (float)(diff * (180 / Math.PI)); 

    }

    async Task setOrient(Vector3 currentEuler)
    {
        plane.transform.eulerAngles = currentEuler; //currentEuler;

    }
}