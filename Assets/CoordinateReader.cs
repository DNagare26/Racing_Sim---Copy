using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CoordinateReader : MonoBehaviour
{
    public TextAsset coordinateText;

    void Start()
    {
        string text = coordinateText.text;
        string[] lines = text.Split('\n');

        List<Vector2> points = new List<Vector2>();
        List<string> IRLpoints = new List<string>();

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            if (!string.IsNullOrWhiteSpace(trimmedLine))
            {
                string[] coordinates = trimmedLine.Split(',');
                if (coordinates.Length == 3) // assuming lat,long,0 format
                {
                    if (float.TryParse(coordinates[0], out float latitude) &&
                        float.TryParse(coordinates[1], out float longitude))
                    {
                        Vector2 point = ConvertAzureMapsCoordinate(latitude, longitude);
                        points.Add(point);
                        IRLpoints.Add(point.x + "," + point.y);
                    }
                }
            }
        }

        string path = Application.dataPath + "/SilverStoneIRL.txt";
        File.WriteAllLines(path, IRLpoints);

        CreateTrackMesh(points);

        Debug.Log("Coordinates count: " + points.Count);
    }

    Vector2 ConvertAzureMapsCoordinate(float latitude, float longitude)
    {
        // Mercator projection
        float x_mercator = 6371f * longitude * Mathf.PI / 180f;
        float y_mercator = 6371f * Mathf.Log(Mathf.Tan(Mathf.PI / 4f + latitude * Mathf.PI / 360f));

        // Convert to Unity coordinates
        float x_unity = x_mercator * 100f; // adjust this value to change the map scale
        float y_unity = y_mercator * 100f;

        return new Vector2(x_unity, y_unity);
    }

    void CreateTrackMesh(List<Vector2> points)
    {
        GameObject trackObject = new GameObject("Track");
        MeshFilter meshFilter = trackObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = trackObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[points.Count * 2];
        int[] triangles = new int[(points.Count - 1) * 6];

        float width = 5f; // Adjust the width of the track

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 point = points[i];
            Vector2 direction = (i < points.Count - 1) ? (points[i + 1] - point).normalized : (point - points[i - 1]).normalized;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x) * width;

            vertices[i * 2] = new Vector3(point.x + perpendicular.x, 0, point.y + perpendicular.y);
            vertices[i * 2 + 1] = new Vector3(point.x - perpendicular.x, 0, point.y - perpendicular.y);

            if (i < points.Count - 1)
            {
                int startIndex = i * 6;
                int vertexIndex = i * 2;

                triangles[startIndex] = vertexIndex;
                triangles[startIndex + 1] = vertexIndex + 2;
                triangles[startIndex + 2] = vertexIndex + 1;

                triangles[startIndex + 3] = vertexIndex + 1;
                triangles[startIndex + 4] = vertexIndex + 2;
                triangles[startIndex + 5] = vertexIndex + 3;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        // Optionally, set the material and color of the track
        meshRenderer.material = new Material(Shader.Find("Standard"));
        meshRenderer.material.color = Color.red;
    }
}
