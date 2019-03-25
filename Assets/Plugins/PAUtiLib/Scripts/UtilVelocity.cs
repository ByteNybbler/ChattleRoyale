// Author(s): Paul Calande
// Utility functions for making velocity-based calculations.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilVelocity
{
    // Gets the predicted position of an object with a given start position and velocity after a
    // given number of seconds. Does not take potential collisions or external forces into account.
    public static T GetFuturePosition<T>(T startPos, T velocity, float seconds)
    {
        T distanceTraveled = UtilGeneric.Multiply<T, float, T>(velocity, seconds);
        return UtilGeneric.Add<T, T, T>(startPos, distanceTraveled);
    }

    // Returns the velocity at which an object will have to move to get from the start position to
    // the end position in the given number of seconds. This assumes the object will move in a
    // straight line with a constant velocity.
    public static T GetVelocity<T>(T startPos, T endPos, float seconds)
    {
        T difference = UtilGeneric.Subtract<T, T, T>(endPos, startPos);
        return UtilGeneric.Divide<T, float, T>(difference, seconds);
    }
}