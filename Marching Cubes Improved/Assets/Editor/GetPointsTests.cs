using System;
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

    public void TestGetPoint(Vector3Int position)
    {
        NativeArray<Point> allPoints = points.ToNativeArray();

        NativeArray<Point> actual = MarchingCubesHelperFunctions.GetCorners(position.x, position.y, position.z, allPoints, chunkSize);
        allPoints.Dispose();

        Point[] target = GetPoints(position, points);

        for (int i = 0; i < 8; i++)
        {
            Assert.AreEqual(target[i], actual[i]);
        }

        actual.Dispose();
    }

    private Point[] GetPoints(Vector3Int position, Point[,,] points)
    {
        Point[] output = new Point[8];
        for (int i = 0; i < 8; i++)
        {
            Vector3Int newPosition = position + LookupTables.CubePoints[i];

            if(newPosition.IsBetween(Vector3Int.zero, Vector3Int.one*(chunkSize)))
                output[i] = points[newPosition.x, newPosition.y, newPosition.z];
        }
        return output;
    }

    [Test]
    public void GetPointsTest1()
    {
        TestGetPoint(new Vector3Int(0, 0, 0));
    }

    [Test]
    public void GetPointsTest2()
    {
        TestGetPoint(new Vector3Int(5, 5, 5));
    }

    [Test]
    public void GetPointsTest3()
    {
        TestGetPoint(new Vector3Int(chunkSize, chunkSize, chunkSize));
    }

    [Test]
    public void GetPointsTest4(){
        TestGetPoint(new Vector3Int(chunkSize + 1, chunkSize + 1, chunkSize + 1));
    }

    [Test]
    public void GetPointsTest5(){
        TestGetPoint(new Vector3Int(-3, -3, -3));
    }

    [Test]
    public void GetPointsTest6(){
        TestGetPoint(new Vector3Int(chunkSize - 1, chunkSize - 1, chunkSize - 1));
    }

    [Test]
    public void GetPointsTest7()
    {
        for (int z = 0; z < chunkSize; z++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    TestGetPoint(new Vector3Int(x, y, z));
                }
            }
        }
    }
}
