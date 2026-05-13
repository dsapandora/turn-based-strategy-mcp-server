using System;
using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Utilities;

namespace TurnBasedStrategyFramework.Common.Cells
{
    /// <summary>
    /// Represents a single cell on the grid.
    /// </summary>
    public interface ICell : IEquatable<ICell>
    {
        /// <summary>
        /// Triggered when the cell is selected, typically when the mouse cursor moves over it.
        /// </summary>
        event Action<ICell> CellHighlighted;

        /// <summary>
        /// Triggered when the cell is dehighlighted, typically when the mouse cursor leaves it.
        /// </summary>
        event Action<ICell> CellDehighlighted;

        /// <summary>
        /// Triggered when the cell is clicked.
        /// </summary>
        event Action<ICell> CellClicked;

        /// <summary>
        /// Invokes the CellHighlighted event to signal that the cell has been highlighted.
        /// </summary>
        public void InvokeCellHighlighted();

        /// <summary>
        /// Invokes the CellDehighlighted event to signal that the cell is no longer highlighted.
        /// </summary>
        public void InvokeCellDehighlighted();

        /// <summary>
        /// Invokes the CellClicked event to signal that the cell has been clicked.
        /// </summary>
        public void InvokeCellClicked();

        /// <summary>
        /// The grid coordinates of the cell, representing its position within the grid.
        /// </summary>
        Vector2IntImpl GridCoordinates { get; set; }

        /// <summary>
        /// Indicates whether the cell is occupied.
        /// </summary>
        bool IsTaken { get; set; }

        /// <summary>
        /// The list of units currently occupying this cell.
        /// </summary>
        IList<IUnit> CurrentUnits { get; }

        /// <summary>
        /// The movement cost required to enter this cell.
        /// </summary>
        float MovementCost { get; set; }

        /// <summary>
        /// The world position of the cell.
        /// </summary>
        Vector3Impl WorldPosition { get; set; }

        /// <summary>
        /// Calculates the grid distance to another cell.
        /// </summary>
        /// <param name="otherCell">The cell to calculate the distance to.</param>
        /// <returns>The distance as an integer.</returns>
        int GetDistance(ICell otherCell);

        /// <summary>
        /// Retrieves the adjacent cells.
        /// </summary>
        /// <param name="cellManager">The grid's cell manager.</param>
        /// <returns>A collection of neighboring cells.</returns>
        IEnumerable<ICell> GetNeighbours(ICellManager cellManager);
    }
}