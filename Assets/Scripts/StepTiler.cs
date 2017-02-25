using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GridPos
{
    public int x;
    public int y;

    public GridPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    static public explicit operator Vector3(GridPos pos)
    {
        return new Vector3(pos.x, 0, pos.y);
    }

    static public explicit operator Vector2(GridPos pos)
    {
        return new Vector3(pos.x, pos.y);
    }

    public override string ToString()
    {
        return string.Format("GridPos ({0}, {1})", x, y);
    }

    public GridPos inverted
    {
        get
        {
            return new GridPos(y, x);
        }
    }
}

[System.Serializable]
public struct GridRect
{
    public GridPos min;
    public GridPos max;

    public GridRect(int minX, int minY, int w, int h)
    {
        min = new GridPos(minX, minY);
        max = new GridPos(minX + w, minY + h);
    }

    public Vector2 Center
    {
        get
        {
            return ((Vector2)min + (Vector2)max) * 0.5f;
        }
    }

    public override string ToString()
    {
        return string.Format("GridRect ({0}, {1}) - ({2}, {3})", min.x, min.y, max.x, max.y);
    }
}

[System.Serializable]
public class OutsideMapException : System.Exception
{
    public OutsideMapException() { }
    public OutsideMapException(string message) : base(message) { }
    public OutsideMapException(string message, System.Exception inner) : base(message, inner) { }
    protected OutsideMapException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context)
    { }
};

public enum GroundGenerationType { Mesh, TiledPrefabs};

public delegate void WorldGenerated(StepTiler world);

public class StepTiler : MonoBehaviour {

    static StepTiler _instance;
    public static StepTiler instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<StepTiler>();
            }
            return _instance;
        }
    }

    public static event WorldGenerated OnNewWorld;

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
    bool[,] occupancy;

    public void Occupy(GridRect gRect)
    {
        GridPos min = gRect.min;
        GridPos max = gRect.max;

        for (int x=min.x; x<max.x; x++)
        {
            for (int y=min.y; y<max.y; y++)
            {
                occupancy[x, y] = true;
            }
        }
    }

    public IEnumerable<GridRect> GetAllPositions(int w, int h, params int[] levels)
    {
        int maxX = topology.GetLength(0) - w;
        int maxY = topology.GetLength(1) - h;
        int l = levels.Length;
        for (int x=0; x<maxX; x++)
        {
            for (int y=0; y<maxY; y++)
            {
                bool validPos = true;

                for (int offX=0; offX< w; offX++)
                {
                    for (int offY=0; offY< h; offY++)
                    {
                        int lvl = topology[x + offX, y + offY];
                        bool coordFound = false;

                        if (!occupancy[x + offX, y + offY])
                        {
                            for (int idL = 0; idL < l; idL++)
                            {
                                if (lvl == levels[idL])
                                {
                                    coordFound = true;
                                    break;
                                }
                            }
                        }
                        if (!coordFound)
                        {
                            validPos = false;
                            break;
                        }
                    }

                    if (!validPos)
                    {
                        break;
                    }
                }
                
                if (validPos)
                {
                    yield return new GridRect(x, y, w, h);
                }
            }
        }
    }

    public IEnumerable<GridRect> GetAllPaddedPositions(int w, int h, int edgeDistance, params int[] levels)
    {
        int maxX = topology.GetLength(0) - w - edgeDistance;
        int maxY = topology.GetLength(1) - h - edgeDistance;
        int l = levels.Length;
        for (int x = edgeDistance; x < maxX; x++)
        {
            for (int y = edgeDistance; y < maxY; y++)
            {
                bool validPos = true;

                for (int offX = 0; offX < w; offX++)
                {
                    for (int offY = 0; offY < h; offY++)
                    {
                        int lvl = topology[x + offX, y + offY];
                        bool coordFound = false;
                        if (!occupancy[x + offX, y + offY])
                        {
                            for (int idL = 0; idL < l; idL++)
                            {
                                if (lvl == levels[idL])
                                {
                                    coordFound = true;
                                    break;
                                }
                            }
                        }
                        if (!coordFound)
                        {
                            validPos = false;
                            break;
                        }
                    }

                    if (!validPos)
                    {
                        break;
                    }
                }

                if (validPos)
                {
                    yield return new GridRect(x, y, w, h);
                }
            }
        }
    }

    public Vector3 GetGroundAt(Vector3 worldPosition)
    {
        Vector3 pos = transform.InverseTransformPoint(worldPosition) + planarOffset;
        float top;
        try {
            top = topology[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z)];
        } catch (System.IndexOutOfRangeException)
        {
            throw new OutsideMapException();
        } 

        return transform.TransformPoint(worldPosition.x, top * heightScale, worldPosition.z);
    }

    public Vector3 GridPositionToWorld(GridPos pos)
    {
        try {
            return transform.TransformPoint(new Vector3(0.5f + pos.x - planarOffset.x, heightScale * topology[pos.x, pos.y], 0.5f + pos.y - planarOffset.z));
        } catch (System.IndexOutOfRangeException)
        {
            throw new OutsideMapException();
        }
    }

    public Vector3 GridPositionToWorld(Vector2 pos)
    {
        //SmarterThingy for handlng jumps needed
        float top = 0;
        try {
            top = topology[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)];
        } catch (System.IndexOutOfRangeException)
        {
            throw new OutsideMapException();
        }
        return transform.TransformPoint(new Vector3(pos.x - planarOffset.x, heightScale * top, pos.y - planarOffset.z));
    }

    public Vector3 GridRectToWorld(GridRect gridRect)
    {
        Vector3 pos = (GridPositionToWorld(gridRect.min) + GridPositionToWorld(gridRect.max));
        return pos * 0.5f;
    }

    public Vector2 WorldToFloatPosition(Vector3 world)
    {
        Vector3 pos = transform.InverseTransformPoint(world);
        return new Vector2(pos.x + planarOffset.x, pos.z + planarOffset.z);
    }

    public float WorldPosHeight(Vector3 pos)
    {
        Vector3 localPos = transform.InverseTransformPoint(pos);
        try
        {
            return heightScale * topology[(int)(localPos.x + planarOffset.x), (int)(localPos.z + planarOffset.z)];
        } catch (System.IndexOutOfRangeException)
        {
            throw new OutsideMapException();
        }
    }

    public Bounds GridWithSizeToWorldBounds(Vector2 center, int size, bool rounded=false)
    {
        Vector2 min = center - Vector2.one * size / 2f;
        Vector2 max = center + Vector2.one * size / 2f;

        if (rounded)
        {
            min.x = Mathf.Floor(min.x);
            min.y = Mathf.Floor(min.y);
            max.x = Mathf.Ceil(max.x);
            max.y = Mathf.Ceil(max.y);
        }

        int x0 = (int) min.x;
        int x1 = (int) max.x;
        int y0 = (int) min.y;
        int y1 = (int) max.y;

        int min_height = 0;
        if (x0 >= 0 && x0 < topology.GetLength(0) && y0 >= 0 && y0 < topology.GetLength(1))
        {
            min_height = topology[x0, y0];
        } else if (x1 >= 0 && x1 < topology.GetLength(0) && y1 >= 0 && y1 < topology.GetLength(1))
        {
            min_height = topology[x1, y1];
        }

        int max_height = min_height;

        for (int x=x0; x<x1; x++)
        {
            for (int y=y0; y<y1; y++)
            {
                int v = topology[x, y];
                if (v < min_height)
                {
                    min_height = v;
                } else if (v > max_height)
                {
                    max_height = v;
                }
            }
        }

        Vector3 minPos = transform.TransformPoint(new Vector3(min.x - planarOffset.x, heightScale * min_height, min.y - planarOffset.z));
        Vector3 maxPos = transform.TransformPoint(new Vector3(max.x - planarOffset.x, heightScale * max_height, max.y -  planarOffset.z));

        return new Bounds((maxPos + minPos) / 2f, maxPos - minPos);
        
    }

    void Start() {

        SetupMesh();
        Reseed();        
    }

    BoxCollider bCollider;

    void SetupMesh()
    {
        if (groundGenerationType == GroundGenerationType.Mesh)
        {
            mesh = new Mesh();
            mesh.name = "Topology";

            mFilt = AddIfNotPresent<MeshFilter>();
            mFilt.mesh = mesh;

            AddIfNotPresent<MeshRenderer>();
            bCollider = GetComponent<BoxCollider>();
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

    Transform tileParent;

    public void Generate() {

        SetupShape();
        midValueA = Mathf.Pow(perlinAMagnitude, perlinAPower) / 2f;
        midValueB = Mathf.Pow(perlinBMagnitude, perlinBPower) / 2f;

        if (groundGenerationType == GroundGenerationType.TiledPrefabs && tileParent == null)
        {
            GameObject tileParentGO = new GameObject("Tiles");
            tileParent = tileParentGO.transform;
            tileParent.SetParent(transform, false);
            tileParent.localPosition = Vector3.zero;
        }

        for (int y = 0; y < worldSize; y++)
        {
            for (int x = 0; x < worldSize; x++)
            {
                float elevation = GetElevation(x, y);                

                topology[y, x] = Mathf.RoundToInt(elevation);
                occupancy[y, x] = false;

                if (groundGenerationType == GroundGenerationType.TiledPrefabs)
                {
                    Color c = Color.Lerp(lowHeight, highHeight, elevation - clampMin / (float)(clampMax - clampMin));
                    Transform t = GetClone(x, y);
                    Vector3 pos = t.transform.position;
                    pos.y = transform.TransformPoint(new Vector3(0, elevation * heightScale, 0)).y;
                    t.transform.position = pos;
                    Renderer r = t.GetComponent<Renderer>();
                    r.material.color = c;
                    r.material.SetColor("_HighlightColor", c);
                    
                }

            }
        }
        if (groundGenerationType == GroundGenerationType.Mesh)
        {
            CreateMeshFromTopology();
        }

        if (OnNewWorld != null)
        {
            OnNewWorld(this);
        }
    }

    void SetupShape()
    {
        if (topology == null || topology.Length != worldSize)
        {
            planarOffset = new Vector3(worldSize / 2f, 0, worldSize / 2f);
            topology = new int[worldSize, worldSize];
            occupancy = new bool[worldSize, worldSize];
        }
    }

    [SerializeField, Range(0, 1)]
    float colorBlending = 0.25f;

    public void SetColor(Vector2 pos, Color color)
    {
        SetColor(new GridPos((int)pos.x, (int)pos.y), color);
    }

    public void SetColor(GridPos pos, Color color)
    {
        if (groundGenerationType == GroundGenerationType.TiledPrefabs)
        {
            Renderer r = grid[pos].GetComponent<Renderer>();
            r.material.SetColor("_HighlightColor", color);
            r.material.SetFloat("_HighlightTime", Time.timeSinceLevelLoad);
        }
    }

    Transform GetClone(int x, int y)
    {
        GridPos pos = new GridPos(x, y).inverted;
        if (grid.ContainsKey(pos))
        {
            return grid[pos];
        }
        else
        {
            Transform t = Instantiate(prefab);
            t.SetParent(tileParent);
            t.position = Vector3.forward * (x + 0.5f) + Vector3.right * (y + 0.5f) - planarOffset;
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

        if (bCollider)
        {
            bCollider.size = new Vector3(worldSize, 1, worldSize);
        }
        
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

    [SerializeField]
    bool debugDrawGizmo = true;

    void OnDrawGizmosSelected()
    {
        if (debugDrawGizmo && topology != null)
        {
            Gizmos.color = Color.magenta;
            int i = 0;
            int tri = 0;
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++, i += 4, tri += 6)
                {
                    Vector3 A = transform.TransformPoint(new Vector3((x + 1) - planarOffset.x, heightScale * topology[x, y], (y + 1) - planarOffset.z));
                    Vector3 B = transform.TransformPoint(new Vector3((x + 1) - planarOffset.x, heightScale * topology[x, y], y - planarOffset.z));
                    Vector3 C = transform.TransformPoint(new Vector3(x - planarOffset.x, heightScale * topology[x, y], y - planarOffset.z));
                    Vector3 D = transform.TransformPoint(new Vector3(x - planarOffset.x, heightScale * topology[x, y], (y + 1) - planarOffset.z));
                    Gizmos.DrawLine(A, B);
                    Gizmos.DrawLine(B, C);
                    Gizmos.DrawLine(C, D);
                    Gizmos.DrawLine(D, A);
                }
            }
        }
    }
}
