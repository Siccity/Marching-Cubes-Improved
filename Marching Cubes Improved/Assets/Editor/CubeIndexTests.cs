using NUnit.Framework;
using Unity.Collections;
using UnityEngine;

public class CubeIndexTests
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

    void CubeIndexTest(int x, int y, int z)
    {
        Point[] points = mc.GetPoints(x, y, z, this.points);
        int target = mc.CalculateCubeIndex(points, mc._isolevel);

        NativeArray<Point> nativePoints = points.ToNativeArray();
        int actual = MarchingCubesHelperFunctions.CalculateCubeIndex(nativePoints, mc._isolevel);

        Assert.AreEqual(target, actual);
    }

    [Test]
    public void CubeIndexTest1()
    {
        CubeIndexTest(0, 0, 0);
    }

    [Test]
    public void CubeIndexTest2()
    {
        CubeIndexTest(5, 0, 0);
    }

    [Test]
    public void CubeIndexTest3()
    {
        CubeIndexTest(chunkSize, chunkSize, chunkSize);
    }

    [Test]
    public void CubeIndexTest4()
    {
        float iso = mc._isolevel;

        int[,,] targetIndices = mc.GenerateCubeIndexes(points);

        NativeArray<Point> allPoints = points.ToNativeArray();

        NativeArray<int> target = targetIndices.ToNativeArray();
        NativeArray<int> actual = MarchingCubesHelperFunctions.GenerateCubeIndices(allPoints, chunkSize, iso);

        for (int i = 0; i < target.Length; i++)
        {
            int targetIndex = target[i];
            int actualIndex = actual[i];

            Assert.AreEqual(targetIndex, actualIndex);
        }
        
        allPoints.Dispose();

        target.Dispose();
        actual.Dispose();
    }
}