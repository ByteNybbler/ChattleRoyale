// Author(s): Paul Calande
// Shakes the GameObject.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake2D : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How big the shake is.")]
    float shakeScale;

    // The vector by which to move the GameObject.
    Vector3 shake = Vector3.zero;
    // The timer that tracks how long to shake the GameObject.
    Timer timerShake;

    private void Start()
    {
        timerShake = new Timer(1.0f);
    }

    private void FixedUpdate()
    {
        Shake(shakeScale);
    }

    private void Shake(float shakeScale)
    {
        transform.position -= shake;
        shake = UtilRandom.OnUnitCircle() * shakeScale;
        transform.position += shake;
    }

    // Start shaking.
    public void Run()
    {

    }
}