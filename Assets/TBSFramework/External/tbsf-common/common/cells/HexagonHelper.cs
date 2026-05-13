using System;
using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Utilities;

namespace TurnBasedStrategyFramework.Common.Cells
{
    /// <summary>
    /// Provides utility methods for working with hexagonal grids, including coordinate conversions and neighbor calculations.
    /// </summary>
    public static class HexagonHelper
    {
        /// <summary>
        /// Converts offset coordinates into cube coordinates.
        /// Cube coordinates is another system of coordinates that makes calculation on hex grids easier.
        /// </summary>
        /// <param name="offsetCoordinates">The offset coordinates to be converted.</param>
        /// <param name="gridType">The type of hex grid used (odd_q, even_q, odd_r, even_r).</param>
        /// <returns>The cube coordinates corresponding to the given offset coordinates.</returns>
        public static Vector3IntImpl OffsetToCubeCoordinates(Vector2IntImpl offsetCoordinates, HexGridType gridType)
        {
            Vector3IntImpl cubeCoordinates;
            int cubeCoordinatesX = 0;
            int cubeCoordinatesZ = 0;
            int cubeCoordinatesY = 0;

            switch (gridType)
            {
                case HexGridType.odd_q:
                    {
                        cubeCoordinatesX = offsetCoordinates.x;
                        cubeCoordinatesZ = offsetCoordinates.y - (offsetCoordinates.x + (Math.Abs(offsetCoordinates.x) % 2)) / 2;
                        cubeCoordinatesY = -cubeCoordinatesX - cubeCoordinatesZ;
                        break;
                    }
                case HexGridType.even_q:
                    {
                        cubeCoordinatesX = offsetCoordinates.x;
                        cubeCoordinatesZ = offsetCoordinates.y - (offsetCoordinates.x - (Math.Abs(offsetCoordinates.x) % 2)) / 2;
                        cubeCoordinatesY = -cubeCoordinatesX - cubeCoordinatesZ;
                        break;
                    }
                case HexGridType.odd_r:
                    {
                        cubeCoordinatesX = offsetCoordinates.x - (offsetCoordinates.y - (Math.Abs(offsetCoordinates.y) % 2)) / 2;
                        cubeCoordinatesZ = offsetCoordinates.y;
                        cubeCoordinatesY = -cubeCoordinatesX - cubeCoordinatesZ;
                        break;
                    }
                case HexGridType.even_r:
                    {
                        cubeCoordinatesX = offsetCoordinates.x - (offsetCoordinates.y + (Math.Abs(offsetCoordinates.y) % 2)) / 2;
                        cubeCoordinatesZ = offsetCoordinates.y;
                        cubeCoordinatesY = -cubeCoordinatesX - cubeCoordinatesZ;
                        break;
                    }

            }
            cubeCoordinates = new Vector3IntImpl(cubeCoordinatesX, cubeCoordinatesY, cubeCoordinatesZ);
            return cubeCoordinates;
        }

        /// <summary>
        /// Converts cube coordinates back to offset coordinates.
        /// </summary>
        /// <param name="cubeCoordinates">The cube coordinates to convert.</param>
        /// <param name="gridType">The type of hex grid used (odd_q, even_q, odd_r, even_r).</param>
        /// <returns>Offset coordinates corresponding to the given cube coordinates.</returns>
        public static Vector2IntImpl CubeToOffsetCoordinates(Vector3IntImpl cubeCoordinates, HexGridType gridType)
        {
            Vector2IntImpl offsetCoordinates;
            float offsetCoordinatesX = 0;
            float offsetCoordinatesY = 0;

            switch (gridType)
            {
                case HexGridType.odd_q:
                    {
                        offsetCoordinatesX = cubeCoordinates.x;
                        offsetCoordinatesY = cubeCoordinates.z + (cubeCoordinates.x + (Math.Abs(cubeCoordinates.x) % 2)) / 2;
                        break;
                    }
                case HexGridType.even_q:
                    {
                        offsetCoordinatesX = cubeCoordinates.x;
                        offsetCoordinatesY = cubeCoordinates.z + (cubeCoordinates.x - (Math.Abs(cubeCoordinates.x) % 2)) / 2;
                        break;
                    }
                case HexGridType.odd_r:
                    {
                        offsetCoordinatesX = cubeCoordinates.x + (cubeCoordinates.z - (Math.Abs(cubeCoordinates.z) % 2)) / 2;
                        offsetCoordinatesY = cubeCoordinates.z;
                        break;
                    }
                case HexGridType.even_r:
                    {
                        offsetCoordinatesX = cubeCoordinates.x + (cubeCoordinates.z + (Math.Abs(cubeCoordinates.z) % 2)) / 2;
                        offsetCoordinatesY = cubeCoordinates.z;
                        break;
                    }
            }
            offsetCoordinates = new Vector2IntImpl((int)offsetCoordinatesX, (int)offsetCoordinatesY);
            return offsetCoordinates;
        }

        /// <summary>
        /// Calculates the distance between two cells in a hexagonal grid.
        /// Distance is calculated using the Manhattan norm in cube coordinates.
        /// </summary>
        /// <param name="a">The first cell.</param>
        /// <param name="b">The second cell.</param>
        /// <param name="gridType">The type of hex grid used (odd_q, even_q, odd_r, even_r).</param>
        /// <returns>The distance between the two cells.</returns>
        public static int GetDistance(ICell a, ICell b, HexGridType gridType)
        {
            var cubeCoordinatesA = OffsetToCubeCoordinates(a.GridCoordinates, gridType);
            var cubeCoordinatesB = OffsetToCubeCoordinates(b.GridCoordinates, gridType);

            int distance = (int)(Math.Abs(cubeCoordinatesA.x - cubeCoordinatesB.x) + Math.Abs(cubeCoordinatesA.y - cubeCoordinatesB.y) + Math.Abs(cubeCoordinatesA.z - cubeCoordinatesB.z)) / 2;
            return distance;
        }

        /// <summary>
        /// Directions used to determine the neighboring positions of a hex cell in cube coordinates.
        /// </summary>
        private static readonly Vector3IntImpl[] _directions =  {
            new Vector3IntImpl(+1, -1, 0), new Vector3IntImpl(+1, 0, -1), new Vector3IntImpl(0, +1, -1),
            new Vector3IntImpl(-1, +1, 0), new Vector3IntImpl(-1, 0, +1), new Vector3IntImpl(0, -1, +1)
        };

        /// <summary>
        /// Retrieves the neighboring cells of a given cell in a hexagonal grid.
        /// Each hex cell has six neighbors, which positions relative to the cell are stored in the _directions array.
        /// </summary>
        /// <param name="cell">The cell whose neighbors are being retrieved.</param>
        /// <param name="cellManager">The cell manager.</param>
        /// <param name="gridType">The type of hex grid used (odd_q, even_q, odd_r, even_r).</param>
        /// <returns>A collection of neighboring cells.</returns>
        public static IEnumerable<ICell> GetNeighbours(ICell cell, ICellManager cellManager, HexGridType gridType)
        {
            var cubeCoordinates = OffsetToCubeCoordinates(cell.GridCoordinates, gridType);
            foreach (var direction in _directions)
            {
                var neighbour = cellManager.GetCellAt(CubeToOffsetCoordinates(cubeCoordinates.Add(direction), gridType));
                if (neighbour == null)
                {
                    continue;
                }
                yield return neighbour;
            }
        }
    }

    /// <summary>
    /// Enum representing the different types of hex grid layouts (even or odd column/row).
    /// </summary>
    public enum HexGridType
    {
        even_q,
        odd_q,
        even_r,
        odd_r
    };
}