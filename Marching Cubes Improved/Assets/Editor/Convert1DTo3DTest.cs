using NUnit.Framework;
using UnityEngine;

public class Convert1DTo3DTest
{
    [Test]
    public void Convert1DTo3DTest1()
    {
        Vector3Int vec = MarchingCubesHelperFunctions.Convert1Dto3D(0, 16, 16);
        Assert.AreEqual(new Vector3Int(0, 0, 0), vec);
    }

    [Test]
    public void Convert1DTo3DTest2()
    {
        Vector3Int vec = MarchingCubesHelperFunctions.Convert1Dto3D(256, 16, 16);
        Assert.AreEqual(new Vector3Int(0, 0, 1), vec);
    }

    [Test]
    public void Convert1DTo3DTest3()
    {
        Vector3Int vec = MarchingCubesHelperFunctions.Convert1Dto3D(33, 16, 16);
        Assert.AreEqual(new Vector3Int(1, 2, 0), vec);
    }

    [Test]
    public void Convert1DTo3DTest4()
    {
        Vector3Int vec = MarchingCubesHelperFunctions.Convert1Dto3D(16, 16, 16);
        Assert.AreEqual(new Vector3Int(0, 1, 0), vec);
    }

    [Test]
    public void Convert1DTo3DTest5()
    {
        Vector3Int vec = MarchingCubesHelperFunctions.Convert1Dto3D(1042, 16, 16);
        Assert.AreEqual(new Vector3Int(2, 1, 4), vec);
    }
}