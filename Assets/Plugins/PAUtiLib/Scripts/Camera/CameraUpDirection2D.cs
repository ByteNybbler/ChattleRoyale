// Author(s): Paul Calande
// Makes a camera rotate to accommodate a given up direction.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUpDirection2D : MonoBehaviour
{
    [SerializeField]
    TimeScale timeScale;
    [SerializeField]
    [Tooltip("The up direction to rotate to.")]
    UpDirection2D upDirection;
    [SerializeField]
    [Tooltip("The rotation speed.")]
    Angle rotationSpeed = Angle.FromDegrees(360.0f);

    private void FixedUpdate()
    {
        Vector3 euler = transform.eulerAngles;
        euler.z = Angle.FromDegrees(euler.z).ApproachCoterminal(
            upDirection.GetUpAngle().AddDegrees(-90.0f),
            rotationSpeed * timeScale.DeltaTime())
            .GetDegrees();
        transform.rotation = Quaternion.Euler(euler);
    }
}