using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Cells;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Cells
{
    /// <summary>
    /// Unity-specific implementation of a hexagonal cell.
    /// </summary>
    public class Hexagon : Cell
    {
        [field: SerializeField][field: HideInInspector] public HexGridType GridType { get; set; } = HexGridType.odd_r;
        public override CellShape CellShape { get; protected set; } = CellShape.Hexagon;
        [field: SerializeField] public override Vector3 CellDimensions { get; protected set; } = new Vector3(2f, 2f, 1.73f);

        public override int GetDistance(ICell otherCell)
        {
            return HexagonHelper.GetDistance(this, otherCell, GridType);
        }
        public override IEnumerable<ICell> GetNeighbours(ICellManager cellManager)
        {
            return HexagonHelper.GetNeighbours(this, cellManager, GridType);
        }

        public override void CopyFields(ICell newCell)
        {
            base.CopyFields(newCell);
            (newCell as Hexagon).GridType = GridType;
        }
    }
}