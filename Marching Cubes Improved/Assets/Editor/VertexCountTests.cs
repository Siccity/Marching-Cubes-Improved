using NUnit.Framework;
using UnityEngine;
using Unity.Collections;

public class VertexCountTests
{
    private int chunkSize = 16;
    private Point[,,] points;
    private DensityGenerator dg;
    private float iso = 0.5f;

    [SetUp]
    public void Setup()
    {
        dg = new DensityGenerator(0);

        points = new Point[chunkSize + 1, chunkSize + 1, chunkSize + 1];
        for (int z = 0; z < chunkSize + 1; z++)
        {
            for (int y = 0; y < chunkSize + 1; y++)
            {
                for (int x = 0; x < chunkSize + 1; x++)
                {
                    Point p = new Point(new Vector3Int(x, y, z), dg.TerrainDensity(x, y, z, 8));

                    points[x, y, z] = p;
                }
            }
        }
    }

    int CalculateVertexCount(NativeArray<int> cubeIndices){
        int vertexCount = 0;
        for (int i = 0; i < cubeIndices.Length; i++)
        {
            vertexCount += LookupTables.TriangleTable[cubeIndices[i]].Length;
        }
        return vertexCount;
    }

    [Test]
    public void VertexCountTest1(){
        var allPoints = points.ToNativeArray();
        var targetCubeIndices = MarchingCubesHelperFunctions.GenerateCubeIndices(allPoints, chunkSize, iso);
        int targetVertexCount = CalculateVertexCount(targetCubeIndices);

        int actualVertexCount = MarchingCubesHelperFunctions.GenerateVertexCount(targetCubeIndices);

        Assert.AreEqual(targetVertexCount, actualVertexCount);

        allPoints.Dispose();
        targetCubeIndices.Dispose();
    }
}
