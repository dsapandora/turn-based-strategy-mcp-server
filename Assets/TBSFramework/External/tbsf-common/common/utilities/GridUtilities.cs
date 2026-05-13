using System;
using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Cells;

namespace TurnBasedStrategyFramework.Common.Utilities
{
    public static class GridUtilities
    {
        /// <summary>
        /// Gets a list of cells forming a straight line between two hexagonal grid positions using linear interpolation in cube coordinates.
        /// </summary>
        public static IEnumerable<ICell> GetLineOfCells(ICell startCell, ICell endCell, ICellManager cellManager, HexGridType gridType)
        {
            if (startCell == null || endCell == null || cellManager == null || startCell == endCell)
                yield break;

            var start = HexagonHelper.OffsetToCubeCoordinates(startCell.GridCoordinates, gridType);
            var end = HexagonHelper.OffsetToCubeCoordinates(endCell.GridCoordinates, gridType);
            var steps = Math.Max(Math.Abs(end.x - start.x), Math.Max(Math.Abs(end.y - start.y), Math.Abs(end.z - start.z)));

            for (int i = 0; i <= steps; i++)
            {
                var t = (float)i / steps;
                var x = start.x + (end.x - start.x) * t;
                var y = start.y + (end.y - start.y) * t;
                var z = start.z + (end.z - start.z) * t;

                var rx = (int)Math.Round(x);
                var ry = (int)Math.Round(y);
                var rz = (int)Math.Round(z);

                var xDiff = Math.Abs(rx - x);
                var yDiff = Math.Abs(ry - y);
                var zDiff = Math.Abs(rz - z);

                if (xDiff > yDiff && xDiff > zDiff) rx = -ry - rz;
                else if (yDiff > zDiff) ry = -rx - rz;
                else rz = -rx - ry;

                var offsetCoords = HexagonHelper.CubeToOffsetCoordinates(new Vector3IntImpl(rx, ry, rz), gridType);
                var cell = cellManager.GetCellAt(offsetCoords);
                if (cell != null) yield return cell;
            }
        }
    }
}