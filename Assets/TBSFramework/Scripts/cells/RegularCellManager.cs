using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Utilities;

namespace TurnBasedStrategyFramework.Unity.Cells
{
    /// <summary>
    /// A concrete implementation of <see cref="UnityCellManager"/> for managing regular cells in the grid.
    /// This manager automatically loads cells that are its children in the scene tree.
    /// </summary>
    public class RegularCellManager : UnityCellManager
    {
        public override event Action<ICell> CellAdded;
        public override event Action<ICell> CellRemoved;

        Dictionary<Vector2IntImpl, Cell> _cellCache;

        public override void Initialize(IGridController gridController)
        {
            _cellCache = new Dictionary<Vector2IntImpl, Cell> ();
            foreach (var cell in GetComponentsInChildren<Cell>())
            {
                _cellCache.Add(cell.GridCoordinates, cell);
                CellAdded?.Invoke(cell);
            }
        }

        public override ICell GetCellAt(Vector2IntImpl coords)
        {
            return _cellCache.TryGetValue(coords, out Cell cell) ? cell : null;
        }

        public override IEnumerable<ICell> GetCells()
        {
            return _cellCache.Values;
        }

        /// <summary>
        /// Marks the specified cell as highlighted by delegating to the <see cref="Cell"/> implementation.
        /// </summary>
        /// <param name="cell">The cell to highlight.</param>
        public override async Task MarkAsHighlighted(ICell cell)
        {
            await (cell as Cell).MarkAsHighlighted();
        }

        /// <summary>
        /// Unmarks the specified cell by delegating to the <see cref="Cell"/> implementation.
        /// </summary>
        /// <param name="cell">The cell to unmark.</param>
        public override async Task UnMarkAsHighlighted(ICell cell)
        {
            await (cell as Cell).UnMark();
        }

        /// <summary>
        /// Marks the specified cells as part of a movement path by delegating to each cell's <see cref="Cell.MarkAsPath(IList{ICell}, int, ICell)"/> implementation.
        /// </summary>
        /// <param name="cells">The cells forming the path.</param>
        /// <param name="originCell">The origin cell of the path.</param>
        public override async Task MarkAsPath(IEnumerable<ICell> cells, ICell originCell)
        {
            var path = cells.ToList();
            for (int i = 0; i < path.Count; i++)
            {
                ICell cell = path[i];
                await (cell as Cell).MarkAsPath(path, i, originCell);
            }
        }

        /// <summary>
        /// Unmarks the specified cells by delegating to each cell's <see cref="Cell.UnMark"/> implementation.
        /// </summary>
        /// <param name="cells">The cells to unmark.</param>
        public override async Task UnMark(IEnumerable<ICell> cells)
        {
            await Task.WhenAll(cells.Select(cell => (cell as Cell).UnMark()));
        }

        /// <summary>
        /// Unmarks the specified cell by delegating <see cref="Cell.UnMark"/> implementation.
        /// </summary>
        /// <param name="cells">The cells to unmark.</param>
        public override async Task UnMark(ICell cell)
        {
            await (cell as Cell).UnMark();
        }

        /// <summary>
        /// Marks the specified cells as reachable by delegating to each cell's <see cref="Cell"/> implementation.
        /// </summary>
        /// <param name="cells">The cells to mark as reachable.</param>
        public override async Task MarkAsReachable(IEnumerable<ICell> cells)
        {
            foreach (var cell in cells)
            {
                await MarkAsReachable(cell);
            }
        }

        /// <summary>
        /// Marks the specified cell as reachable by delegating to cell's <see cref="Cell"/> implementation.
        /// </summary>
        /// <param name="cells">The cells to mark as reachable.</param>
        public override async Task MarkAsReachable(ICell cell)
        {
            await (cell as Cell).MarkAsReachable();
        }

        public override void SetColor(ICell cell, float r, float g, float b, float a)
        {
            (cell as Cell).SetColor(r, g, b, a);
        }
    }
}