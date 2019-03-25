// Author(s): Paul Calande
// Sends a ColorAccessor's alpha channel to a FloatAccessor.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAccessorAlphaToFloatAccessor : SingleAccessorConnection
    <Color, ColorAccessor, FloatAccessor>
{
    protected override void Set(Color color)
    {
        connected.Set(color.a);
    }

    protected override Color Get()
    {
        Color result = accessor.Get();
        result.a = connected.Get();
        return result;
    }
}