using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Inst_POI_Towns : MonoBehaviour
{

    public GameObject Prefab_Town;
    public GameObject Prefab_Chastle;
    public GameObject Prefab_Kloster;
    public GameObject Prefab_Tanklager;
    public GameObject Prefab_AKW;
    public GameObject Prefab_Fabrik;

    public GameObject Prefab_Advisory;
    public GameObject Prefab_Pass;
    public GameObject Prefab_VOR;
    public GameObject Prefab_Airport;

    private FORMULAS converter; // Reference to the CoordinateConverter script
    public GameObject POI_parent;
    public GameObject Town_parent;
    public GameObject Advisory_parent;
    public GameObject Pass_parent;
    public GameObject VOR_parent;
    public GameObject Airport_parent;


    // Path to your CSV files
    public string POICsvFilePath;
    public string TOWNCsvFilePath;
    public string AdvisoryFilePath;
    public string PaesseFilePath;
    public string AirportsFilePath;
    public string VorFilePath;
    public float heightOffset;

    double x, y;

    // Start is called before the first frame update
    void Start()
    {
        converter = FindObjectOfType<FORMULAS>();
        
        //Run Functions to instantiate Towns and POIs
        ReadTowns(TOWNCsvFilePath);
        ReadPOI(POICsvFilePath);
        ReadAdvisory(AdvisoryFilePath);
        ReadPaesse(PaesseFilePath);
        ReadAirports(AirportsFilePath);
        ReadVOR(VorFilePath);
         
    }

    private void ReadPOI(string POICsvFilePath)
    {

        // Read the CSV file
        using (StreamReader reader = new StreamReader(POICsvFilePath))
        {
            // Read the header line to get column names
            string headerLine = reader.ReadLine();
            string[] headers = headerLine.Split(',');

            // Find the index of X and Y columns
            int xColumnIndex = -1;
            int yColumnIndex = -1;
            int nameColumIndex = -1;
            int typeIndex = -1;
            string type = null;
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i] == "Easting")
                    xColumnIndex = i;
                else if (headers[i] == "Northing")
                    yColumnIndex = i;
                else if (headers[i] == "name")
                    nameColumIndex = i;
                else if (headers[i] == "type")
                    typeIndex = i;

            }

            // Ensure both X and Y columns are found
            if (xColumnIndex == -1 || yColumnIndex == -1)
            {
                Debug.LogError("Easting or Northing column not found in the CSV file.");
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
                
                float height = 430;
                if (!double.TryParse(values[xColumnIndex], out x) || !double.TryParse(values[yColumnIndex], out y)) //|| !float.TryParse(values[hColumIndex], out height)
                {
                    Debug.LogError("Error parsing Easting, Northing value.");
                    continue;
                }
                

                switch (type)
                {
                    case "CASTLE":
                        //MODEL SOURCE: https://free3d.com/3d-model/castle-v1--782556.html
                        Vector3 unityCoords = converter.lv95_to_unity_meter((float)x, (float)y, height);
                        // Instantiate the prefab at the specified position
                        GameObject poi = Instantiate(Prefab_Chastle, new Vector3(unityCoords.x, unityCoords.z, unityCoords.y), Quaternion.identity);
                        // Set the parent of the instantiated obstacle to the Obstacles parent empty object
                        poi.transform.parent = POI_parent.transform;
                        poi.transform.rotation *= Quaternion.Euler(-90f, 0f, 0f);
                        poi.name = values[nameColumIndex];

                        break;
                    case "KLOSTER":
                        //MODEL SOURCE: https://free3d.com/3d-model/church-v1--574670.html
                        unityCoords = converter.lv95_to_unity_meter((float)x, (float)y, height);
                        // Instantiate the prefab at the specified position
                        poi = Instantiate(Prefab_Kloster, new Vector3(unityCoords.x, unityCoords.z, unityCoords.y), Quaternion.identity);
                        // Set the parent of the instantiated obstacle to the Obstacles parent empty object
                        poi.transform.parent = POI_parent.transform;
                        poi.transform.rotation *= Quaternion.Euler(-90f, 0f, 0f);
                        poi.name = values[nameColumIndex];
                        break;

                    case "TANKLAGER":
                        //MODEL SOURCE: Own Model
                        unityCoords = converter.lv95_to_unity_meter((float)x, (float)y, height);
                        // Instantiate the prefab at the specified position
                        poi = Instantiate(Prefab_Tanklager, new Vector3(unityCoords.x, unityCoords.z, unityCoords.y), Quaternion.identity);
                        // Set the parent of the instantiated obstacle to the Obstacles parent empty object
                        poi.transform.parent = POI_parent.transform;
                        poi.name = values[nameColumIndex];

                        break;

                    case "AKW":
                        //MODEL SOURCE: https://free3d.com/3d-model/nuclear-cooling-tower-v1--252407.html
                        unityCoords = converter.lv95_to_unity_meter((float)x, (float)y, height);
                        // Instantiate the prefab at the specified position
                        poi = Instantiate(Prefab_AKW, new Vector3(unityCoords.x, unityCoords.z, unityCoords.y), Quaternion.identity);
                        // Set the parent of the instantiated obstacle to the Obstacles parent empty object
                        poi.transform.parent = POI_parent.transform;
                        poi.transform.rotation *= Quaternion.Euler(-90f, 0f, 0f);
                        poi.name = values[nameColumIndex];
                        break;
                    case "FARBIK":
                        //MODEL SOURCE: https://free3d.com/3d-model/shot-tower-v1--655783.html
                        unityCoords = converter.lv95_to_unity_meter((float)x, (float)y, height);
                        // Instantiate the prefab at the specified position
                        poi = Instantiate(Prefab_Fabrik, new Vector3(unityCoords.x, unityCoords.z, unityCoords.y), Quaternion.identity);
                        // Set the parent of the instantiated obstacle to the Obstacles parent empty object
                        poi.transform.parent = POI_parent.transform;
                        poi.transform.rotation *= Quaternion.Euler(-90f, 0f, 0f);
                        poi.name = values[nameColumIndex];
                        break;
                    default:
                        
                        unityCoords = converter.lv95_to_unity_meter((float)x, (float)y, height);
                        poi = Instantiate(Prefab_Chastle, new Vector3(unityCoords.x, unityCoords.z, unityCoords.y), Quaternion.identity);
                        poi.transform.parent = POI_parent.transform;
                        poi.transform.rotation *= Quaternion.Euler(-90f, 0f, 0f);
                        poi.name = values[nameColumIndex];
                        break;



                }




            }
        }
    }
    //---------------------------------------------------------------------------------------------------------------------
    //FUNCTION: Reads towns csv file with structure (ZIP4,SHAPE_AREA,E,N,BFS-Nr,Gemeindename,EINWOHNER)
    private void ReadTowns(string TOWNCsvFilePath)
    {
        List<TownData> townDataList = new List<TownData>();

        using (StreamReader reader = new StreamReader(TOWNCsvFilePath))
        {
            // Skip the header
            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                //ALTITUDE,NAMN1,UUID,OBJVAL,E,N
                string[] data = reader.ReadLine().Split(',');
                string townName = data[1];
                float x = float.Parse(data[4]);
                float y = float.Parse(data[0]); ; //No height data available -> set to 430 to get to 0 with offset
                float z = float.Parse(data[5]);
                
                Vector3 town_coord_unity = converter.lv95_to_unity_meter(x, z, y);
                //Debug.Log("MARKERPOS: " + town_coord_unity);
                Vector3 position = new Vector3(town_coord_unity.x, town_coord_unity.z, town_coord_unity.y);

                townDataList.Add(new TownData(townName, position));
            }
        }

        // Instantiate markers for each town
        foreach (TownData townData in townDataList)
        {
            // Instantiate the town marker prefab
            GameObject townMarker = Instantiate(Prefab_Town, townData.position, Quaternion.identity);
            townMarker.name = townData.townName;
            townMarker.transform.parent = Town_parent.transform;
            TextMeshProUGUI text = townMarker.GetComponentInChildren<TextMeshProUGUI>();
            text.text = townData.townName; //Adjust Text to name of town
        }
    }

    //---------------------------------------------------------------------------------------------------------------------
    private void ReadAdvisory(string AdvisoryFilePath)
    {
        List<AdvisoryData> advisoryDataList = new List<AdvisoryData>();

        using (StreamReader reader = new StreamReader(AdvisoryFilePath))
        {
            // Skip the header
            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                string[] data = reader.ReadLine().Split(',');
                string adName = data[1];
                string airport = data[2];
                float x = float.Parse(data[5]);
                float y = heightOffset; //No height data available -> set to 430 to get to 0 with offset
                float z = float.Parse(data[6]);

                Vector3 town_coord_unity = converter.lv95_to_unity_meter(x, z, y);
                //Debug.Log("MARKERPOS: " + town_coord_unity);
                Vector3 position = new Vector3(town_coord_unity.x, town_coord_unity.z, town_coord_unity.y);

                advisoryDataList.Add(new AdvisoryData(adName, airport, position));
            }
        }

        // Instantiate markers for each town
        foreach (AdvisoryData adData in advisoryDataList)
        {
            // Instantiate the town marker prefab
            GameObject adMarker = Instantiate(Prefab_Advisory, adData.position, Quaternion.identity);
            adMarker.name = adData.adName;
            adMarker.transform.parent = Advisory_parent.transform;
            TextMeshProUGUI text = adMarker.GetComponentInChildren<TextMeshProUGUI>();
            text.text = adData.adName + " " + adData.adAirport; //Adjust Text to name of advisory point + airport 
        }

    }
    //---------------------------------------------------------------------------------------------------------------------
    private void ReadPaesse(string PaesseFilePath)
    {
        List<PaesseData> paesseDataList = new List<PaesseData>();

        using (StreamReader reader = new StreamReader(PaesseFilePath))
        {
            // Skip the header
            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                string[] data = reader.ReadLine().Split(',');
                string paName = data[1];
                string height = data[2]; //in feet!
                float x = float.Parse(data[4]);
                float y = float.Parse(data[2]);//heightOffset; //No height data available
                float z = float.Parse(data[5]);

                Vector3 town_coord_unity = converter.lv95_to_unity_feet(x, z, y);
                //Debug.Log("MARKERPOS: " + town_coord_unity);
                Vector3 position = new Vector3(town_coord_unity.x, town_coord_unity.z, town_coord_unity.y);

                paesseDataList.Add(new PaesseData(paName, height, position));
            }
        }

        // Instantiate markers for each town
        foreach (PaesseData paData in paesseDataList)
        {
            // Instantiate the town marker prefab
            GameObject paMarker = Instantiate(Prefab_Pass, paData.position, Quaternion.identity);
            paMarker.name = paData.passName;
            paMarker.transform.parent = Pass_parent.transform;
            TextMeshProUGUI text = paMarker.GetComponentInChildren<TextMeshProUGUI>();
            text.text = paData.passName + " " + "Elevation: " + paData.height + " feet"; //Adjust Text to name of the pass
        }
    }
    //---------------------------------------------------------------------------------------------------------------------
    private void ReadAirports(string AirportsFilePath)
    {
        List<AirportData> airportDataList = new List<AirportData>();

        using (StreamReader reader = new StreamReader(AirportsFilePath))
        {
            // Skip the header
            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                //id,name,ICAO,altitude,runways,longest_rw,ATIS,TOWER,AD,APRON,type,rwy_type,geometry,E,N
                string[] data = reader.ReadLine().Split(',');
                string airportName = data[1];
                string freq = data[7];
                string runways = data[4];
                string atis = data[6];
                string icao = data[2];
                string alt = data[3];
                string longest_rwy = data[5];
                float x = float.Parse(data[13]);
                float y = float.Parse(data[3]);
                float z = float.Parse(data[14]);

                Vector3 town_coord_unity = converter.lv95_to_unity_feet(x, z, y);
                //Debug.Log("MARKERPOS: " + town_coord_unity);
                Vector3 position = new Vector3(town_coord_unity.x, town_coord_unity.z, town_coord_unity.y);

                airportDataList.Add(new AirportData(airportName, freq, position, runways, atis, icao, alt, longest_rwy));
            }
        }

        // Instantiate markers for each town
        foreach (AirportData airportData in airportDataList)
        {
            // Instantiate the town marker prefab
            GameObject airportMarker = Instantiate(Prefab_Airport, airportData.position, Quaternion.identity);
            airportMarker.name = airportData.airportName;
            airportMarker.transform.parent = Airport_parent.transform;
            TextMeshProUGUI text = airportMarker.GetComponentInChildren<TextMeshProUGUI>();
            text.text = airportData.icao + " " + airportData.airportName + "\n" + "TOWER: " + airportData.ap_freq + "\n" 
                + "ATIS: " + airportData.atis + "\n" + "Altitude: " + airportData.alt + "\n" + "Runways: " + airportData.runways + "\n"
                + " Longest RWY [m]: " + airportData.longest_rwy;  
        }
    }
    //---------------------------------------------------------------------------------------------------------------------

    private void ReadVOR(string VorFilePath)
    {
        List<VORData> vorDataList = new List<VORData>();

        using (StreamReader reader = new StreamReader(VorFilePath))
        {
            // Skip the header
            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                string[] data = reader.ReadLine().Split(',');
                string vorName = data[1];
                string freq = data[2];
                float x = float.Parse(data[5]);
                float y = heightOffset; //No height data available -> set to 430 to get to 0 with offset
                float z = float.Parse(data[6]);

                Vector3 town_coord_unity = converter.lv95_to_unity_meter(x, z, y);
                //Debug.Log("MARKERPOS: " + town_coord_unity);
                Vector3 position = new Vector3(town_coord_unity.x, town_coord_unity.z, town_coord_unity.y);

                vorDataList.Add(new VORData(vorName, freq, position));
            }
        }

        // Instantiate markers for each town
        foreach (VORData vorData in vorDataList)
        {
            // Instantiate the town marker prefab
            GameObject adMarker = Instantiate(Prefab_VOR, vorData.position, Quaternion.identity);
            adMarker.name = vorData.vorName;
            adMarker.transform.parent = VOR_parent.transform;
            TextMeshProUGUI text = adMarker.GetComponentInChildren<TextMeshProUGUI>();
            text.text = vorData.vorName + " " + vorData.frequency; //Adjust Text to name of advisory point + airport 
        }
    }
    //---------------------------------------------------------------------------------------------------------------------

    // Update is called once per frame
    void Update()
    {

    }
    //---------------------------------------------------------------------------------------------------------------------
    public class TownData
    {
        public string townName;
       
        public Vector3 position;

        public TownData(string name, Vector3 pos)
        {
            townName = name;
            position = pos;
          
        }
    }
    //---------------------------------------------------------------------------------------------------------------------
    public class PaesseData
    {
        public string passName;
        public string height;
        public Vector3 position;

        public PaesseData(string name, string ele, Vector3 pos)
        {
            passName = name;
            position = pos;
            height = ele;
        }
    }
    //---------------------------------------------------------------------------------------------------------------------
    public class AirportData
    {
        //id,name,ICAO,altitude,runways,longest_rw,ATIS,TOWER,AD,APRON,type,rwy_type,geometry,E,N
        public string airportName;
        public string ap_freq;
        public string runways;
        public string atis;
        public string icao;
        public string alt;
        public string longest_rwy;
        public Vector3 position;
       
        public AirportData(string name, string freq, Vector3 pos, string runways, string atis, string icao, string alt, string longest_rwy)
        {
            this.airportName = name;
            this.position = pos;
            this.ap_freq = freq;
            this.runways = runways;
            this.atis = atis;
            this.icao = icao;
            this.alt = alt;
            this.longest_rwy = longest_rwy;
        }
    }
    //---------------------------------------------------------------------------------------------------------------------
    public class AdvisoryData
    {
        public string adName;
        public string adAirport;
        public Vector3 position;

        public AdvisoryData(string name, string airport, Vector3 pos)
        {
            adName = name;
            position = pos;
            adAirport = airport;
        }
    }
    //---------------------------------------------------------------------------------------------------------------------
    public class VORData
    {
        public string vorName;
        public string frequency; 
        public Vector3 position;

        public VORData(string name, string freq, Vector3 pos)
        {
            vorName = name;
            position = pos;
            frequency = freq;
        }
    }
    //---------------------------------------------------------------------------------------------------------------------
}
