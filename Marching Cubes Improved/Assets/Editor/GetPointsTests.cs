using NUnit.Framework;
using Unity.Collections;
using UnityEngine;

public class GetPointsTest
{
    private int chunkSize = 16;
    private Point[,,] points;
    private DensityGenerator dg;

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

    public Point[] GetTarget(int x, int y, int z)
    {
        Point[] target = new Point[8];

        for (int i = 0; i < 8; i++)
        {
            int newX = x + LookupTables.CubePoints[i].x;
            int newY = y + LookupTables.CubePoints[i].y;
            int newZ = z + LookupTables.CubePoints[i].z;

            if (newX.IsBetween(0, chunkSize) && newY.IsBetween(0, chunkSize) && newZ.IsBetween(0, chunkSize))
            {
                target[i] = points[newX, newY, newZ];
            }
        }

        return target;
    }

    [Test]
    public void GetPointsTest1()
    {
        NativeArray<Point> allPoints = points.ToNativeArray();

        Vector3Int position = new Vector3Int(0, 0, 0);

        NativeArray<Point> actual = MarchingCubesHelperFunctions.GetPoints(position.x, position.y, position.z, allPoints, chunkSize);
        allPoints.Dispose();

        Point[] target = GetTarget(position.x, position.y, position.z);
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

        Point[] target = GetTarget(position.x, position.y, position.z);
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

        Point[] target = GetTarget(position.x, position.y, position.z);
        for (int i = 0; i < 8; i++)
        {
            Assert.AreEqual(target[i], actual[i]);
        }

        actual.Dispose();
    }

    [Test]
    public void GetPointsTest4()
    {
        NativeArray<Point> allPoints = points.ToNativeArray();

        Vector3Int position = new Vector3Int(chunkSize + 1, chunkSize + 1, chunkSize + 1);

        NativeArray<Point> actual = MarchingCubesHelperFunctions.GetPoints(position.x, position.y, position.z, allPoints, chunkSize);
        allPoints.Dispose();

        Point[] target = GetTarget(position.x, position.y, position.z);
        for (int i = 0; i < 8; i++)
        {
            Assert.AreEqual(target[i], actual[i]);
        }

        actual.Dispose();
    }

    [Test]
    public void GetPointsTest5()
    {
        NativeArray<Point> allPoints = points.ToNativeArray();

        Vector3Int position = new Vector3Int(-3, -3, -3);

        NativeArray<Point> actual = MarchingCubesHelperFunctions.GetPoints(position.x, position.y, position.z, allPoints, chunkSize);
        allPoints.Dispose();

        Point[] target = GetTarget(position.x, position.y, position.z);
        for (int i = 0; i < 8; i++)
        {
            Assert.AreEqual(target[i], actual[i]);
        }

        actual.Dispose();
    }

    [Test]
    public void GetPointsTest6()
    {
        NativeArray<Point> allPoints = points.ToNativeArray();

        Vector3Int position = new Vector3Int(chunkSize-1, chunkSize-1, chunkSize-1);

        NativeArray<Point> actual = MarchingCubesHelperFunctions.GetPoints(position.x, position.y, position.z, allPoints, chunkSize);
        allPoints.Dispose();

        Point[] target = GetTarget(position.x, position.y, position.z);
        for (int i = 0; i < 8; i++)
        {
            Assert.AreEqual(target[i], actual[i]);
        }

        actual.Dispose();
    }
}
