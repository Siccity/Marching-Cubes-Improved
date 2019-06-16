using NUnit.Framework;
using Unity.Collections;
using UnityEngine;

public class VertexListTests
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

    public void TestVertexList(int x, int y, int z)
    {
        Point[] targetPoints = mc.GetPoints(x, y, z, points);
        NativeArray<Point> nativeCubePoints = targetPoints.ToNativeArray();
        
        int cubeIndex = mc.CalculateCubeIndex(targetPoints, mc._isolevel);
        int edgeIndex = LookupTables.EdgeTable[cubeIndex];

        Vector3[] target = mc.GenerateVertexList(targetPoints, edgeIndex);
        NativeArray<Vector3> actual = MarchingCubesHelperFunctions.GenerateVertexList(nativeCubePoints, edgeIndex, mc._isolevel);

        for (int i = 0; i < target.Length; i++)
        {
            Assert.AreEqual(target[i], actual[i]);
        }

        actual.Dispose();
        nativeCubePoints.Dispose();
    }

    [Test]
    public void VertexListTest1(){
        TestVertexList(0, 0, 0);
    }

    [Test]
    public void VertexListTest2(){
        TestVertexList(4, 8, 8);
    }

    [Test]
    public void VertexListTest3(){
        TestVertexList(3, 7, 5);
    }

    [Test]
    public void VertexListTest4(){
        TestVertexList(chunkSize, chunkSize, chunkSize);
    }

    [Test]
    public void VertexListTest5(){
        TestVertexList(-7, -4, -9);
    }

    [Test]
    public void VertexListTest6(){
        TestVertexList(100, 100, 100);
    }
}
