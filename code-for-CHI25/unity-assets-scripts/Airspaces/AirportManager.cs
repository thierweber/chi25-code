using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using static KMLParser;

public class AirportManager : MonoBehaviour
{
    public string kmlFilePath;
    private List<AirspaceData> airspaces;
    public GameObject airspaceParent;
    public GameObject ctrParent;
    public GameObject tmaParent;
    public GameObject atzParent;
    public GameObject awyParent;
    public GameObject restParent;
    public GameObject fizParent;

    public Material ctr_mat;
    public Material rest_mat;
    public Material tma_mat;
    public Material fiz_mat;
    public Material atz_mat;

    public bool show_all_airspaces;
    public List<string> airspace_to_incl;



    void Start()
    {
       
        airspaces = GetComponent<KMLParser>().ParseKML(kmlFilePath);
        foreach (var airspace in airspaces)
        {
            //If show_all_airpaces is checked, show all Else only show the once listed in airspaces_to_incl
            if (show_all_airspaces || airspace_to_incl.Contains(airspace.Name))
            {
                CreateAirspaceGameObject(airspace, airspace.LowerLimit);
            }
        }
    }

    Mesh CreateMesh(List<Vector3> topVertices, List<Vector3> bottomVertices)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Add vertices for the top and bottom
        vertices.AddRange(topVertices);
        vertices.AddRange(bottomVertices);

        int count = topVertices.Count;

        // Create triangles for the top polygon (clockwise)
        for (int i = 0; i < count - 2; i++)
        {
            triangles.Add(0);
            triangles.Add(i + 1);
            triangles.Add(i + 2);
        }

        // Create triangles for the bottom polygon (counter-clockwise to flip the normal)
        int bottomOffset = count;
        for (int i = 0; i < count - 2; i++)
        {
            triangles.Add(bottomOffset);
            triangles.Add(bottomOffset + i + 2);
            triangles.Add(bottomOffset + i + 1);
        }

        // Create side walls (each side as two triangles, clockwise from outside) -> to get rectangles as sidewalls between two verticies
        for (int i = 0; i < count; i++)
        {
            int next = (i + 1) % count;
            // Triangle 1
            triangles.Add(i);
            triangles.Add(bottomOffset + i);
            triangles.Add(next);

            // Triangle 2
            triangles.Add(next);
            triangles.Add(bottomOffset + i);
            triangles.Add(bottomOffset + next);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    void CreateAirspaceGameObject(AirspaceData data, float lowerAltitude)
    {

        GameObject airportGO = new GameObject(data.Name);
        airportGO.tag = "Airspace";

        MeshFilter meshFilter = airportGO.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = airportGO.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = airportGO.AddComponent<MeshCollider>();
        XRRayInteractor interactable = airportGO.AddComponent <XRRayInteractor>();
        meshCollider.convex = true;
        //meshCollider.isTrigger = true;
        AirspaceInteraction interactionScript = airportGO.AddComponent<AirspaceInteraction>();

        List<Vector3> topVertices = new List<Vector3>(data.Polygon);
        List<Vector3> bottomVertices = new List<Vector3>();

        // Create bottom vertices
        foreach (Vector3 vertex in topVertices)
        {
            bottomVertices.Add(new Vector3(vertex.x, lowerAltitude, vertex.z));
        }
        
        Mesh mesh = CreateMesh(topVertices, bottomVertices);
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        string type = data.type;

        //Select Material according to airpace type
        switch (type)
        {
            case "#CTR":
                meshRenderer.material = ctr_mat;
                airportGO.transform.parent = ctrParent.transform;
                break;
            case "#TMA":
                meshRenderer.material = tma_mat;
                airportGO.transform.parent = tmaParent.transform;
                break;
            case "#FIZ":
                meshRenderer.material = fiz_mat;
                airportGO.transform.parent = fizParent.transform;
                break;
            case "#DANGER" or "#RESTRICTED":
                meshRenderer.material = rest_mat;
                airportGO.transform.parent = restParent.transform;
                break;
            case "#AWY":
                meshRenderer.material = tma_mat;
                airportGO.transform.parent = awyParent.transform;
                break;
            case "#ATZ":
                meshRenderer.material = atz_mat;
                airportGO.transform.parent = atzParent.transform;
                break;
        }

        
    }
    void OnDrawGizmos()
    {
        if (airspaces != null)
        {
            foreach (var airspace in airspaces)
            {
                if (airspace.Polygon != null)
                {
                    Gizmos.color = Color.red;
                    foreach (var p in airspace.Polygon)
                    {
                        Gizmos.DrawSphere(p, 0.05f); // Draw a small sphere at each vertex
                    }
                }
            }
        }
    }
}