using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Linq;


public class airspace_inst : MonoBehaviour
{
    private FORMULAS converter; // Reference to the CoordinateConverter script
    // Start is called before the first frame update
    void Start()
    {
        // Find the GameObject with the CoordinateConverter script attached
        converter = FindObjectOfType<FORMULAS>();

        string path = "SwissAirspaces_3D.kml";
        var polygons = ParseKML(path);
        foreach (var poly in polygons)
        {
            Create3DGameObject(poly); // Adjust heights as necessary

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<List<Vector3>> ParseKML(string filePath)
    {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            XmlNodeList coordinatesList = doc.GetElementsByTagName("coordinates");
            List<List<Vector3>> polygons = new List<List<Vector3>>();

            foreach (XmlNode coordinates in coordinatesList)
            {
                List<Vector3> polygon = new List<Vector3>();
                string[] coords = coordinates.InnerText.Trim().Split(' ');

                foreach (string coord in coords)
                {
                    string[] parts = coord.Split(',');
                    if (parts.Length >= 3)
                    {
                        float lon = float.Parse(parts[0]);
                        float lat = float.Parse(parts[1]);
                        float alt = float.Parse(parts[2]);


                    // Convert lat/lon to Unity world coordinates
                    Vector2 lv95Coords = converter.WGS84ToLV95(lon, lat);
                    Vector3 unityCoords = converter.lv95_to_unity_feet(lv95Coords.x, lv95Coords.y, alt);
                    polygon.Add(new Vector3(unityCoords.x, unityCoords.z, unityCoords.y));
                    }
                }

                polygons.Add(polygon);
            }

            return polygons;
     }


    public Mesh CreateMesh(List<Vector2> vertices)
    {
        Mesh mesh = new Mesh();
        List<int> triangles = new List<int>();
        int n = vertices.Count;

        if (n < 3)
            return mesh;

        // Assume the vertices are ordered counter-clockwise
        // Find the area of the polygon to check the order
        float area = 0;
        for (int i = 0; i < n; i++)
        {
            int j = (i + 1) % n;
            area += vertices[i].x * vertices[j].y - vertices[j].x * vertices[i].y;
        }

        // If the area is positive, the vertices are counter-clockwise
        if (area < 0)
        {
            vertices.Reverse(); // Reverse to counter-clockwise
        }

        // Ear clipping algorithm for convex polygons
        int[] V = new int[n];
        for (int i = 0; i < n; i++)
        {
            V[i] = i;
        }

        int count = n;
        int v = n - 1;
        while (count > 2)
        {
            int u = v;
            if (count <= u) u = 0; // Reset u to 0 if it exceeds count
            v = u + 1;
            if (v >= count) v = 0;
            int w = v + 1;
            if (w >= count) w = 0;

            if (IsConvex(vertices[V[u]], vertices[V[v]], vertices[V[w]]))
            {
                bool isEar = true;
                for (int i = 0; i < count; i++)
                {
                    if (i == u || i == v || i == w) continue;
                    if (IsPointInTriangle(vertices[V[i]], vertices[V[u]], vertices[V[v]], vertices[V[w]]))
                    {
                        isEar = false;
                        break;
                    }
                }

                if (isEar)
                {
                    triangles.Add(V[u]);
                    triangles.Add(V[v]);
                    triangles.Add(V[w]);
                    for (int i = v; i < count - 1; i++)
                    {
                        V[i] = V[i + 1];
                    }
                    count--;
                    v--;
                }
            }
        }

        mesh.vertices = vertices.Select(v => new Vector3(v.x, 0, v.y)).ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    bool IsConvex(Vector2 a, Vector2 b, Vector2 c)
    {
        // Return true if the triangle abc is convex
        return (c.y - a.y) * (b.x - a.x) > (b.y - a.y) * (c.x - a.x);
    }

    bool IsPointInTriangle(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        var s = p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y;
        var t = p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y;

        if ((s < 0) != (t < 0))
            return false;

        var A = -p1.y * p2.x + p0.y * (p2.x - p1.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y;
        return A < 0 ?
                (s <= 0 && s + t >= A) :
                (s >= 0 && s + t <= A);
    }

    public GameObject Create3DGameObject(List<Vector3> basePolygon)
    {
            GameObject obj = new GameObject("Airspace");
            MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>(); // Fill this with your triangulation indices

            // Create base polygon
            Mesh baseMesh = CreateMesh(basePolygon.ConvertAll(p => new Vector2(p.x, p.y)));
            vertices.AddRange(basePolygon);

            // Create top polygon (assuming constant altitude for simplicity)
            float minHeight = basePolygon[0].z;
            float maxHeight = minHeight + 500; // Example: 500 units higher
            foreach (Vector3 baseVertex in basePolygon)
            {
                vertices.Add(new Vector3(baseVertex.x, baseVertex.y, maxHeight));
            }

            // Assuming that baseMesh.triangles contains the indices for the base polygon
            triangles.AddRange(baseMesh.triangles);

            // Add indices for the top polygon (flipped order for correct normals)
            int count = basePolygon.Count;
            for (int i = 0; i < baseMesh.triangles.Length; i += 3)
            {
                triangles.Add(baseMesh.triangles[i] + count);
                triangles.Add(baseMesh.triangles[i + 2] + count);
                triangles.Add(baseMesh.triangles[i + 1] + count);
            }

            // Add side quads
            for (int i = 0; i < count; i++)
            {
                int next = (i + 1) % count;
                triangles.Add(i);
                triangles.Add(next);
                triangles.Add(next + count);

                triangles.Add(i);
                triangles.Add(next + count);
                triangles.Add(i + count);
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
            return obj;
        }
    }
