// Author(s): Paul Calande
// Replication of the swizzling functionality from GLSL.
// Swizzling is the rearrangement of the elements of a vector.
// This class is capable of compressing larger vectors into smaller ones.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Swizzle
{
    // Returns the value of the given component of the given vector.
    // The hyphen character will return currentValue.
    // There are also characters for working with colors.
    private static float GetComponent(Vector4 vector, char component,
        float currentValue = 0.0f)
    {
        switch (component)
        {
            case 'x':
            case 'r':
                return vector.x;
            case 'y':
            case 'g':
                return vector.y;
            case 'z':
            case 'b':
                return vector.z;
            case 'w':
            case 'a':
                return vector.w;
            case '_':
                return currentValue;
            case '*':
                return -currentValue;
            case '1':
                return 1.0f;
            case '0':
            default:
                return 0.0f;
            case '-':
                return -1.0f;
        }
    }

    // Returns the components of a vector in the given swizzle order.
    // vectorToChange is presumably the vector to be modified by this swizzle.
    private static Vector4 ReadComponents(Vector4 vectorToRead, string components,
        Vector4 vectorToChange, int iterations)
    {
        Vector4 result = Vector4.zero;
        for (int i = 0; i < iterations; ++i)
        {
            result[i] = GetComponent(vectorToRead, components[i], vectorToChange[i]);
        }
        return result;
    }

    public static Vector2 Vec2(Vector4 vectorToRead, string components,
        Vector4 vectorToChange)
    {
        return ReadComponents(vectorToRead, components, vectorToChange, 2);
    }
    public static Vector2 Vec2(Vector4 vectorToRead, string components)
    {
        return ReadComponents(vectorToRead, components, vectorToRead, 2);
    }

    public static Vector3 Vec3(Vector4 vectorToRead, string components,
        Vector4 vectorToChange)
    {
        return ReadComponents(vectorToRead, components, vectorToChange, 3);
    }
    public static Vector3 Vec3(Vector4 vectorToRead, string components)
    {
        return ReadComponents(vectorToRead, components, vectorToRead, 3);
    }

    public static Vector4 Vec4(Vector4 vectorToRead, string components,
        Vector4 vectorToChange)
    {
        return ReadComponents(vectorToRead, components, vectorToChange, 4);
    }
    public static Vector4 Vec4(Vector4 vectorToRead, string components)
    {
        return ReadComponents(vectorToRead, components, vectorToRead, 4);
    }
}