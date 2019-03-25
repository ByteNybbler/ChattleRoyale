// Author(s): Paul Calande
// Accesses the color of any number of supported components.

using UnityEngine;

public class ColorAccessor : SingleAccessor<Color>
{
    public void SetAlpha(float alpha)
    {
        Color col = Get();
        col.a = alpha;
        Set(col);
    }

    public float GetAlpha()
    {
        return Get().a;
    }
}