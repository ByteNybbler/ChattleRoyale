// Author(s): Paul Calande
// Applies a gravitational acceleration to a given target.
// If the target is a dynamic rigidbody, its gravity scale field should be zero.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity2D
{
    public bool enabled = true;

    //[SerializeField]
    [Tooltip("Gravity will be applied in the direction opposite the upwards direction.")]
    VelocityInUpSpace2D vius;
    //[SerializeField]
    [Tooltip("How much acceleration is applied via the gravity.")]
    float acceleration = 39.2f;

    public Gravity2D(VelocityInUpSpace2D vius, float acceleration = 39.2f)
    {
        this.vius = vius;
        this.acceleration = acceleration;
    }

    public void Tick(float deltaTime)
    {
        if (!enabled)
        {
            return;
        }
        Vector2 velocity = vius.GetVelocity();
        velocity.y -= acceleration * deltaTime;
        vius.SetVelocity(velocity);
    }

    public void SetAcceleration(float value)
    {
        acceleration = value;
    }

    public float GetAcceleration()
    {
        return acceleration;
    }
}