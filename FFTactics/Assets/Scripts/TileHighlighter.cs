using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TileHighlighter : MonoBehaviour {

    public TileMap Map;

    private MeshFilter _meshFilter;

    // Use this for initialization
    void Start () {
        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.mesh = new Mesh();

        if(Map == null)
            Map = FindObjectOfType<TileMap>();
    }
    
    // Update is called once per frame
    void Update () {
    
    }

    public void HighlightCells(Vector3[] cells)
    {
        Debug.Log("Hello");
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        float yOffset = 0.01f;
        foreach(Vector3 cell in cells)
        {
            Vector3[] tempVertices;
            int[] tempTriangles;
            Vector3[] tempNormals;
            Vector3 center = cell;
            center.y += yOffset;
            CreateHexagonAround(center, out tempVertices, out tempTriangles, out tempNormals);

            int trianglesOffset = vertices.Count;
            vertices.AddRange(tempVertices);

            foreach(int v in tempTriangles)
            {
                triangles.Add(v + trianglesOffset);
            }

            normals.AddRange(tempNormals);
        }
        Mesh mesh = _meshFilter.mesh;
        mesh.Clear();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetNormals(normals);
    }

    private void CreateHexagonAround(Vector3 center, out Vector3[] vertices, out int[] triangles, out Vector3[] normals)
    {
        vertices = new Vector3[6];
        triangles = new int[4*3];
        normals = new Vector3[6];

        Vector3 radiusVector = new Vector3(Map.Radius, 0,0);
        
        for(int i = 0; i < 6; ++i)
        {
            vertices[i] = center + radiusVector;
            normals[i] = Vector3.up;

            radiusVector = Quaternion.AngleAxis(60, Vector3.up) * radiusVector;
        }

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 5;

        triangles[6] = 5;
        triangles[7] = 2;
        triangles[8] = 3;

        triangles[9] = 5;
        triangles[10] = 3;
        triangles[11] = 4;
    }
}
