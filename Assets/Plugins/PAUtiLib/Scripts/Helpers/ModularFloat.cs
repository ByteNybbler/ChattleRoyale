// Author(s): Paul Calande
// A float value that is constrained to a given interval.
// When the float passes an endpoint of the interval,
// it wraps around to the other side.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModularFloat
{
    [SerializeField]
    [Tooltip("The value of the actual float.")]
    float value;
    [SerializeField]
    [Tooltip("The interval that the float exists on.")]
    IntervalFloat interval;

    // Constructor.
    public ModularFloat(IntervalFloat interval, float initialValue)
    {
        value = initialValue;
        this.interval = interval;
    }

    // Constrains the value to the interval.
    void ConstrainValue()
    {
        value = interval.Remainder(value);
    }

    // Assigns a new value to the float.
    public void SetValue(float newValue)
    {
        value = newValue;
        ConstrainValue();
    }

    // Change the interval that the float exists on.
    public void SetInterval(IntervalFloat newInterval)
    {
        interval = newInterval;
        ConstrainValue();
    }

    public float GetReverse()
    {
        return interval.Reverse(value);
    }
    public void Reverse()
    {
        value = GetReverse();
    }

    public float GetMirrorHorizontal()
    {
        return interval.MirrorHorizontal(value);
    }
    public void MirrorHorizontal()
    {
        value = GetMirrorHorizontal();
    }

    public float GetMirrorVertical()
    {
        return interval.MirrorVertical(value);
    }
    public void MirrorVertical()
    {
        value = GetMirrorVertical();
    }

    public static implicit operator float(ModularFloat mf)
    {
        return mf.value;
    }

    public static ModularFloat operator +(ModularFloat first, float second)
    {
        first.SetValue(first.value + second);
        return first;
    }

    public static ModularFloat operator -(ModularFloat first, float second)
    {
        first.SetValue(first.value - second);
        return first;
    }

    public static ModularFloat operator *(ModularFloat first, float second)
    {
        first.SetValue(first.value * second);
        return first;
    }

    public static ModularFloat operator /(ModularFloat first, float second)
    {
        first.SetValue(first.value / second);
        return first;
    }
}