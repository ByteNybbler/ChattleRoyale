// Author(s): Paul Calande
// A useful enum for defining the directions represented by a coordinate pair.

public enum CoordinateOrder2D
{
    RightThenDown,
    RightThenUp,
    LeftThenDown,
    LeftThenUp,
    UpThenRight,
    UpThenLeft,
    DownThenRight,
    DownThenLeft
}

static class CoordinateOrder2DExtensions
{
    public static Direction2D GetDirectionFirst(this CoordinateOrder2D e)
    {
        switch (e)
        {
            default:
            case CoordinateOrder2D.RightThenDown:
            case CoordinateOrder2D.RightThenUp:
                return Direction2D.Right;
            case CoordinateOrder2D.LeftThenDown:
            case CoordinateOrder2D.LeftThenUp:
                return Direction2D.Left;
            case CoordinateOrder2D.UpThenRight:
            case CoordinateOrder2D.UpThenLeft:
                return Direction2D.Up;
            case CoordinateOrder2D.DownThenRight:
            case CoordinateOrder2D.DownThenLeft:
                return Direction2D.Down;
        }
    }

    public static Direction2D GetDirectionSecond(this CoordinateOrder2D e)
    {
        switch (e)
        {
            default:
            case CoordinateOrder2D.UpThenRight:
            case CoordinateOrder2D.DownThenRight:
                return Direction2D.Right;
            case CoordinateOrder2D.UpThenLeft:
            case CoordinateOrder2D.DownThenLeft:
                return Direction2D.Left;
            case CoordinateOrder2D.RightThenUp:
            case CoordinateOrder2D.LeftThenUp:
                return Direction2D.Up;
            case CoordinateOrder2D.RightThenDown:
            case CoordinateOrder2D.LeftThenDown:
                return Direction2D.Down;
        }
    }

    public static Direction2D GetDirectionHorizontal(this CoordinateOrder2D e)
    {
        switch (e)
        {
            default:
            case CoordinateOrder2D.UpThenRight:
            case CoordinateOrder2D.DownThenRight:
            case CoordinateOrder2D.RightThenUp:
            case CoordinateOrder2D.RightThenDown:
                return Direction2D.Right;
            case CoordinateOrder2D.UpThenLeft:
            case CoordinateOrder2D.DownThenLeft:
            case CoordinateOrder2D.LeftThenUp:
            case CoordinateOrder2D.LeftThenDown:
                return Direction2D.Left;
        }
    }

    public static Direction2D GetDirectionVertical(this CoordinateOrder2D e)
    {
        switch (e)
        {
            default:
            case CoordinateOrder2D.UpThenRight:
            case CoordinateOrder2D.UpThenLeft:
            case CoordinateOrder2D.RightThenUp:
            case CoordinateOrder2D.LeftThenUp:
                return Direction2D.Up;
            case CoordinateOrder2D.DownThenRight:
            case CoordinateOrder2D.DownThenLeft:
            case CoordinateOrder2D.RightThenDown:
            case CoordinateOrder2D.LeftThenDown:
                return Direction2D.Down;
        }
    }

    public static CoordinateOrder2D Rotate180(this CoordinateOrder2D e)
    {
        switch (e)
        {
            default:
            case CoordinateOrder2D.RightThenDown:
                return CoordinateOrder2D.LeftThenUp;
            case CoordinateOrder2D.RightThenUp:
                return CoordinateOrder2D.LeftThenDown;
            case CoordinateOrder2D.LeftThenDown:
                return CoordinateOrder2D.RightThenUp;
            case CoordinateOrder2D.LeftThenUp:
                return CoordinateOrder2D.RightThenDown;
            case CoordinateOrder2D.UpThenRight:
                return CoordinateOrder2D.DownThenLeft;
            case CoordinateOrder2D.UpThenLeft:
                return CoordinateOrder2D.DownThenRight;
            case CoordinateOrder2D.DownThenRight:
                return CoordinateOrder2D.UpThenLeft;
            case CoordinateOrder2D.DownThenLeft:
                return CoordinateOrder2D.UpThenRight;
        }
    }

    // TODO: FlipHorizontal, FlipVertical, Rotate90CW, etc.
}