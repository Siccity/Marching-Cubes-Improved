using System.Collections.Generic;
using UnityEngine;

public struct Point
{
    public Vector3Int localPosition;
    public float density;
    public bool initialized; // This is quite hacky, remove this later if possible

    public Point(Vector3Int localPosition, float density)
    {
        this.localPosition = localPosition;
        this.density = density;
        initialized = true;
    }

    public override bool Equals(object obj)
    {
        if (obj is Point)
        {
            Point p = (Point) obj;
            return localPosition == p.localPosition && density == p.density && initialized == p.initialized;
        }

        return false;
    }

    public override int GetHashCode()
    {
        var hashCode = 1771454171;
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector3Int>.Default.GetHashCode(localPosition);
        hashCode = hashCode * -1521134295 + density.GetHashCode();
        hashCode = hashCode * -1521134295 + initialized.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        return $"{localPosition.x}/{localPosition.y}/{localPosition.z} : {density}";
    }
}