using System;
using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Utilities;

namespace TurnBasedStrategyFramework.Common.Cells
{
    /// <summary>
    /// Provides utility methods for working with square grids, including distance calculation and neighbor retrieval.
    /// </summary>
    public static class SquareHelper
    {
        /// <summary>
        /// Calculates the distance between two cells in a square grid.
        /// Distance is given using the Manhattan norm.
        /// </summary>
        /// <param name="a">The first cell.</param>
        /// <param name="b">The second cell.</param>
        /// <returns>The Manhattan distance between the two cells.</returns>
        public static int GetDistance(ICell a, ICell b)
        {
            return Math.Abs(a.GridCoordinates.x - b.GridCoordinates.x) + Math.Abs(a.GridCoordinates.y - b.GridCoordinates.y);
        }

        /// <summary>
        /// Directions used to determine the neighboring positions of a square cell.
        /// </summary>
        private static readonly Vector2IntImpl[] _directions =
        {
            new Vector2IntImpl(1, 0), new Vector2IntImpl(-1, 0), new Vector2IntImpl(0, 1), new Vector2IntImpl(0, -1)
        };

        /// <summary>
        /// Retrieves the neighboring cells of a given cell in a square grid.
        /// Each square cell has up to four neighbors, which positions relative to the cell are stored in the _directions array.
        /// </summary>
        /// <param name="cell">The cell whose neighbors are being retrieved.</param>
        /// <param name="cellManager">The cell manager responsible.</param>
        /// <returns>A collection of neighboring cells.</returns>
        public static IEnumerable<ICell> GetNeighbours(ICell cell, ICellManager cellManager)
        {
            foreach (var direction in _directions)
            {
                var neighbour = cellManager.GetCellAt(cell.GridCoordinates.Add(direction));
                if (neighbour == null)
                {
                    continue;
                }
                yield return neighbour;
            }
        }
    }
}
