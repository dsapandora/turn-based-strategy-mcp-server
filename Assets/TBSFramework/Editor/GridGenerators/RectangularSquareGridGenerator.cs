using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Utilities;
using TurnBasedStrategyFramework.Unity.Cells;
using UnityEditor;
using UnityEngine;

namespace TbsFramework.EditorUtils.GridGenerators
{
    /// <summary>
    /// Generates rectangular shaped grid of squares.
    /// </summary>
    [ExecuteInEditMode()]
    public class RectangularSquareGridGenerator : ICellGridGenerator
    {
        public GameObject SquarePrefab;

        public int Width;
        public int Height;

        public override GridInfo GenerateGrid()
        {
            var cells = new List<Cell>();

            if (SquarePrefab.GetComponent<Square>() == null)
            {
                Debug.LogError("Invalid square cell prefab provided");
                return null;
            }

            var squareSize = SquarePrefab.GetComponent<Cell>().CellDimensions;

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    var square = PrefabUtility.InstantiatePrefab(SquarePrefab) as GameObject;
                    var x = (i + 0.5f) * squareSize.x;
                    var y = (j + 0.5f) * squareSize.y;
                    var z = (j + 0.5f) * squareSize.z;

                    var position = Is2D ? new Vector3(x, y, 0) : new Vector3(x, 0, z);

                    square.transform.position = position;
                    var gridCoordinates = new Vector2IntImpl(i, j);
                    square.GetComponent<Cell>().GridCoordinates = gridCoordinates;
                    square.GetComponent<Cell>().MovementCost = 1;
                    square.name = $"{square.name}_{gridCoordinates}";
                    cells.Add(square.GetComponent<Cell>());

                    square.transform.parent = CellsParent;
                }
            }

            return GetGridInfo(cells);
        }
    }
}