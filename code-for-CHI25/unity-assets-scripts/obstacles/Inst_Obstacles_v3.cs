using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using static MixedReality.Toolkit.UX.Experimental.NonNativeFunctionKey;
using UnityEditor;

public class Inst_Obstacle_v3 : MonoBehaviour
{
    public GameObject prefab_building;
    public GameObject prefab_stack;
    public GameObject prefab_pole;
    public GameObject prefab_crane;
    private  FORMULAS converter; // Reference to the CoordinateConverter script
    public GameObject Obstacles;
    public string obstaclesCsvFilePath;
    public string transmissionLinesCsvFilePath;
    

    //Color Selection Line Objects
    private Color transmission_color = Color.red;
    private Color catnery_color =  new Color((253f/256f),(173f/ 256f),(11f/ 256f)) ; //orange
    private Color bridge_color = new Color((208f / 256f), (138f / 256f), (0f / 256f)) ; //dark orange
    private Color cable_car_color = new Color((153f / 256f), (0f / 256f), (0f / 256f)) ; //dark red

    //Define cable width
    float cablewidth = 0.002f;



    void Start()
    {
       
      

        // Dictionary to store transmission lines by their names
        Dictionary<string, List<Vector3>> transmissionLines = new Dictionary<string, List<Vector3>>();
        // Find the GameObject with the CoordinateConverter script attached
        converter = FindObjectOfType<FORMULAS>();

        //Run Functions to instantiate Obstacles
        ReadAndInstantiateObstacles(obstaclesCsvFilePath);
        ReadTransmissionlines(transmissionLinesCsvFilePath);
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Function to read the Point Obstacles
        void ReadAndInstantiateObstacles(string obstaclesCsvFilePath)
        {
            // Read the CSV file
            using (StreamReader reader = new StreamReader(obstaclesCsvFilePath))
            {
                // Read the header line to get column names
                string headerLine = reader.ReadLine();
                string[] headers = headerLine.Split(',');

                //Name,uuid,airport,obstacleTy,maxHeightA,topElevati,geometry,N,E
                int xColumnIndex = -1;
                int yColumnIndex = -1;
                int hColumIndex = -1;
                int typeIndex = -1;
                string type = null;
               
                
                for (int i = 0; i < headers.Length; i++)
                {
                    if (headers[i] == "N")
                        xColumnIndex = i;
                    else if (headers[i] == "E")
                        yColumnIndex = i;
                    else if (headers[i] == "topElevati")
                        hColumIndex = i;
                    else if (headers[i] == "obstacleTy")
                        typeIndex = i;

                }

                // Ensure both X and Y columns are found
                if (xColumnIndex == -1 || yColumnIndex == -1)
                {
                    Debug.LogError("E or N column not found in the CSV file.");
                    return;
                }

                // Read remaining lines
                while (!reader.EndOfStream)
                {
                    // Read a line
                    string line = reader.ReadLine();
                    

                    // Split the line by comma
                    string[] values = line.Split(',');
                    type = values[typeIndex];
                    // Parse X and Y values
                    double x, y;
                    float height;
                    if (!double.TryParse(values[xColumnIndex], out x) || !double.TryParse(values[yColumnIndex], out y) || !float.TryParse(values[hColumIndex], out height))
                    {
                        Debug.LogError("Error parsing E or N value.");
                        continue;
                    }

                    switch (type)
                    {
                        case "BUILDING":
                            Vector3 unityCoords = converter.lv95_to_unity_meter((float)x,(float) y, height);
                            // Instantiate the prefab at the specified position
                            GameObject obst = Instantiate(prefab_building, new Vector3(unityCoords.x, unityCoords.z, unityCoords.y), Quaternion.identity);
                            // Set the parent of the instantiated obstacle to the Obstacles parent empty object
                            obst.transform.parent = Obstacles.transform;

                            break;
                        case "CRANE":
                            unityCoords = converter.lv95_to_unity_meter((float)x, (float)y, height);
                            // Instantiate the prefab at the specified position
                            obst = Instantiate(prefab_crane, new Vector3(unityCoords.x, unityCoords.z, unityCoords.y), Quaternion.identity);
                            // Set the parent of the instantiated obstacle to the Obstacles parent empty object
                            obst.transform.parent = Obstacles.transform;
                            break;

                        case "STACK":
                            unityCoords = converter.lv95_to_unity_meter((float)x, (float)y, height);
                            // Instantiate the prefab at the specified position
                            obst = Instantiate(prefab_stack, new Vector3(unityCoords.x, unityCoords.z, unityCoords.y), Quaternion.identity);
                            // Set the parent of the instantiated obstacle to the Obstacles parent empty object
                            obst.transform.parent = Obstacles.transform;

                            break;

                        case "POLE":
                            unityCoords = converter.lv95_to_unity_meter((float)x, (float)y, height);
                            // Instantiate the prefab at the specified position
                            obst = Instantiate(prefab_pole, new Vector3(unityCoords.x, unityCoords.z, unityCoords.y), Quaternion.identity);
                            // Set the parent of the instantiated obstacle to the Obstacles parent empty object
                            obst.transform.parent = Obstacles.transform;
                            break;
                        default:
                            unityCoords = converter.lv95_to_unity_meter((float)x, (float)y, height);
                            obst = Instantiate(prefab_building, new Vector3(unityCoords.x, unityCoords.z, unityCoords.y), Quaternion.identity);
                             obst.transform.parent = Obstacles.transform;
                            break;



                    }
                    
                    


                }
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Function to render a cable from a list of points. This function is called Transmission lines, but works for all cable types.
        void ReadTransmissionlines(string transmissionLinesCsvFilePath)
        {
            List<string> LinesType = new List<string>(); ;
            // Read the CSV file
            using (StreamReader reader = new StreamReader(transmissionLinesCsvFilePath))
            {
                // Read the header line to get column names
                string headerLine = reader.ReadLine();
                string[] headers = headerLine.Split(',');
                string currentTransmissionLineName = null;
                string currentLineType = null;
               
                List<Vector3> currentTransmissionLinePoints = null;
                

                while (!reader.EndOfStream)
                {
                    // Read a line
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');

                    // Assuming the first column is the name of the transmission line
                    string transmissionLineName = values[2];

                    // If the transmission line name changes, create a new list of points
                    if (transmissionLineName != currentTransmissionLineName)
                    {
                        // If there was a previous line, store it
                        if (currentTransmissionLineName != null && currentTransmissionLinePoints != null)
                        {
                            transmissionLines[currentTransmissionLineName] = currentTransmissionLinePoints;
                            //Debug.Log(LinesType);
                            LinesType.Add(currentLineType);
                        }

                        // Start a new line
                        currentTransmissionLineName = transmissionLineName;
                        currentTransmissionLinePoints = new List<Vector3>();
                    }

                    // Parse the coordinates
                    //Debug.Log("VALUES:  " + values);
                    float x = float.Parse(values[7]);
                    float y = float.Parse(values[6]);
                    float height = float.Parse(values[8]);
                    currentLineType = values[4];
                    Vector2 lv95Coords = converter.WGS84ToLV95(x, y);
                    Vector3 unityCoords = converter.lv95_to_unity_meter(lv95Coords.x, lv95Coords.y, height);
                    Vector3 point = new Vector3(unityCoords.x, unityCoords.z, unityCoords.y);

                    // Add the point to the current line's list of points
                    currentTransmissionLinePoints.Add(point);
                }

                // Add the last transmission line
                if (currentTransmissionLineName != null && currentTransmissionLinePoints != null)
                {
                   
                    transmissionLines[currentTransmissionLineName] = currentTransmissionLinePoints;
                    //Debug.Log(LinesType);
                    LinesType.Add(currentLineType);
                }
            }

            int i = 0;
            // Instantiate LineRenderers for each transmission line
            foreach ( var kvp in transmissionLines)
            {
                    string linetype = LinesType[i];
                    //Debug.Log("LINETYPE: " +linetype);

                switch (linetype)
                {
                    case "TRANSMISSION_LINE":

                        GameObject lineObject = new GameObject(kvp.Key + "_Line");
                        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
                        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                        lineRenderer.startColor = transmission_color;
                        lineRenderer.endColor = transmission_color;
                        lineRenderer.startWidth = cablewidth;
                        lineRenderer.endWidth = cablewidth;
                        lineRenderer.positionCount = kvp.Value.Count;
                        lineRenderer.SetPositions(kvp.Value.ToArray());

                        // Set the parent of the line object
                        lineObject.transform.parent = Obstacles.transform;
                        i = i + 1;
                        break;

                    case "CATENRY":
                        lineObject = new GameObject(kvp.Key + "_Line");
                        lineRenderer = lineObject.AddComponent<LineRenderer>();
                        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                        lineRenderer.startColor = catnery_color;
                        lineRenderer.endColor = catnery_color;
                        lineRenderer.startWidth = cablewidth;
                        lineRenderer.endWidth = cablewidth;
                        lineRenderer.positionCount = kvp.Value.Count;
                        lineRenderer.SetPositions(kvp.Value.ToArray());

                        // Set the parent of the line object
                        lineObject.transform.parent = Obstacles.transform;
                        i = i + 1;
                        break;

                    case "CABLE_CAR":
                        lineObject = new GameObject(kvp.Key + "_Line");
                        lineRenderer = lineObject.AddComponent<LineRenderer>();
                        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                        lineRenderer.startColor = cable_car_color;
                        lineRenderer.endColor = cable_car_color;
                        lineRenderer.startWidth = cablewidth;
                        lineRenderer.endWidth = cablewidth;
                        lineRenderer.positionCount = kvp.Value.Count;
                        lineRenderer.SetPositions(kvp.Value.ToArray());

                        // Set the parent of the line object
                        lineObject.transform.parent = Obstacles.transform;
                        i = i + 1;
                        break;

                    case "BRIDGE":
                        lineObject = new GameObject(kvp.Key + "_Line");
                        lineRenderer = lineObject.AddComponent<LineRenderer>();
                        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                        lineRenderer.startColor = bridge_color;
                        lineRenderer.endColor = bridge_color;
                        lineRenderer.startWidth = cablewidth;
                        lineRenderer.endWidth = cablewidth;
                        lineRenderer.positionCount = kvp.Value.Count;
                        lineRenderer.SetPositions(kvp.Value.ToArray());

                        // Set the parent of the line object
                        lineObject.transform.parent = Obstacles.transform;
                        i = i + 1;
                        break;



                    default:
                        lineObject = new GameObject(kvp.Key + "_Line");
                        lineRenderer = lineObject.AddComponent<LineRenderer>();
                        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                        lineRenderer.startColor = transmission_color;
                        lineRenderer.endColor = transmission_color;
                        lineRenderer.startWidth = cablewidth;
                        lineRenderer.endWidth = cablewidth;
                        lineRenderer.positionCount = kvp.Value.Count;
                        lineRenderer.SetPositions(kvp.Value.ToArray());

                        // Set the parent of the line object
                        lineObject.transform.parent = Obstacles.transform;
                        i = i + 1;
                        break;
                }

                }
        }
    
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------

    }
}