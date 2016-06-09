using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour
{

    public float Radius = 1;
    public float Height = 1;
    public int StepCount = 4;

    public int DimX = 4;
    public int DimZ = 4;

    private const int ANGLE = 60;
    private const float MAGIC_NUMBER = 0.866025403f;

    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

	// Use this for initialization
	void Start ()
	{
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
	    GenerateMesh();
	}

    void Update()
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(_meshCollider.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            int triangleIndex = hitInfo.triangleIndex;
            int hexagonIndex = triangleIndex / 16;
            int z = hexagonIndex % DimZ;
            int x = hexagonIndex / DimZ;
            Debug.Log(string.Format("{0}:, ({1}, {2})", hexagonIndex, x, z));

        }
    }

    public void GenerateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        Vector3 center = new Vector3(0, Height, 0);
        Vector3 xOffset = new Vector3(Radius + Radius/2.0f, 0, 0);
        float seed = System.DateTime.Now.Second;
        for(int x = 0; x < DimX; ++x)
        {
            for(int z = 0; z < DimZ; ++z)
            {
                float perlin = Mathf.Clamp01(Mathf.PerlinNoise(seed + x / (float)DimX, seed + z/(float)DimZ));
                perlin = Step(perlin, StepCount, 0, 1);
                center.y = perlin*Height;
                Vector3[] tempVertices;
                int[] tempTriangles;
                Vector3[] tempNormals;
                Vector2[] tempUvs;
                CreateTileAround(center, out tempVertices, out tempTriangles, out tempNormals, out tempUvs, perlin - 1.0f/StepCount);

                int trianglesOffset = vertices.Count;
                vertices.AddRange(tempVertices);

                foreach(int v in tempTriangles)
                {
                    triangles.Add(v + trianglesOffset);
                }

                normals.AddRange(tempNormals);
                uvs.AddRange(tempUvs);
                center += new Vector3(0, 0, 2*Radius*MAGIC_NUMBER);
            }
            center += xOffset;
            if(x%2==0)
            {
                center.z = MAGIC_NUMBER*Radius;
            }
            else
            {
                center.z = 0;
            }
        }


        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();

        _meshFilter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
    }

    void CreateTileAround(Vector3 center, out Vector3[] vertices, out int[] triangles, out Vector3[] normals, out Vector2[] uvs, float uvOffset)
    {
        vertices = new Vector3[6*3];
        triangles = new int[16*3];
        normals = new Vector3[6*3];
        uvs = new Vector2[6*3];

        Vector3 radiusVector = new Vector3(Radius, 0, 0);
        Vector3 heightVector = new Vector3(0, center.y, 0);

        for (int i = 0; i < 6; ++i)
        {
            
            vertices[i] = center + radiusVector;
            vertices[6+i] = vertices[i];
            vertices[12+i] = vertices[i] - heightVector;
            radiusVector = Quaternion.AngleAxis(ANGLE, Vector3.up) * radiusVector;

            normals[i] = Vector3.up;
            normals[6+i] = Vector3.Normalize(vertices[i] - center);
            normals[12+i] = Vector3.Normalize(vertices[i] - center);
        }

        // Top hexagon
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

        //Top Uv
        uvs[0] = new Vector2(.25f, uvOffset + 1.0f * 0.25f);
        uvs[1] = new Vector2(0.5f, uvOffset +  0.75f * 0.25f);
        uvs[2] = new Vector2(0.5f, uvOffset +  0.25f * 0.25f);
        uvs[3] = new Vector2(0.25f, uvOffset +  0 * 0.25f);
        uvs[4] = new Vector2(0, uvOffset +  0.25f* 0.25f);
        uvs[5] = new Vector2(0, uvOffset +  0.75f * 0.25f);

        //Side Uvs
        uvs[6] = new Vector2(0.5f, uvOffset + .25f);
        uvs[7] = new Vector2(1, uvOffset + .25f);
        uvs[8] =  new Vector2(0.5f, uvOffset + .25f);
        uvs[9] = new Vector2(1 ,uvOffset + .25f);
        uvs[10] =  new Vector2(0.5f, uvOffset + .25f);
        uvs[11] = new Vector2(1, uvOffset + .25f);

        uvs[12] = new Vector2(0.5f, uvOffset);
        uvs[13] = new Vector2(1, uvOffset);
        uvs[14] = new Vector2(.5f, uvOffset);
        uvs[15] = new Vector2(1, uvOffset);
        uvs[16] = new Vector2(0.5f, uvOffset);
        uvs[17] = new Vector2(1, uvOffset);

        //Side wall
        for(int i = 0; i < 6; ++i)
        {
            triangles[12 + 6*i] = i+6;
            triangles[12 + 6*i + 1]= i + 12;
            triangles[12 + 6*i + 2]= (i + 1)%6 + 6;

            triangles[12 + 6*i + 3] = (i + 1)%6 + 6;
            triangles[12 + 6*i + 4] = i + 12;
            triangles[12 + 6*i + 5] = (i + 1)%6 + 12;
        }
    }


    float Step(float input, int stepCount, float min, float max)
    {
        float step = (max - min)/stepCount;
        float currentStep = min;

        while(currentStep < input)
        {
            currentStep += step;
        }
        Debug.Log(currentStep);
        return currentStep;
    }
}
