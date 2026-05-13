using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Cells;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Cells
{
    /// <summary>
    /// Unity-specific implementation of a square cell.
    /// </summary>
    public class Square : Cell
    {
        public override CellShape CellShape { get; protected set; } = CellShape.Square;
        [field: SerializeField] public override Vector3 CellDimensions { get; protected set; } = new Vector3(1f, 1f, 1f);

        public override int GetDistance(ICell otherCell)
        {
            return SquareHelper.GetDistance(this, otherCell);
        }
        public override IEnumerable<ICell> GetNeighbours(ICellManager cellManager)
        {
            return SquareHelper.GetNeighbours(this, cellManager);
        }
    }
}