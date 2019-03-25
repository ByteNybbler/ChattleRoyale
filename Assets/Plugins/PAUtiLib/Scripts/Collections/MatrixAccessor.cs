// Author(s): Paul Calande
// For a more user-friendly interface with a matrix stored in row-major order.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatrixAccessor
{
    // The current width of the array.
    int width;
    // The current height of the array.
    int height;
    // The coordinate order for accessing the array.
    CoordinateOrder2D order;
    // Whether the accessor-based coordinates should be zero-indexed.
    bool zeroIndexed;

    public MatrixAccessor(int width, int height, CoordinateOrder2D order, bool zeroIndexed = true)
    {
        this.width = width;
        this.height = height;
        this.order = order;
        this.zeroIndexed = zeroIndexed;
    }

    /*
    // Adjusts any non-zero-indexed coordinates to be zero-indexed.
    private void CompensateZeroIndexing(ref int coord1, ref int coord2)
    {
        if (!zeroIndexed)
        {
            coord1 -= 1;
            coord2 -= 1;
        }
    }
    */

    // Returns the offset from traversing this many rows in the array.
    private int GetOffsetVertical(int rows)
    {
        return width * rows;
    }

    // Returns the offset from traversing this many columns in the array.
    private int GetOffsetHorizontal(int columns)
    {
        return columns;
    }

    // Returns the zero-indexed array row that the given index exists on.
    private int GetArrayRowFromIndex(int index)
    {
        return index / width;
    }

    // Returns the zero-indexed array column that the given index exists on.
    private int GetArrayColumnFromIndex(int index)
    {
        return index % width;
    }

    // Returns the offset resulting from a given coordinate and its direction.
    private int GetOffsetFromCoordinate(int coordinate, Direction2D direction)
    {
        int offset;

        // Horizontal directions move across columns.
        // Vertical directions move across rows.
        if (direction.IsHorizontal())
        {
            offset = GetOffsetHorizontal(coordinate);
        }
        else
        {
            offset = GetOffsetVertical(coordinate);
        }

        // Invert offsets for coordinates that are moving back towards the start of the array.
        if (direction == Direction2D.Left || direction == Direction2D.Up)
        {
            offset *= -1;
        }

        return offset;
    }

    // Returns the total offset resulting from two given coordinates.
    // The directions used are defined by the coordinate order.
    private int GetOffsetFromCoordinates(int coord1, int coord2)
    {
        Direction2D dir1 = order.GetDirectionFirst();
        Direction2D dir2 = order.GetDirectionSecond();
        int coord1Offset = GetOffsetFromCoordinate(coord1, dir1);
        int coord2Offset = GetOffsetFromCoordinate(coord2, dir2);
        return coord1Offset + coord2Offset;
    }

    // Returns the array's start offset based on the given step direction.
    private int GetStartOffsetFromDirection(Direction2D direction)
    {
        switch (direction)
        {
            // If we're ever going to step to the left, we should start at the right side of the grid.
            case Direction2D.Left:
                return GetOffsetHorizontal(width - 1);

            // If we're ever going to step upwards, we should start at the bottom of the grid.
            case Direction2D.Up:
                return GetOffsetVertical(height - 1);

            default:
                return 0;
        }
    }

    private int GetEndOffsetFromDirection(Direction2D direction)
    {
        return GetStartOffsetFromDirection(direction.Rotate180());
    }

    // Returns the offset resulting from traversing the given number of bands.
    // The offset is affected by the given direction to travel in.
    // A band is formed by a single line of contiguous cells that crosses the entire array.
    // For instance, a row is a horizontal band and a column is a vertical band.
    // A horizontal band is formed by traversing the array horizontally.
    // A vertical band is formed by traversing the array vertically.
    private int GetBandOffsetFromDirection(Direction2D direction, int bands = 1)
    {
        int bandLength = GetEndOffsetFromDirection(direction)
            - GetStartOffsetFromDirection(direction);
        return bandLength * bands;
    }

    // Returns the starting offset for the given coordinate.
    private int GetStartOffsetCoord1()
    {
        return GetStartOffsetFromDirection(order.GetDirectionFirst());
    }
    private int GetStartOffsetCoord2()
    {
        return GetStartOffsetFromDirection(order.GetDirectionSecond());
    }
    // Returns the ending offset for the given coordinate.
    private int GetEndOffsetCoord1()
    {
        return GetEndOffsetFromDirection(order.GetDirectionFirst());
    }
    private int GetEndOffsetCoord2()
    {
        return GetEndOffsetFromDirection(order.GetDirectionSecond());
    }

    // Returns the array's start index based on the given coordinate order.
    // This is equivalent to getting the index at zero-based (0, 0).
    private int GetStartIndex(CoordinateOrder2D coordinateOrder)
    {
        int offset1 = GetStartOffsetFromDirection(coordinateOrder.GetDirectionFirst());
        int offset2 = GetStartOffsetFromDirection(coordinateOrder.GetDirectionSecond());
        return offset1 + offset2;
    }
    private int GetStartIndex()
    {
        return GetStartIndex(order);
    }

    // Returns the array's end index based on the given coordinate order.
    // This is equivalent to getting the index at zero-based (width-1, height-1).
    private int GetEndIndex(CoordinateOrder2D coordinateOrder)
    {
        return GetStartIndex(coordinateOrder.Rotate180());
    }
    private int GetEndIndex()
    {
        return GetEndIndex(order);
    }

    // Returns the index corresponding to the given coordinate pair.
    public int GetIndexAt(int coord1, int coord2)
    {
        //CompensateZeroIndexing(ref coord1, ref coord2);

        int startIndex = GetStartIndex();
        int coordOffset = GetOffsetFromCoordinates(coord1, coord2);

        return startIndex + coordOffset;
    }

    // Returns true if the given point is in the array.
    public bool IsWithinMatrix(int coord1, int coord2)
    {
        //CompensateZeroIndexing(ref coord1, ref coord2);
        return coord1 >= 0 && coord1 < width && coord2 >= 0 && coord2 < height;
    }

    // Swaps the width and height.
    // This is a valid operation because the array size remains the same.
    public void SwapWidthAndHeight()
    {
        int temp = width;
        width = height;
        height = temp;
    }

    // TODO: Specify the order of the iteration too, utilizing CoordinateOrder2D?
    public IEnumerable<int> SelectAll()
    {
        for (int i = 0; i < width * height; ++i)
        {
            yield return i;
        }
    }

    // Enumerate across the rectangle bounded by the given coordinates.
    // TODO: Actually make it loop properly.
    public IEnumerable<int> SelectRange(int start1, int start2, int end1, int end2)
    {
        int startIndex = GetIndexAt(start1, start2);
        int endIndex = GetIndexAt(end1, end2);

        int step = endIndex.CompareTo(startIndex);
        for (int i = startIndex; i != endIndex; i += step)
        {
            yield return i;
        }
    }

    // Enumerate across the given band defined by the first coordinate.
    public IEnumerable<int> SelectBandsUsingCoord1(int start, int end)
    {
        return SelectRange(start, GetStartOffsetCoord2(), end, GetEndOffsetCoord2());
    }
    public IEnumerable<int> SelectBandUsingCoord1(int coord1)
    {
        return SelectBandsUsingCoord1(coord1, coord1);
    }

    // Enumerate across the given band defined by the second coordinate.
    public IEnumerable<int> SelectBandsUsingCoord2(int start, int end)
    {
        return SelectRange(GetStartOffsetCoord1(), start, GetEndOffsetCoord1(), end);
    }
    public IEnumerable<int> SelectBandUsingCoord2(int coord2)
    {
        return SelectBandsUsingCoord2(coord2, coord2);
    }

    /*
    // Enumerate across the given band defined by the first coordinate.
    public IEnumerable<int> SelectBandFirst(int coord1)
    {
        return SelectRange(coord1, 0, coord1, width - 1);
    }
    */

    /*
    // Enumerate across the rectangle defined by the given rows.
    public IEnumerable<int> SelectRowRange(int startRow, int endRow)
    {
        IEnumerable<int> result = Enumerable.Empty<int>();
        for (int i = startRow; i <= endRow; ++i)
        {
            result.Concat(SelectRow(i));
        }
        return result;
    }
    */
}