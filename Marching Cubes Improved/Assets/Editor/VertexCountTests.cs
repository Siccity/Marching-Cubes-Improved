using NUnit.Framework;
using UnityEngine;
using Unity.Collections;

public class VertexCountTests
{
    private int chunkSize = 16;
    private Point[,,] points;
    private DensityGenerator dg;
    private MarchingCubes mc;

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
        mc = new MarchingCubes(points, 0.5f, 0);
    }

    [Test]
    public void VertexCountTest1(){
        int[,,] targetCubeIndices = mc.GenerateCubeIndexes(points);
        int targetVertexCount = mc.GenerateVertexCount(targetCubeIndices);

        NativeArray<int> nativeTargetCubeIndices = targetCubeIndices.ToNativeArray();
        int actualVertexCount = MarchingCubesHelperFunctions.GenerateVertexCount(nativeTargetCubeIndices);

        Assert.AreEqual(targetVertexCount, actualVertexCount);

        nativeTargetCubeIndices.Dispose();
    }
}
