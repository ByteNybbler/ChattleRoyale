// Author(s): Paul Calande
// Data about an orthographic camera that can be manipulated.
// This data represents an orthographic camera view.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDataOrtho2D : IDeepCopyable<CameraDataOrtho2D>
{
    // The position of the camera in space.
    // Does not account for the z-coordinate.
    Vector2 position;
    // The orthographic size is half the camera's height in world units.
    float orthographicSize;
    // The aspect ratio (width divided by height).
    float aspect;

    // Constructor.
    CameraDataOrtho2D(Vector2 position, float orthographicSize, float aspect)
    {
        this.position = position;
        this.orthographicSize = orthographicSize;
        this.aspect = aspect;
    }

    // Deep copy method.
    public CameraDataOrtho2D DeepCopy()
    {
        return new CameraDataOrtho2D(position, orthographicSize, aspect);
    }

    // Construct the camera data based on the state of an existing camera.
    // This is the recommended way to construct this class.
    public CameraDataOrtho2D(Camera cam)
    {
        position = cam.transform.position;
        orthographicSize = cam.orthographicSize;
        aspect = cam.aspect;
    }
    /*
    public static CameraDataOrtho2D FromCamera(Camera cam)
    {
        return new CameraDataOrtho2D(cam.transform.position, cam.orthographicSize, cam.aspect);
    }
    */

    // Construct the camera data based on a rectangle.
    public CameraDataOrtho2D(Rect rect)
    {
        FitRect(rect);
    }

    // Assign this data to an orthographic camera.
    public void AssignTo(Camera cam)
    {
        // Move the camera to the new 2D position without modifying the camera z position.
        cam.transform.position = Swizzle.Vec3(position, "xy_", cam.transform.position);
        cam.orthographicSize = orthographicSize;
        cam.aspect = aspect;
    }

    // Returns the camera position in space.
    public Vector2 GetPosition()
    {
        return position;
    }

    // Returns half the height of the camera view in world units.
    public float GetOrthographicSize()
    {
        return orthographicSize;
    }

    // Returns the aspect ratio of the camera view.
    public float GetAspect()
    {
        return aspect;
    }

    // Returns the width of the camera view in world units.
    public float GetWidth()
    {
        return GetHeight() * aspect;
    }

    // Returns the height of the camera view in world units.
    public float GetHeight()
    {
        return 2 * orthographicSize;
    }

    public float GetHalfWidth()
    {
        return GetWidth() * 0.5f;
    }

    public float GetHalfHeight()
    {
        return orthographicSize;
    }

    // Returns the x coordinate of the right edge.
    public float GetCoordinateRightEdge()
    {
        return position.x + GetHalfWidth();
    }

    // Returns the x coordinate of the left edge.
    public float GetCoordinateLeftEdge()
    {
        return position.x - GetHalfWidth();
    }

    // Returns the y coordinate of the top edge.
    public float GetCoordinateTopEdge()
    {
        return position.y + GetHalfHeight();
    }

    // Returns the y coordinate of the bottom edge.
    public float GetCoordinateBottomEdge()
    {
        return position.y - GetHalfHeight();
    }

    // Adjust the orthographic size to fit the given width, in world units.
    public void FitWidth(float width)
    {
        orthographicSize = width / (2 * aspect);
    }

    // Adjust the orthographic size to fit the given height, in world units.
    public void FitHeight(float height)
    {
        orthographicSize = height * 0.5f;
    }

    // Adjust the orthographic size to fit both the given width and height, in world units.
    public void FitWidthAndHeight(float width, float height)
    {
        if (width > height)
        {
            FitWidth(width);
        }
        else
        {
            FitHeight(height);
        }
    }

    // Adjust the camera data to fit the given rectangle.
    public void FitRect(Rect rect)
    {
        position = rect.center;
        FitWidthAndHeight(rect.width, rect.height);
    }

    // Adjust the camera data to fit the given bounds.
    public void FitBounds(Bounds bounds)
    {
        position = bounds.center;
        FitWidthAndHeight(bounds.max.x - bounds.min.x, bounds.max.y - bounds.min.y);
    }

    public void SetPosition(Vector2 position)
    {
        this.position = position;
    }

    public void OffsetPosition(Vector2 offset)
    {
        position += offset;
    }

    // Interpolate.
    public static CameraDataOrtho2D Lerp(CameraDataOrtho2D a, CameraDataOrtho2D b, float t)
    {
        return new CameraDataOrtho2D(
            Vector3.Lerp(a.position, b.position, t),
            Mathf.Lerp(a.orthographicSize, b.orthographicSize, t),
            Mathf.Lerp(a.aspect, b.aspect, t));
    }
    public static CameraDataOrtho2D SmoothStep(CameraDataOrtho2D a, CameraDataOrtho2D b, float t)
    {
        return Lerp(a, b, Mathf.SmoothStep(0.0f, 1.0f, t));
    }

    // Positions and resizes the camera so that the camera is placed between
    // two given points and has edges that contain those points.
    public void AnchorBetween(Vector2 first, Vector2 second)
    {
        position = Vector2.Lerp(first, second, 0.5f);
        float differenceX = second.x - first.x;
        float differenceY = second.y - first.y;
        int signX = UtilMath.SignWithZero(differenceX);
        int signY = UtilMath.SignWithZero(differenceY);
        float targetWidth = Mathf.Abs(differenceX);
        float targetHeight = Mathf.Abs(differenceY);
        if (signX == 0)
        {
            FitHeight(targetHeight);
        }
        else if (signY == 0)
        {
            FitWidth(targetWidth);
        }
        else
        {
            if (targetWidth / targetHeight < aspect)
            {
                FitHeight(targetHeight);
            }
            else
            {
                FitWidth(targetWidth);
            }
        }
    }

    // Positions and resizes the camera so that a given edge of the camera matches
    // the line formed by the given vertices. Each vertex corresponds to one corner
    // of the camera. The vertices should be defined in a clockwise order.
    // If the given edge is diagonal, nothing happens.
    // (TODO: Perhaps diagonal edges should rotate the camera to match the edge?)
    public void AnchorToClockwiseEdge(Vector2 vertexFirst, Vector2 vertexSecond)
    {
        float differenceX = vertexSecond.x - vertexFirst.x;
        float differenceY = vertexSecond.y - vertexFirst.y;
        int signX = UtilMath.SignWithZero(differenceX);
        int signY = UtilMath.SignWithZero(differenceY);
        if (signX == 0)
        {
            FitHeight(Mathf.Abs(differenceY));
            position = new Vector2(vertexFirst.x + GetHalfWidth() * signY,
                vertexFirst.y + GetHalfHeight() * signY);
        }
        else if (signY == 0)
        {
            FitWidth(Mathf.Abs(differenceX));
            position = new Vector2(vertexFirst.x + GetHalfWidth() * signX,
                vertexFirst.y - GetHalfHeight() * signX);
        }
        else
        {
            // The edge is diagonal.
            Debug.LogWarning("CameraDataOrtho2D: Attempted to anchor to diagonal edge.");
        }
    }
}