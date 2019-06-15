using NUnit.Framework;
using Unity.Collections;
using UnityEngine;

public class GetPointsTest
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
    public void GetPointsTest1()
    {
        NativeArray<Point> allPoints = points.ToNativeArray();

        Vector3Int position = new Vector3Int(0, 0, 0);

        NativeArray<Point> actual = MarchingCubesHelperFunctions.GetPoints(position.x, position.y, position.z, allPoints, chunkSize);
        allPoints.Dispose();

        Point[] target = mc.GetPoints(position.x, position.y, position.z, points);

        for (int i = 0; i < 8; i++)
        {
            Assert.AreEqual(target[i], actual[i]);
        }

        actual.Dispose();
    }

    [Test]
    public void GetPointsTest2()
    {
        NativeArray<Point> allPoints = points.ToNativeArray();

        Vector3Int position = new Vector3Int(5, 5, 5);

        NativeArray<Point> actual = MarchingCubesHelperFunctions.GetPoints(position.x, position.y, position.z, allPoints, chunkSize);
        allPoints.Dispose();

        Point[] target = mc.GetPoints(position.x, position.y, position.z, points);

        for (int i = 0; i < 8; i++)
        {
            Assert.AreEqual(target[i], actual[i]);
        }

        actual.Dispose();
    }

    [Test]
    public void GetPointsTest3()
    {
        NativeArray<Point> allPoints = points.ToNativeArray();

        Vector3Int position = new Vector3Int(chunkSize, chunkSize, chunkSize);

        NativeArray<Point> actual = MarchingCubesHelperFunctions.GetPoints(position.x, position.y, position.z, allPoints, chunkSize);
        allPoints.Dispose();

        Point[] target = mc.GetPoints(position.x, position.y, position.z, points);

        for (int i = 0; i < 8; i++)
        {
            if(actual[i].initialized)
                Assert.AreEqual(target[i], actual[i]);
        }

        actual.Dispose();
    }

    //TODO: Implement this test

    //[Test]
    //public void GetPointsTest4()
    //{
    //    NativeArray<Point> allPoints = points.ToNativeArray(chunkSize);

    //    Vector3Int position = new Vector3Int(chunkSize+1, chunkSize+1, chunkSize+1);

    //    NativeArray<Point> actual = MarchingCubesHelperFunctions.GetPoints(position.x, position.y, position.z, allPoints, chunkSize);
    //    allPoints.Dispose();

    //    Point[] target = MarchingCubes.GetPoints(position.x, position.y, position.z, points);

    //    for (int i = 0; i < 8; i++)
    //    {
    //        Assert.AreEqual(target[i], actual[i]);
    //    }

    //    actual.Dispose();
    //}
}
