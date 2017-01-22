using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridPos
{
    public int x;
    public int y;

    public GridPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public enum GroundGenerationType { Mesh, TiledPrefabs};


public class StepTiler : MonoBehaviour {

    public GroundGenerationType groundGenerationType = GroundGenerationType.Mesh;

    [SerializeField, Range(1, 100)]
    int worldSize = 50;

    [SerializeField]
    int clampMin = -1;

    [SerializeField]
    int clampMax = 5;

    [SerializeField]
    float heightScale = 0.4f;

    [Space(5)]
    [EditorColorLabel("First Perilin Function")]

    [SerializeField]
    float perlinAMagnitude = 1;

    [SerializeField]
    Vector2 perlinAFrequency = Vector2.one;

    [SerializeField, Range(1, 5)]
    float perlinAPower = 1;

    [Space(5)]
    [EditorColorLabel("Second Perilin Function")]

    [SerializeField]
    float perlinBMagnitude = 1;

    [SerializeField]
    Vector2 perlinBFrequency = Vector2.one;

    [SerializeField, Range(1, 5)]
    float perlinBPower = 1;

    [Space(5)]

    [SerializeField, EditorVisibilityDependence("groundGenerationType", (int)GroundGenerationType.TiledPrefabs)]
    Transform prefab;

    [SerializeField, EditorVisibilityDependence("groundGenerationType", (int) GroundGenerationType.TiledPrefabs)]
    Color lowHeight;

    [SerializeField, EditorVisibilityDependence("groundGenerationType", (int)GroundGenerationType.TiledPrefabs)]
    Color highHeight;

    Vector2 seedA;
    Vector2 seedB;

    float midValueA = 0;
    float midValueB = 0;

    Dictionary<GridPos, Transform> grid = new Dictionary<GridPos, Transform>();

    Vector3 planarOffset;
    int[,] topology;


    void Start() {

        SetupMesh();
        Reseed();        
    }

    void SetupMesh()
    {
        if (groundGenerationType == GroundGenerationType.Mesh)
        {
            mesh = new Mesh();
            mesh.name = "Topology";

            mFilt = AddIfNotPresent<MeshFilter>();
            mFilt.mesh = mesh;

            AddIfNotPresent<MeshRenderer>();
        }
    }

    T AddIfNotPresent<T>() where T : Component
    {
        T component = GetComponent<T>();
        if (component != null)
        {
            return component;
        }
        return gameObject.AddComponent<T>();
    }

    public void Reseed()
    {
        float seedSpace = 100000f;
        seedA = new Vector2(Random.value * seedSpace, Random.value * seedSpace);
        seedB = new Vector2(Random.value * seedSpace, Random.value * seedSpace);
        Generate();
    }

    public void Generate() {

        SetupShape();
        midValueA = Mathf.Pow(perlinAMagnitude, perlinAPower) / 2f;
        midValueB = Mathf.Pow(perlinBMagnitude, perlinBPower) / 2f;

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                float elevation = GetElevation(x, y);                
                topology[x, y] = Mathf.RoundToInt(elevation);

                if (groundGenerationType == GroundGenerationType.TiledPrefabs)
                {
                    Color c = Color.Lerp(lowHeight, highHeight, elevation - clampMin / (float)(clampMax - clampMin));
                    Transform t = GetClone(x, y);
                    Vector3 pos = t.transform.position;
                    pos.y = elevation * heightScale;
                    t.transform.position = pos;
                    t.GetComponent<Renderer>().material.color = c;
                }

            }
        }
        if (groundGenerationType == GroundGenerationType.Mesh)
        {
            CreateMeshFromTopology();
        }
    }

    void SetupShape()
    {
        if (topology == null || topology.Length != worldSize)
        {
            planarOffset = new Vector3(worldSize / 2f, 0, worldSize / 2f);
            topology = new int[worldSize, worldSize];
        }
    }

    Transform GetClone(int x, int y)
    {
        GridPos pos = new GridPos(x, y);
        if (grid.ContainsKey(pos))
        {
            return grid[pos];
        }
        else
        {
            Transform t = Instantiate(prefab);
            t.SetParent(transform);
            t.position = Vector3.forward * x + Vector3.right * y;
            t.name = string.Format("Tile {0}, {1}", x, y);
            grid[pos] = t;
            return t;
        }
    }

    float GetElevation(int x, int y)
    {       
        float a = Mathf.Pow(Mathf.Max(Mathf.Epsilon,
           Mathf.PerlinNoise(((seedA.x + x) * perlinAFrequency.x), ((seedA.y + y) * perlinAFrequency.y)) 
             * perlinAMagnitude), perlinAPower) - midValueA;

        float b = Mathf.Pow(Mathf.Max(Mathf.Epsilon,
            Mathf.PerlinNoise(((seedB.x + x) * perlinBFrequency.x), ((seedB.y + y) * perlinBFrequency.y))
             * perlinBMagnitude), perlinBPower) - midValueB;
       
        if (float.IsNaN(a))
        {
            float noise = Mathf.PerlinNoise(((seedA.x + x) * perlinAFrequency.x), ((seedA.y + y) * perlinAFrequency.y));
            Debug.Log(string.Format("Perlin Returns Value Outside Range Error: {0} x={1} y={2}", noise, ((seedA.x + x) * perlinAFrequency.x), ((seedA.y + y) * perlinAFrequency.y)));

        }
        if (float.IsNaN(b))
        {
            float noise = Mathf.PerlinNoise(((seedB.x + x) * perlinBFrequency.x), ((seedB.y + y) * perlinBFrequency.y));
            Debug.Log(string.Format("Perlin B Error: {0} {1} {2}", noise, noise * perlinBMagnitude ,Mathf.Pow(Mathf.Epsilon + noise * perlinBMagnitude, perlinBPower)));
        }
        return Mathf.Clamp(Mathf.Round(a - b), clampMin, clampMax);

    }

    MeshFilter mFilt;
    Mesh mesh;

    void CreateMeshFromTopology()
    {
        int[] tris;
        Vector2[] uv;
        Vector3[] verts = GetMesh(out tris, out uv);

        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
    }

    [SerializeField, EditorVisibilityDependence("groundGenerationType", (int)GroundGenerationType.Mesh)]
    Vector2 uvScaling = Vector2.one;

    Vector3[] GetMesh(out int[] triangles, out Vector2[] uv)
    {
        int tiles = worldSize * worldSize;
        int nVerts = tiles * 4;
        
        Vector3[] verts = new Vector3[nVerts];
        triangles = new int[tiles * 6];
        uv = new Vector2[nVerts];

        Vector2 uvScale = new Vector2(uvScaling.x / worldSize, uvScaling.y / worldSize);


        int i = 0;
        int tri = 0;
        for (int x=0; x<worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++, i += 4, tri += 6)
            {
                verts[i] = new Vector3((x + 1) - planarOffset.x, heightScale * topology[x, y], (y + 1) - planarOffset.z);
                verts[i + 1] = new Vector3((x + 1) - planarOffset.x, heightScale * topology[x, y], y - planarOffset.z);
                verts[i + 2] = new Vector3(x - planarOffset.x, heightScale * topology[x, y], y - planarOffset.z);
                verts[i + 3] = new Vector3(x - planarOffset.x, heightScale * topology[x, y], (y + 1) - planarOffset.z);


                uv[i] = new Vector2(((x + 1) * uvScale.x) % 1, ((y + 1) * uvScale.y) % 1);
                uv[i + 1] = new Vector2(((x + 1) * uvScale.x) % 1 , ((y) * uvScale.y) % 1);
                uv[i + 2] = new Vector2(((x) * uvScale.x) % 1, ((y) * uvScale.y) % 1);
                uv[i + 3] = new Vector2(((x) * uvScale.x) % 1, ((y + 1) * uvScale.y) % 1);

                triangles[tri] = i;
                triangles[tri + 1] = i + 1;
                triangles[tri + 2] = i + 2;
                triangles[tri + 3] = i;
                triangles[tri + 4] = i + 2;
                triangles[tri + 5] = i + 3;

            }
        }

        return verts;
    }
}
