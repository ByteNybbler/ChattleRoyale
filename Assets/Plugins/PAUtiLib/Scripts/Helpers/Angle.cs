// Author(s): Paul Calande
// Angle class with several different read-only repesentations,
// including degrees and radians.
// This class also includes various other angle-related utility functions.
// The internal angle representation is not clamped to the smallest possible coterminals.
// This is necessary because some rotations may be larger than 360 degrees.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Angle : IDeepCopyable<Angle>
{
    // The common constant, 2*pi, is the full circumference of the unit circle.
    public const float TWO_PI = Mathf.PI * 2;

    // Unsigned interval for angles, in degrees: [0, 360).
    public static readonly IntervalFloat INTERVAL_UNSIGNED_DEGREES
        = IntervalFloat.FromStartEnd(0.0f, 360.0f);

    // Signed interval for angles, in degrees: [-180, 180).
    public static readonly IntervalFloat INTERVAL_SIGNED_DEGREES
        = IntervalFloat.FromStartEnd(-180.0f, 180.0f);

    // Unsigned interval for angles, in radians: [0, 2*pi).
    public static readonly IntervalFloat INTERVAL_UNSIGNED_RADIANS
        = IntervalFloat.FromStartEnd(0.0f, TWO_PI);

    // Signed interval for angles, in radians: [-pi. pi).
    public static readonly IntervalFloat INTERVAL_SIGNED_RADIANS
        = IntervalFloat.FromStartEnd(-Mathf.PI, Mathf.PI);

    [SerializeField]
    [Tooltip("The degree measure of the angle. "
        + "The radian measure is also obtained using this value.")]
    float degrees;

    // Private constructors means that a factory method must be used
    // to construct an instance of this class.
    private Angle(float degrees)
    {
        this.degrees = degrees;
    }

    // Factory methods.
    // Constructs an angle from a degree measure.
    public static Angle FromDegrees(float degrees)
    {
        return new Angle(degrees);
    }
    // Constructs an angle from a radian measure.
    public static Angle FromRadians(float radians)
    {
        return new Angle(radians * Mathf.Rad2Deg);
    }
    // Returns a random angle from the given range.
    // If only one argument is specified, the start of the range will be zero.
    // If no arguments are specified, the 0-360 degree range will be used.
    public static Angle FromRandomDegrees(float start, float end)
    {
        return Angle.FromDegrees(Random.Range(start, end));
    }
    public static Angle FromRandomDegrees(float end = 360.0f)
    {
        return Angle.FromRandomDegrees(0.0f, end);
    }
    public static Angle FromRandomRadians(float start, float end)
    {
        return Angle.FromRadians(Random.Range(start, end));
    }
    public static Angle FromRandomRadians(float end = TWO_PI)
    {
        return Angle.FromRandomRadians(0.0f, end);
    }
    public static Angle FromRandom(Angle start, Angle end)
    {
        return Angle.FromRandomDegrees(start.GetDegrees(), end.GetDegrees());
    }
    public static Angle FromRandom(Angle end)
    {
        return Angle.FromRandomDegrees(0.0f, end.GetDegrees());
    }
    public static Angle FromRandom()
    {
        return Angle.FromRandomDegrees();
    }
    // Returns a random angle measure within the given interval.
    public static Angle FromRandomDegrees(IntervalFloat interval)
    {
        return Angle.FromDegrees(interval.GetRandom());
    }
    public static Angle FromRandomRadians(IntervalFloat interval)
    {
        return Angle.FromRadians(interval.GetRandom());
    }
    // xzx
    //const Angle ZORO = new Angle(0.0f);
    //const Angle ZERO = Angle.FromDegrees(0.0f);
    // In this case, the radius of the angle refers to the measure between the
    // angle's center and the angle's end. In other words, the radius is half
    // of the angle's total measure.
    public static Angle FromRandomRadius(Angle radius, Angle center)
    {
        return Angle.FromRandom(center - radius, center + radius);
    }
    /*
    public static Angle FromRandomRadius(Angle radius)
    {
        return Angle.FromRandomRadius(radius, )
    }
    */
    public static Angle FromRandomDiameter(Angle diameter, Angle center)
    {
        return Angle.FromRandomRadius(diameter * 0.5f, center);
    }
    public static Angle FromRandomRadiusDegrees(float radius, float center = 0.0f)
    {
        return Angle.FromRandomDegrees(-radius, radius).AddDegrees(center);
    }
    public static Angle FromRandomDiameterDegrees(float diameter, float center = 0.0f)
    {
        return Angle.FromRandomRadiusDegrees(diameter * 0.5f, center);
    }
    public static Angle FromRandomRadiusRadians(float radius, float center = 0.0f)
    {
        return Angle.FromRandomRadiusDegrees(radius * Mathf.Rad2Deg, center * Mathf.Rad2Deg);
    }
    public static Angle FromRandomDiameterRadians(float diameter, float center = 0.0f)
    {
        return Angle.FromRandomRadiusRadians(diameter * 0.5f, center);
    }
    // Constructs an angle from the given heading vector.
    public static Angle FromHeadingVector(Vector2 heading)
    {
        float degrees = Vector2.SignedAngle(Vector2.right, heading);
        //degrees = INTERVAL_UNSIGNED_DEGREES.Remainder(degrees);
        return Angle.FromDegrees(degrees);
    }
    public static Angle FromHeadingVector(float x, float y)
    {
        return Angle.FromHeadingVector(new Vector2(x, y));
    }
    /*
    // Returns the angle on the plane perpendicular to any 3D axis.
    public static Angle FromHeadingVectorPerpendicular(Vector2 heading)
    {
        return Angle.FromHeadingVector(heading).MirrorHorizontal().AddDegrees(-90.0f);
    }
    // Returns the angle on the plane perpendicular to the Y axis.
    public static Angle FromHeadingVectorY(Vector3 heading)
    {
        return Angle.FromHeadingVectorPerpendicular(Swizzle.Vec2(heading, "xz"));
    }
    */
    // Returns a wheel's angular velocity based on its linear velocity and radius.
    public static Angle FromAngularVelocity(float linearVelocity, float radius)
    {
        return Angle.FromRadians(linearVelocity / radius);
    }
    // Returns the angle between two quaternions.
    public static Angle FromQuaternions(Quaternion start, Quaternion end)
    {
        return Angle.FromDegrees(Quaternion.Angle(start, end));
    }
    // Returns the angle between the given quaternion and the quaternion identity.
    public static Angle FromQuaternion(Quaternion quat)
    {
        return Angle.FromQuaternions(Quaternion.identity, quat);
    }
    // Returns the signed angle that faces the end point from the start point.
    public static Angle FromPoint(Vector2 startPoint, Vector2 endPoint)
    {
        return Angle.FromHeadingVector(endPoint - startPoint);
    }
    /*
    // Returns the angle to a moving point, given the point's velocity and the
    // theoretical projectile's velocity.
    public static Angle FromMovingPoint(Vector2 startPosition,
        Vector2 targetPosition, Vector2 targetVelocity, Vector2 projectileVelocity)
    {

    }
    */

    // Creates a copy of the given angle.
    public Angle DeepCopy()
    {
        return new Angle(degrees);
    }
    public static Angle FromDeepCopy(Angle otherAngle)
    {
        return otherAngle.DeepCopy();
    }

    // Returns a measurement of the angle.
    // This is not the measurement of the smallest possible coterminal.
    public float GetDegrees()
    {
        return degrees;
    }
    public float GetRadians()
    {
        return degrees * Mathf.Deg2Rad;
    }

    // Returns the unsigned coterminal measure of the angle.
    public float GetDegreesUnsigned()
    {
        return INTERVAL_UNSIGNED_DEGREES.Remainder(degrees);
    }
    public float GetRadiansUnsigned()
    {
        return INTERVAL_UNSIGNED_RADIANS.Remainder(GetRadians());
    }

    // Returns the signed coterminal measure of the angle.
    public float GetDegreesSigned()
    {
        return INTERVAL_SIGNED_DEGREES.Remainder(degrees);
    }
    public float GetRadiansSigned()
    {
        return INTERVAL_SIGNED_RADIANS.Remainder(GetRadians());
    }

    // Converts an angle to the coterminal angle within the [-180, 180) range.
    public Angle ToCoterminalSigned()
    {
        degrees = GetDegreesSigned();
        return this;
    }
    public Angle GetCoterminalSigned()
    {
        return DeepCopy().ToCoterminalSigned();
    }

    // Converts an angle to the coterminal angle within the [0, 360) range.
    public Angle ToCoterminalUnsigned()
    {
        degrees = GetDegreesUnsigned();
        return this;
    }
    public Angle GetCoterminalUnsigned()
    {
        return DeepCopy().ToCoterminalUnsigned();
    }

    // Converts an angle to an equivalent angle within the range with the given center.
    // The range will always be 360 degrees in size.
    public Angle ToCoterminal(Angle centerOfTheInterval)
    {
        IntervalFloat interval = IntervalFloat.FromRadius(
            180.0f, centerOfTheInterval.GetDegrees());
        degrees = interval.Remainder(degrees);
        return this;
    }
    public Angle GetCoterminal(Angle centerOfTheInterval)
    {
        return DeepCopy().ToCoterminal(centerOfTheInterval);
    }

    // Returns a unit vector pointing in the direction given by the angle.
    public Vector2 GetHeadingVector()
    {
        float radians = GetRadians();
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }

    // Returns a wheel's linear velocity based on its angular velocity and radius.
    // The calling angle is assumed to be the angular velocity.
    public float GetLinearVelocity(float radius)
    {
        return GetRadians() * radius;
    }
    
    // Returns a random angle within this angle.
    public Angle GetRandom(Angle center)
    {
        return Angle.FromRandomDiameter(this, center);
    }

    // Returns the z-axis quaternion equivalent to rotating by this angle.
    public Quaternion GetQuaternionZ(bool counterclockwise = true)
    {
        return Quaternion.Euler(0.0f, 0.0f, degrees * UtilMath.Sign(counterclockwise));
    }

    // Returns the given vector once it's rotated by this angle.
    public Vector2 RotateVector(Vector2 vector, bool counterclockwise = true)
    {
        return GetQuaternionZ(counterclockwise) * vector;
    }

    // Adds this many degrees to the angle.
    public Angle AddDegrees(float degreesAdditional)
    {
        degrees += degreesAdditional;
        return this;
    }

    // Adds this many radians to the angle.
    public Angle AddRadians(float radiansAdditional)
    {
        degrees += radiansAdditional * Mathf.Rad2Deg;
        return this;
    }

    // Adds a different angle to this angle.
    public Angle AddAngle(Angle other)
    {
        degrees += other.GetDegrees();
        return this;
    }

    // Rotates the angle by 180 degrees, effectively reversing its direction.
    public Angle Reverse()
    {
        degrees = INTERVAL_UNSIGNED_DEGREES.Reverse(degrees);
        return this;
    }
    public Angle GetReverse()
    {
        return DeepCopy().Reverse();
    }

    // Mirrors an angle across the y-axis of a circle.
    public Angle MirrorHorizontal()
    {
        degrees = INTERVAL_UNSIGNED_DEGREES.MirrorHorizontal(degrees);
        return this;
    }
    public Angle GetMirrorHorizontal()
    {
        return DeepCopy().MirrorHorizontal();
    }

    // Mirrors an angle across the x-axis of a circle.
    public Angle MirrorVertical()
    {
        degrees = INTERVAL_UNSIGNED_DEGREES.MirrorVertical(degrees);
        return this;
    }
    public Angle GetMirrorVertical()
    {
        return DeepCopy().MirrorVertical();
    }

    /*
    // Returns this angle's reference angle.
    // This is the acute angle between the x-axis and the given angle.
    public Angle GetReference()
    {
        return new Angle(0.0f);
    }
    */

    // Returns the smaller distance between the two angles' coterminals.
    public static Angle GetSmallerDistance(Angle angle1, Angle angle2)
    {
        return Angle.FromDegrees(INTERVAL_UNSIGNED_DEGREES.GetSmallerDistance(
            angle1.GetDegrees(), angle2.GetDegrees()));
    }

    // Returns the larger distance between the two angles' coterminals.
    public static Angle GetLargerDistance(Angle angle1, Angle angle2)
    {
        return Angle.FromDegrees(INTERVAL_UNSIGNED_DEGREES.GetLargerDistance(
            angle1.GetDegrees(), angle2.GetDegrees()));
    }

    // Returns true if the shortest path between the two given angles' coterminals is a
    // positive (counterclockwise on the unit circle) rotation from the start angle.
    public static bool IsShortestRotationPositive(Angle start, Angle end)
    {
        return INTERVAL_UNSIGNED_DEGREES.IsShortestRotationPositive(
            start.GetDegrees(), end.GetDegrees());
    }

    // Returns true if both angles are coterminal.
    // Coterminal angles are angles that share a common terminal side.
    // 30 degrees, 390 degrees, and -330 degrees are all coterminal to each other.
    // Angles are also coterminal with themselves.
    // This method can be used as a broad check for directional equality between angles.
    public bool IsCoterminal(Angle other)
    {
        return GetDegreesUnsigned() == other.GetDegreesUnsigned();
    }
    public static bool IsCoterminal(Angle first, Angle second)
    {
        return first.IsCoterminal(second);
    }

    // Like the approach float function, but rotates current along the shortest path
    // to the target, like an angle moving along a circle towards a different angle.
    // This method utilizes both angles' coterminals, so having a rotation
    // greater than 360 degrees is not feasible without changing the target.
    public Angle ApproachCoterminal(Angle target, Angle stepSize,
        bool useShorterPath = true)
    {
        degrees = INTERVAL_UNSIGNED_DEGREES.Approach(degrees, target.GetDegrees(),
            stepSize.GetDegrees(), useShorterPath);
        return this;
    }

    // Move one angle towards another without considering coterminality.
    public Angle ApproachRaw(Angle target, Angle stepSize)
    {
        degrees = UtilApproach.Float(degrees, target.degrees, stepSize.degrees);
        return this;
    }

    // Returns the sign of the shortest rotation between the two angles' coterminals.
    public static int SignShortestRotation(Angle start, Angle end)
    {
        return INTERVAL_UNSIGNED_DEGREES.SignShortestRotation(
            start.GetDegrees(), end.GetDegrees());
    }

    // The multiplication operator scales the angle.
    public static Angle operator *(Angle first, float second)
    {
        return Angle.FromDegrees(first.degrees * second);
    }
    public static Angle operator /(Angle first, float second)
    {
        return Angle.FromDegrees(first.degrees / second);
    }
    // Add two angles together, combining their measures.
    public static Angle operator +(Angle first, Angle second)
    {
        return Angle.FromDegrees(first.degrees + second.degrees);
    }
    public static Angle operator -(Angle first, Angle second)
    {
        return Angle.FromDegrees(first.degrees - second.degrees);
    }

    // Angle measure comparisons.
    public static bool operator >=(Angle first, Angle second)
    {
        return first.GetDegrees() >= second.GetDegrees();
    }
    public static bool operator <=(Angle first, Angle second)
    {
        return first.GetDegrees() <= second.GetDegrees();
    }
    public static bool operator >(Angle first, Angle second)
    {
        return first.GetDegrees() > second.GetDegrees();
    }
    public static bool operator <(Angle first, Angle second)
    {
        return first.GetDegrees() < second.GetDegrees();
    }
}