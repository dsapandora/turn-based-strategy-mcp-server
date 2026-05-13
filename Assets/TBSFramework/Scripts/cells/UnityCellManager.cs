using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Utilities;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Cells
{
    /// <summary>
    /// Abstract, Unity-specific implementation of the cell manager, responsible for managing cells within a game grid and handling their visual states.
    /// </summary>
    public abstract class UnityCellManager : MonoBehaviour, ICellManager
    {
        public abstract event Action<ICell> CellAdded;
        public abstract event Action<ICell> CellRemoved;

        public abstract void Initialize(IGridController gridController);
        public abstract ICell GetCellAt(Vector2IntImpl coords);
        public abstract IEnumerable<ICell> GetCells();

        public abstract Task MarkAsPath(IEnumerable<ICell> cells, ICell originCell);
        public abstract Task MarkAsReachable(IEnumerable<ICell> cells);
        public abstract Task MarkAsReachable(ICell cell);
        public abstract Task MarkAsHighlighted(ICell cell);
        public abstract Task UnMarkAsHighlighted(ICell cell);
        public abstract Task UnMark(IEnumerable<ICell> cells);
        public abstract Task UnMark(ICell cell);
        public abstract void SetColor(ICell cell, float r, float g, float b, float a);
    }
}