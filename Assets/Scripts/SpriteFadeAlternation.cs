// Author(s): Paul Calande
// Fades between two sprites in a sinusoidal fashion.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFadeAlternation : MonoBehaviour
{
    [SerializeField]
    float oscillationSpeed;
    [SerializeField]
    SpriteRenderer sprite1;
    [SerializeField]
    SpriteRenderer sprite2;

    Oscillator oscillator;

    private void Awake()
    {
        oscillator = new Oscillator(0.5f, oscillationSpeed, Mathf.Sin);
    }

    private void Update()
    {
        float alpha = 0.5f + oscillator.SampleAmplitude(Time.deltaTime);
        Color col = Color.white;
        col.a = alpha;
        sprite1.color = col;
        //col.a = 1.0f - alpha;
        //sprite2.color = col;
    }
}