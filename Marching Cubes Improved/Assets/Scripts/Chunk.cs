using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [HideInInspector] public bool readyForUpdate;
    [HideInInspector] public Point[,,] points;
    [HideInInspector] public int chunkSize;
    [HideInInspector] public Vector3Int position;

    private float _isolevel;
    private int _seed;

    //private MarchingCubes _marchingCubes;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private DensityGenerator _densityGenerator;
    private World _world;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
    }

    private void Start()
    {
        Generate();
    }

    private void Update()
    {
        if (readyForUpdate)
        {
            Generate();
            readyForUpdate = false;
        }
    }

    public void Initialize(World world, int chunkSize, Vector3Int position)
    {
        this.chunkSize = chunkSize;
        this.position = position;
        _isolevel = world.isolevel;
        _world = world;

        _densityGenerator = world.densityGenerator;

        int worldPosX = position.x;
        int worldPosY = position.y;
        int worldPosZ = position.z;

        points = new Point[chunkSize + 1, chunkSize + 1, chunkSize + 1];

        _seed = world.seed;

        for (int z = 0; z < points.GetLength(2); z++)
        {
            for (int y = 0; y < points.GetLength(1); y++)
            {
                for (int x = 0; x < points.GetLength(0); x++)
                {
                    points[x, y, z] = new Point(
                        new Vector3Int(x, y, z),
                        _densityGenerator.CalculateDensity(x + worldPosX, y + worldPosY, z + worldPosZ)
                    );
                }
            }
        }
    }

    public void Generate()
    {
        Mesh mesh = CreateMesh();

        _meshFilter.sharedMesh = mesh;
        _meshCollider.sharedMesh = mesh;
    }

    Mesh CreateMesh()
    {
        NativeArray<Point> points = this.points.ToNativeArray(Allocator.TempJob);

        NativeArray<int> cubeIndices = MarchingCubesHelperFunctions.GenerateCubeIndices(points, chunkSize, _isolevel);

        int vertexCount = MarchingCubesHelperFunctions.GenerateVertexCount(cubeIndices);

        NativeArray<Vector3> vertices = new NativeArray<Vector3>(vertexCount, Allocator.TempJob);
        NativeArray<int> triangles = new NativeArray<int>(vertexCount, Allocator.TempJob);

        MarchingCubesJob job = new MarchingCubesJob()
        {
            points = points,
            chunkSize = chunkSize,
            isolevel = _isolevel,

            vertices = vertices,
            triangles = triangles
        };

        JobHandle jobHandle = job.Schedule();

        Mesh mesh = new Mesh();
        cubeIndices.Dispose();

        jobHandle.Complete();

        points.Dispose();

        Vector3[] meshVertices = vertices.ToArray();
        vertices.Dispose();

        int[] meshTriangles = triangles.ToArray();
        triangles.Dispose();

        mesh.vertices = meshVertices;
        mesh.triangles = meshTriangles;
        
        mesh.RecalculateNormals();

        return mesh;
    }

    public Point GetPoint(int x, int y, int z)
    {
        return points[x, y, z];
    }

    public void SetDensity(float density, int x, int y, int z)
    {
        points[x, y, z].density = density;
    }

    public void SetDensity(float density, Vector3Int pos)
    {
        SetDensity(density, pos.x, pos.y, pos.z);
    }
}
