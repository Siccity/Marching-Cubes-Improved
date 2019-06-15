using Unity.Collections;
using UnityEngine;

public static class MarchingCubesHelperFunctions
{
    public static Vector3Int Convert1Dto3D(int i, int maxWidth, int maxHeight)
    {
        int x = i % maxWidth;
        int y = i / maxWidth % maxHeight;
        int z = i / (maxHeight * maxWidth);
        return new Vector3Int(x, y, z);
    }

    public static int Convert3Dto1D(Vector3Int vec, int xMax, int yMax)
    {
        return Convert3Dto1D(vec.x, vec.y, vec.z, xMax, yMax);
    }

    public static int Convert3Dto1D(int x, int y, int z, int xMax, int yMax)
    {
        return (int)((z * xMax * yMax) + (y * xMax) + x);
    }

    public static Vector3 VertexInterpolate(Vector3 p1, Vector3 p2, float v1, float v2, float isolevel)
    {
        if (Utils.Abs(isolevel - v1) < 0.000001f)
        {
            return p1;
        }
        if (Utils.Abs(isolevel - v2) < 0.000001f)
        {
            return p2;
        }
        if (Utils.Abs(v1 - v2) < 0.000001f)
        {
            return p1;
        }

        float mu = (isolevel - v1) / (v2 - v1);

        Vector3 p = p1 + mu * (p2 - p1);

        return p;
    }

    public static NativeArray<Vector3> GenerateVertexList(NativeArray<Point> points, int edgeIndex, float isolevel)
    {
        NativeArray<Vector3> vertexList = new NativeArray<Vector3>(12, Allocator.Temp);

        for (int i = 0; i < 12; i++)
        {
            if ((edgeIndex & (1 << i)) != 0)
            {
                Point point1 = points[LookupTables.EdgeIndexTable[i][0]];
                Point point2 = points[LookupTables.EdgeIndexTable[i][1]];

                vertexList[i] = VertexInterpolate(point1.localPosition, point2.localPosition, point1.density, point2.density, isolevel);
            }
        }

        return vertexList;
    }

    public static int GenerateVertexCount(NativeArray<int> cubeIndices)
    {
        int vertexCount = 0;

        for (int i = 0; i < cubeIndices.Length; i++)
        {
            int cubeIndex = cubeIndices[i];
            vertexCount += LookupTables.TriangleTable[cubeIndex].Length;
        }

        return vertexCount;
    }

    public static NativeArray<int> GenerateCubeIndices(NativeArray<Point> points, int chunkSize, float isolevel)
    {
        NativeArray<int> cubeIndices = new NativeArray<int>(chunkSize * chunkSize * chunkSize, Allocator.Temp);

        for (int i = 0; i < chunkSize * chunkSize * chunkSize; i++)
        {
            NativeArray<Point> cubePoints = GetPoints(i, points, chunkSize);
            int cubeIndex = CalculateCubeIndex(cubePoints, isolevel);
            cubeIndices[i] = cubeIndex;
            cubePoints.Dispose();
        }

        return cubeIndices;
    }

    public static int CalculateCubeIndex(NativeArray<Point> points, float isolevel)
    {
        int cubeIndex = 0;

        for (int i = 0; i < 8; i++)
        {
            if (points[i].density < isolevel)
            {
                cubeIndex |= 1 << i;
            }
        }

        return cubeIndex;
    }

    public static NativeArray<Point> GetPoints(int x, int y, int z, NativeArray<Point> points, int chunkSize)
    {
        int index = Convert3Dto1D(x, y, z, chunkSize + 1, chunkSize + 1);
        return GetPoints(index, points, chunkSize);
    }

    public static NativeArray<Point> GetPoints(int i, NativeArray<Point> points, int chunkSize)
    {
        NativeArray<Point> cubePoints = new NativeArray<Point>(8, Allocator.Temp);

        if (!i.IsBetween(0, points.Length - 1))
        {
            return cubePoints;
        }

        Vector3Int startPos = Convert1Dto3D(i, chunkSize + 1, chunkSize + 1);
        for (int j = 0; j < 8; j++)
        {
            Vector3Int pos = startPos + LookupTables.CubePoints[j];
            if (!pos.IsBetween(Vector3Int.zero, Vector3Int.one * chunkSize))
            {
                continue;
            }

            int newIndex = Convert3Dto1D(pos, chunkSize + 1, chunkSize + 1);
            if (newIndex.IsBetween(0, points.Length - 1))
            {
                cubePoints[j] = points[newIndex];
            }
        }

        return cubePoints;
    }

    public static void March(NativeArray<Point> points, int cubeIndex, ref int vertexIndex, ref NativeArray<Vector3> vertices, float isolevel)
    {
        int edgeIndex = LookupTables.EdgeTable[cubeIndex];

        NativeArray<Vector3> vertexList = GenerateVertexList(points, edgeIndex, isolevel);

        for (int i = 0; i < LookupTables.TriangleTable[cubeIndex].Length; i++)
        {
            vertices[vertexIndex] = vertexList[LookupTables.TriangleTable[cubeIndex][i]];
            vertexIndex++;
        }

        vertexList.Dispose();
    }

    public static NativeArray<Vector3> GenerateMesh(NativeArray<Point> points, int chunkSize, float isolevel, NativeArray<Vector3> vertices)
    {
        NativeArray<int> cubeIndices = GenerateCubeIndices(points, chunkSize, isolevel);

        int vertexCount = GenerateVertexCount(cubeIndices);

        if (vertexCount <= 0)
        {
            cubeIndices.Dispose();
            return vertices;
        }

        int vertexIndex = 0;

        for (int i = 0; i < chunkSize * chunkSize * chunkSize; i++)
        {
            int cubeIndex = cubeIndices[i];
            if (cubeIndex == 0 || cubeIndex == 255)
                continue;

            NativeArray<Point> cubePoints = GetPoints(i, points, chunkSize);
            March(cubePoints, cubeIndex, ref vertexIndex, ref vertices, isolevel);
            cubePoints.Dispose();
        }

        cubeIndices.Dispose();

        return vertices;
    }
}