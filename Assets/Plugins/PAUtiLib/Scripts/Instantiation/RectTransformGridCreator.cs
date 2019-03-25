// Author(s): Paul Calande
// A class for instantiating grids of RectTransforms.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public static class RectTransformGridCreator
{
    // Instantiate a grid of cells, where each cell has a RectTransform.
    // This is good for UI-based grids.
    // T is a component type that is a member of the prefab.
    // This component type will be returned in a List containing this
    // component for each generated cell.
    public static List2<T> MakeGrid<T>(int gridWidth, int gridHeight,
        GameObject cellPrefab, bool cellsAreSquare,
        RectTransform gridContainer, float cellScale = 1.0f,
        Action<T> callback = null, GameObject prefabAxisMarking = null)
    {
        // Calculate the width and height of the cells.
        float cellWidth = gridContainer.rect.width / gridWidth;
        float cellHeight = gridContainer.rect.height / gridHeight;
        Vector2 center = Vector2.zero;

        // To keep the cells square-shaped, make their width and height match.
        if (cellsAreSquare)
        {
            if (cellWidth < cellHeight)
            {
                cellHeight = cellWidth;
            }
            if (cellHeight < cellWidth)
            {
                cellWidth = cellHeight;
            }
        }

        // Calculate the final cell size based on the influence of cellScale.
        // Used for the size delta of the cells' RectTransforms.
        Vector2 cellSize = new Vector2(cellWidth, cellHeight) * cellScale;

        // Calculate the position of the cell at the top-left corner of the grid.
        float cornerX = center.x - ((gridWidth - 1) * cellWidth * 0.5f);
        float cornerY = center.y + ((gridHeight - 1) * cellHeight * 0.5f);

        // Instantiate the List that will be returned by this method.
        List<T> result = new List<T>();

        // Instantiate the grid in row-major order, moving right then down.
        for (int row = 0; row < gridHeight; ++row)
        {
            for (int column = 0; column < gridWidth; ++column)
            {
                // Instantiate a cell using the cell prefab.
                GameObject cellObj = GameObject.Instantiate(cellPrefab, gridContainer);

                // Scale the cell.
                // Cells with scales less than 1.0 will create gaps in the grid.
                RectTransform cellTransform = cellObj.GetComponent<RectTransform>();
                cellTransform.sizeDelta = cellSize;

                // Move the cell to the appropriate position.
                Vector2 position = new Vector2(
                    cornerX + column * cellWidth, cornerY - row * cellHeight);
                cellTransform.localPosition = position;

                // Run the callback on the designated cell component.
                T cellComponent = cellObj.GetComponent<T>();
                if (callback != null)
                {
                    callback(cellComponent);
                }

                // Add the cell's component to the List.
                result.Add(cellComponent);

                // Spawn axis markings, if applicable.
                // TODO: Make this work in more directions and stuff.
                // Also, this part below duplicates code from above.
                // It may also be worth it to put this in a different loop,
                // for the sake of efficiency.
                if (prefabAxisMarking != null)
                {
                    if (column == 0)
                    {
                        int num = gridHeight - row;

                        GameObject labelObj = GameObject.Instantiate(prefabAxisMarking, gridContainer);
                        RectTransform labelTransform = labelObj.GetComponent<RectTransform>();
                        labelTransform.localPosition = position - new Vector2(cellWidth, 0.0f);

                        labelObj.GetComponentInChildren<Text>().text = num.ToString();
                    }
                    if (row == gridHeight - 1)
                    {
                        int num = column + 1;

                        GameObject labelObj = GameObject.Instantiate(prefabAxisMarking, gridContainer);
                        RectTransform labelTransform = labelObj.GetComponent<RectTransform>();
                        labelTransform.localPosition = position - new Vector2(0.0f, cellHeight);

                        labelObj.GetComponentInChildren<Text>().text = num.ToString();
                    }
                }
            }
        }

        return new List2<T>(result,
            new MatrixAccessor(gridWidth, gridHeight, CoordinateOrder2D.RightThenUp, false));
    }
}