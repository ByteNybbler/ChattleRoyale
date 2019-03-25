// Author(s): Paul Calande
// An interval between two Quaternions.
// Similar to IntervalFloat, but with Quaternions instead of floats.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntervalQuaternion
{
    [SerializeField]
    [Tooltip("The start of the interval.")]
    private Quaternion start;
    [SerializeField]
    [Tooltip("The end of the interval.")]
    private Quaternion end;

    // Private constructors mean that this class can only be constructed
    // from the factory methods.
    private IntervalQuaternion(Quaternion start, Quaternion end)
    {
        this.start = start;
        this.end = end;
    }

    // Factory methods.
    public static IntervalQuaternion FromStartEnd(Quaternion start, Quaternion end)
    {
        return new IntervalQuaternion(start, end);
    }
    public static IntervalQuaternion FromCenterRadius(Quaternion center, Quaternion radius)
    {
        return new IntervalQuaternion(center * Quaternion.Inverse(radius), center * radius);
    }
    public static IntervalQuaternion FromCenterDiameter(Quaternion center, Quaternion diameter)
    {
        return FromCenterRadius(center, UtilQuaternion.Scale(diameter, 0.5f));
    }
    public static IntervalQuaternion FromRadius(Quaternion radius)
    {
        return FromCenterRadius(Quaternion.identity, radius);
    }
    public static IntervalQuaternion FromDiameter(Quaternion diameter)
    {
        return FromRadius(UtilQuaternion.Scale(diameter, 0.5f));
    }

    // Returns the start of the interval.
    public Quaternion GetStart()
    {
        return start;
    }
    // Returns the end of the interval.
    public Quaternion GetEnd()
    {
        return end;
    }
    // Returns the diameter of the interval.
    public Quaternion GetDiameter()
    {
        return UtilQuaternion.Difference(start, end);
    }
    // Returns the radius of the interval.
    public Quaternion GetRadius()
    {
        return UtilQuaternion.Scale(GetDiameter(), 0.5f);
    }
    // Returns the Slerp between the interval's start and end by t.
    public Quaternion Slerp(float t)
    {
        return Quaternion.Slerp(start, end, t);
    }
    // Returns the center of the interval.
    public Quaternion GetCenter()
    {
        return Slerp(0.5f);
    }
    // Sets the center of the interval while preserving the diameter.
    public void SetCenter(Quaternion center)
    {
        Quaternion radius = GetRadius();
        start = center * Quaternion.Inverse(radius);
        end = center * radius;
    }
    // Returns a random quaternion from the interval.
    public Quaternion GetRandom()
    {
        return Slerp(Random.Range(0.0f, 1.0f));
    }
}