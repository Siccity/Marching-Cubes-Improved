using NUnit.Framework;
using Unity.Collections;
using UnityEngine;

public class VertexListTests
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

    public void TestVertexList(int x, int y, int z)
    {
        NativeArray<Point> nativePoints = points.ToNativeArray();
        NativeArray<Point> cubePoints = MarchingCubesHelperFunctions.GetCorners(x, y, z, nativePoints, chunkSize);

        int cubeIndex = MarchingCubesHelperFunctions.CalculateCubeIndex(cubePoints, iso);
        int edgeIndex = LookupTables.EdgeTable[cubeIndex];

        NativeArray<Vector3> target = GenerateVertexList(cubePoints, edgeIndex);
        NativeArray<Vector3> actual = MarchingCubesHelperFunctions.GenerateVertexList(cubePoints, edgeIndex, iso);

        for (int i = 0; i < target.Length; i++)
        {
            Assert.AreEqual(target[i], actual[i]);
        }

        nativePoints.Dispose();
        cubePoints.Dispose();

        target.Dispose();
        actual.Dispose();
    }

    private NativeArray<Vector3> GenerateVertexList(NativeArray<Point> targetPoints, int edgeIndex)
    {
        NativeArray<Vector3> vertexList = new NativeArray<Vector3>(12, Allocator.Temp);
        for (int i = 0; i < 12; i++)
        {
            if ((edgeIndex & (1 << i)) != 0)
            {
                int[] edgePair = LookupTables.EdgeIndexTable[i];
                int edge1 = edgePair[0];
                int edge2 = edgePair[1];

                Point point1 = targetPoints[edge1];
                Point point2 = targetPoints[edge2];

                vertexList[i] = MarchingCubesHelperFunctions.VertexInterpolate(point1.localPosition, point2.localPosition, point1.density, point2.density, iso);
            }
        }

        return vertexList;
    }

    [Test]
    public void VertexListTest1()
    {
        TestVertexList(0, 0, 0);
    }

    [Test]
    public void VertexListTest2()
    {
        TestVertexList(4, 8, 8);
    }

    [Test]
    public void VertexListTest3()
    {
        TestVertexList(3, 7, 5);
    }

    [Test]
    public void VertexListTest4()
    {
        TestVertexList(chunkSize, chunkSize, chunkSize);
    }

    [Test]
    public void VertexListTest5()
    {
        TestVertexList(-7, -4, -9);
    }

    [Test]
    public void VertexListTest6()
    {
        TestVertexList(100, 100, 100);
    }

    [Test]
    public void VertexListTest7()
    {
        TestVertexList(chunkSize+1, chunkSize+1, chunkSize+1);
    }
}
