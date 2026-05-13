using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Utilities;

namespace TurnBasedStrategyFramework.Common.Cells
{
    /// <summary>
    /// Represents a manager responsible for managing cells within a grid, including adding, removing, and marking cells.
    /// </summary>
    public interface ICellManager
    {
        /// <summary>
        /// Triggered when a cell is added to the grid.
        /// </summary>
        event Action<ICell> CellAdded;

        /// <summary>
        /// Triggered when a cell is removed from the grid.
        /// </summary>
        event Action<ICell> CellRemoved;

        /// <summary>
        /// Initializes the CellManager when the game start.
        /// </summary>
        void Initialize(IGridController gridController);

        /// <summary>
        /// Retrieves all cells managed by the cell manager.
        /// </summary>
        /// <returns>An enumerable collection of all cells.</returns>
        IEnumerable<ICell> GetCells();

        /// <summary>
        /// Retrieves the cell at the specified grid coordinates.
        /// </summary>
        /// <param name="gridCoordinates">The grid coordinates of the desired cell.</param>
        /// <returns>The cell at the given coordinates, or null if no cell is found.</returns>
        ICell GetCellAt(Vector2IntImpl gridCoordinates);

        /// <summary>
        /// Unmarks the specified cells, typically used to reset their visual representation.
        /// </summary>
        /// <param name="cells">The cells to unmark.</param>
        /// <returns>A task representing the asynchronous unmarking operation.</returns>
        Task UnMark(IEnumerable<ICell> cells);

        /// <summary>
        /// Unmarks the specified cell, typically used to reset their visual representation.
        /// </summary>
        /// <param name="cells">The cells to unmark.</param>
        /// <returns>A task representing the asynchronous unmarking operation.</returns>
        Task UnMark(ICell cell);

        /// <summary>
        /// Marks the specified cell as selected, typically used for indicating the current focus or selection.
        /// </summary>
        /// <param name="cell">The cell to mark as selected.</param>
        /// <returns>A task representing the asynchronous marking operation.</returns>
        Task MarkAsHighlighted(ICell cell);

        /// <summary>
        /// Unmarks the specified cell as selected, typically used to remove visual indicators of selection.
        /// </summary>
        /// <param name="cell">The cell to unmark as selected.</param>
        /// <returns>A task representing the asynchronous unmarking operation.</returns>
        Task UnMarkAsHighlighted(ICell cell);

        /// <summary>
        /// Marks the specified cells as reachable, typically used to indicate potential movement destinations.
        /// </summary>
        /// <param name="cells">The cells to mark as reachable.</param>
        /// <returns>A task representing the asynchronous marking operation.</returns>
        Task MarkAsReachable(IEnumerable<ICell> cells);

        /// <summary>
        /// Marks the specified cell as reachable, typically used to indicate potential movement destination.
        /// </summary>
        /// <param name="cells">The cells to mark as reachable.</param>
        /// <returns>A task representing the asynchronous marking operation.</returns>
        Task MarkAsReachable(ICell cell);

        /// <summary>
        /// Marks the specified cells as part of a path, typically used to indicate a planned movement path.
        /// </summary>
        /// <param name="cells">The cells that form part of the path.</param>
        /// <param name="originCell">The origin cell from which the path starts.</param>
        /// <returns>A task representing the asynchronous marking operation.</returns>
        Task MarkAsPath(IEnumerable<ICell> cells, ICell originCell);

        void SetColor(ICell cell, float r, float g, float b, float a);
    }
}
