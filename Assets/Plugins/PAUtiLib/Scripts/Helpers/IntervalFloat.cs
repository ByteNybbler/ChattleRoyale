// Author(s): Paul Calande
// An interval that is terminated at float values.
// These intervals are considered continuous and can also be utilized periodically.
// That is, when a number passes through one end of the interval,
// it "wraps around" to the other side.
// One example of a variable that exists within a periodic interval is an angle measure.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntervalFloat
{
    [SerializeField]
    [Tooltip("The start of the interval.")]
    private float start;
    [SerializeField]
    [Tooltip("The end of the interval.")]
    private float end;

    // Private constructors mean that this class can only be constructed
    // from the factory methods.
    private IntervalFloat(float start, float end)
    {
        this.start = start;
        this.end = end;
    }

    // Factory methods.
    public static IntervalFloat FromStartEnd(float start, float end)
    {
        return new IntervalFloat(start, end);
    }
    public static IntervalFloat FromRadius(float radius, float center = 0.0f)
    {
        return new IntervalFloat(center - radius, center + radius);
    }
    // This method can also be used to define an angle centered at a direction.
    public static IntervalFloat FromDiameter(float diameter, float center = 0.0f)
    {
        return FromRadius(diameter * 0.5f, center);
    }

    // Returns the start of the interval.
    public float GetStart()
    {
        return start;
    }
    // Returns the end of the interval.
    public float GetEnd()
    {
        return end;
    }
    // Returns the diameter of the interval.
    public float GetDiameter()
    {
        return end - start;
    }
    // Returns the radius of the interval.
    public float GetRadius()
    {
        return GetDiameter() * 0.5f;
    }
    // Returns the Lerp between the interval's start and end by t.
    public float Lerp(float t)
    {
        return Mathf.Lerp(start, end, t);
    }
    // Returns the center of the interval.
    public float GetCenter()
    {
        return (start + end) * 0.5f;
    }
    // Sets the center of the interval while preserving the diameter.
    public void SetCenter(float center)
    {
        float radius = GetRadius();
        start = center - radius;
        end = center + radius;
    }
    // Returns a random float from the interval.
    public float GetRandom()
    {
        return Lerp(Random.Range(0.0f, 1.0f));
    }

    // Returns an array of floats that are evenly spaced out from each other
    // and cover this interval. The distance between each float is based on
    // the value of the "count" parameter, which determines the number of floats.
    public float[] PopulateLinear(int count)
    {
        List<float> result = new List<float>();
        float difference = end - start;
        if (count == 1)
        {
            result.Add(GetCenter());
        }
        else
        {
            float stepSize = difference / (count - 1);
            for (int i = 0; i < count; ++i)
            {
                result.Add(start + stepSize * i);
            }
        }
        return result.ToArray();
    }

    // Moves the given value into the range, preserving the value's position in the period.
    // This essentially retrieves the remainder of the given value modulo the diameter.
    // In other words, this method returns the principal value that is congruent to the
    // given value.
    public float Remainder(float value)
    {
        float diameter = GetDiameter();
        while (value >= end)
        {
            value -= diameter;
        }
        while (value < start)
        {
            value += diameter;
        }
        return value;
    }

    // Moves a value to the opposite end of a circle.
    // This is effectively the same as rotating by 180 degrees.
    public float Reverse(float value)
    {
        return Remainder(value + GetRadius());
    }

    // Mirrors a value across the y-axis of a circle.
    public float MirrorHorizontal(float value)
    {
        return Remainder(GetCenter() - value);
    }

    // Mirrors a value across the x-axis of a circle.
    public float MirrorVertical(float value)
    {
        return Remainder(-value);
    }

    // Returns one of the two distances between the two values on the interval.
    // This accounts for the distance traveled via wrapping across the ends of the interval.
    // There's no guarantee whether this will be the smaller distance or the larger distance.
    private float GetSomeDistance(float value1, float value2)
    {
        return Remainder(Mathf.Abs(value1 - value2));
    }

    // Returns the smaller distance between the two values.
    public float GetSmallerDistance(float value1, float value2)
    {
        float distance = GetSomeDistance(value1, value2);
        if (distance > GetCenter())
        {
            return GetDiameter() - distance;
        }
        else
        {
            return distance;
        }
    }

    // Returns the larger distance between the two values.
    public float GetLargerDistance(float value1, float value2)
    {
        float distance = GetSomeDistance(value1, value2);
        if (distance <= GetCenter())
        {
            return GetDiameter() - distance;
        }
        else
        {
            return distance;
        }
    }

    // Returns true if the shortest path between the two given values can be
    // traversed by a positive increase from the start value.
    public bool IsShortestRotationPositive(float startPoint, float endPoint)
    {
        // Normalize the start and end values to be within the interval.
        startPoint = Remainder(startPoint);
        endPoint = Remainder(endPoint);

        // Whether the shortest rotation involves stepping (wrapping)
        // across the ends of the interval.
        // True means it doesn't need to wrap. False means it does.
        bool wrapless = Mathf.Abs(startPoint - endPoint) < GetCenter();

        if (startPoint < endPoint)
        {
            return wrapless;
        }
        else
        {
            return !wrapless;
        }
    }

    // Returns the sign of the shortest rotation between the two values.
    public int SignShortestRotation(float startPoint, float endPoint)
    {
        return UtilMath.Sign(IsShortestRotationPositive(startPoint, endPoint));
    }

    // Like the approach float function, but rotates current along the shortest path
    // to the target, like an angle moving along a circle towards a different angle.
    public float Approach(float current, float target, float stepSize,
        bool useShorterPath = true)
    {
        current = Remainder(current);
        target = Remainder(target);
        if (GetSmallerDistance(current, target) < stepSize)
        {
            return target;
        }
        current += stepSize * SignShortestRotation(current, target)
            * UtilMath.Sign(useShorterPath);
        return current;
    }
}