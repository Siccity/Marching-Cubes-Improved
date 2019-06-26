using NUnit.Framework;
using Unity.Collections;
using UnityEngine;

public class CubeIndexTests
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

    int CalculateCubeIndex(NativeArray<Point> points, float iso){
        int cubeIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            if(points[i].density < iso){
                cubeIndex |= 1 << i;
            }
        }
        return cubeIndex;
    }

    void CubeIndexTest(int x, int y, int z)
    {
        NativeArray<Point> nativePoints = points.ToNativeArray();

        int target = CalculateCubeIndex(nativePoints, iso);
        int actual = MarchingCubesHelperFunctions.CalculateCubeIndex(nativePoints, iso);

        Assert.AreEqual(target, actual);

        nativePoints.Dispose();
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
        NativeArray<Point> allPoints = points.ToNativeArray();

        NativeArray<int> actual = MarchingCubesHelperFunctions.GenerateCubeIndices(allPoints, chunkSize, iso);

        int[] target = new int[chunkSize * chunkSize * chunkSize];
        for (int i = 0; i < target.Length; i++)
        {
            Vector3Int p = MarchingCubesHelperFunctions.Convert1Dto3D(i, chunkSize, chunkSize);
            var cubePoints = MarchingCubesHelperFunctions.GetCorners(p, allPoints, chunkSize);
            target[i] = CalculateCubeIndex(cubePoints, iso);
        }

        for (int i = 0; i < target.Length; i++)
        {
            int targetIndex = target[i];
            int actualIndex = actual[i];

            Assert.AreEqual(targetIndex, actualIndex);
        }
        
        allPoints.Dispose();

        actual.Dispose();
    }
}