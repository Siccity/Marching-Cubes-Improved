using NUnit.Framework;
using UnityEngine;

public class Convert3DTo1DTest
{
    [Test]
    public void Convert3DTo1DTest1()
    {
        int index = MarchingCubesHelperFunctions.Convert3Dto1D(new Vector3Int(0,0,0), 16, 16);
        Assert.AreEqual(0, index);
    }

    [Test]
    public void Convert3DTo1DTest2()
    {
        int index = MarchingCubesHelperFunctions.Convert3Dto1D(new Vector3Int(0, 0, 1), 16, 16);
        Assert.AreEqual(256, index);
    }

    [Test]
    public void Convert3DTo1DTest3()
    {
        int index = MarchingCubesHelperFunctions.Convert3Dto1D(new Vector3Int(1, 2, 0), 16, 16);
        Assert.AreEqual(33, index);
    }

    [Test]
    public void Convert3DTo1DTest4()
    {
        int index = MarchingCubesHelperFunctions.Convert3Dto1D(new Vector3Int(0, 1, 0), 16, 16);
        Assert.AreEqual(16, index);
    }

    [Test]
    public void Convert3DTo1DTest5()
    {
        int index = MarchingCubesHelperFunctions.Convert3Dto1D(new Vector3Int(2, 1, 4), 16, 16);
        Assert.AreEqual(1042, index);
    }
}