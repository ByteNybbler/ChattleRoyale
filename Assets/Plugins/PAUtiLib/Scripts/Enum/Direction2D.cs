// Author(s): Paul Calande
// A useful enum for defining a direction on a 2D plane.

public enum Direction2D
{
    Up,
    Down,
    Left,
    Right,
    North = Up,
    South = Down,
    East = Right,
    West = Left
}

static class Direction2DExtensions
{
    public static bool IsHorizontal(this Direction2D e)
    {
        switch (e)
        {
            case Direction2D.Left:
            case Direction2D.Right:
                return true;
            default:
                return false;
        }
    }

    public static bool IsVertical(this Direction2D e)
    {
        switch (e)
        {
            case Direction2D.Up:
            case Direction2D.Down:
                return true;
            default:
                return false;
        }
    }

    // Returns the direction resulting from the given degree rotation.
    public static Direction2D Rotate180(this Direction2D e)
    {
        switch (e)
        {
            default:
            case Direction2D.Up:
                return Direction2D.Down;
            case Direction2D.Down:
                return Direction2D.Up;
            case Direction2D.Left:
                return Direction2D.Right;
            case Direction2D.Right:
                return Direction2D.Left;
        }
    }
    // TODO: Rotate90CW (Clockwise)
}