// Author(s): Paul Calande
// Anchors a camera to a sprite from a SpriteRenderer.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrthoAnchorToSprite2D : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The camera to modify.")]
    Camera cam;
    [SerializeField]
    [Tooltip("The SpriteRenderer to anchor to.")]
    SpriteRenderer rend;

    private void Start()
    {
        CameraDataOrtho2D cdo = new CameraDataOrtho2D(cam);
        Bounds bounds = rend.sprite.bounds;

        //bounds.size *= rend.transform.localScale;
        Vector3 result = bounds.size;
        result.x *= rend.transform.localScale.x;
        result.y *= rend.transform.localScale.y;
        result.z *= rend.transform.localScale.z;
        bounds.size = result;

        cdo.FitBounds(bounds);
        cdo.AssignTo(cam);
    }
}