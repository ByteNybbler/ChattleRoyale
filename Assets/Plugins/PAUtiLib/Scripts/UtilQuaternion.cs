// Author(s): Paul Calande
// Helper fuctions for working with quaternions.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilQuaternion
{
    // Returns the difference between two quaternions.
    public static Quaternion Difference(Quaternion start, Quaternion end)
    {
        return Quaternion.Inverse(start) * end;
    }

    // Scales a quaternion by the given factor.
    // Scales above 1 might become inaccurate.
    public static Quaternion Scale(Quaternion toScale, float factor)
    {
        return Quaternion.SlerpUnclamped(Quaternion.identity, toScale, factor);
    }
}