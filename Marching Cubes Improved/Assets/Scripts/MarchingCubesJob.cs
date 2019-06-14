using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct MarchingCubesJob : IJob
{
    // ------------ INPUTS ----------------
    [ReadOnly] public NativeArray<Point> points;
    [ReadOnly] public int chunkSize;
    [ReadOnly] public float isolevel;
    // ------------------------------------

    // ------------ OUTPUTS ---------------
    public NativeArray<Vector3> vertices;
    public NativeArray<int> triangles;
    // ------------------------------------

    public void Execute()
    {
        vertices = MarchingCubesHelperFunctions.GenerateMesh(points, chunkSize, isolevel, vertices);
        for (int i = 0; i < vertices.Length; i++)
        {
            triangles[i] = i;
        }
    }
}