using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Utilities;

namespace TurnBasedStrategyFramework.Common.Units
{
    /// <summary>
    /// Defines movement-related behavior for a unit.
    /// </summary>
    /// <remarks>
    /// This interface is used to separate concerns related to movement, keeping
    /// the <see cref="IUnit"/> interface cleaner and easier to manage.
    /// </remarks>
    public interface IMoveable
    {
        /// <summary>
        /// The current movement points available to the unit.
        /// </summary>
        float MovementPoints { get; set; }

        /// <summary>
        /// The maximum movement points available to the unit.
        /// </summary>
        float MaxMovementPoints { get; set; }

        /// <summary>
        /// The speed at which the movement animation occurs.
        /// </summary>
        float MovementAnimationSpeed { get; set; }

        /// <summary>
        /// Triggered when the unit has moved to a new grid position.
        /// </summary>
        event Action<UnitMovedEventArgs> UnitMoved;

        /// <summary>
        /// Triggered when the unit leaves its current cell.
        /// </summary>
        event Action<UnitChangedGridPositionEventArgs> UnitLeftCell;

        /// <summary>
        /// Triggered when the unit enters a new cell.
        /// </summary>
        event Action<UnitChangedGridPositionEventArgs> UnitEnteredCell;

        /// <summary>
        /// Triggered when the unit's world position changes.
        /// </summary>
        event Action<UnitPositionChangedEventArgs> UnitWorldPositionChanged;

        /// <summary>
        /// Invokes the event to indicate the unit has moved.
        /// </summary>
        /// <param name="eventArgs">The event arguments containing move details.</param>
        void InvokeUnitMoved(UnitMovedEventArgs eventArgs);

        /// <summary>
        /// Invokes the event to indicate the unit has left its current cell.
        /// </summary>
        /// <param name="eventArgs">The event arguments containing cell transition details.</param>
        void InvokeUnitLeftCell(UnitChangedGridPositionEventArgs eventArgs);

        /// <summary>
        /// Invokes the event to indicate the unit has entered a new cell.
        /// </summary>
        /// <param name="eventArgs">The event arguments containing cell transition details.</param>
        void InvokeUnitEnteredCell(UnitChangedGridPositionEventArgs eventArgs);

        /// <summary>
        /// Invokes the event to indicate the unit's world position has changed.
        /// </summary>
        /// <param name="eventArgs">The event arguments containing position change details.</param>
        void InvokeUnitPositionChanged(UnitPositionChangedEventArgs eventArgs);

        /// <summary>
        /// Indicates if the unit is capable of moving to the cell given as parameter.
        /// </summary>
        /// <param name="cell">The cell to check for movement eligibility.</param>
        /// <returns>True if the unit can move to the cell, otherwise false.</returns>
        bool IsCellMovableTo(ICell destination);

        /// <summary>
        /// Determines if the unit can traverse from one cell to an adjacent cell.
        /// </summary>
        /// <param name="source">The current cell the unit is in.</param>
        /// <param name="destination">The neighboring cell the unit wants to move into.</param>
        /// <returns>True if the unit can traverse from source to destination; otherwise, false.</returns>
        bool IsCellTraversable(ICell source, ICell destination);

        /// <summary>
        /// Calculates the movement cost required for the unit to move from the source cell to the destination cell.
        /// </summary>
        /// <param name="source">The starting cell.</param>
        /// <param name="destination">The cell the unit wants to move to.</param>
        /// <returns>The numeric movement cost for moving between the two cells.</returns>
        float GetMovementCost(ICell source, ICell destination);

        /// <summary>
        /// Retrieves all available cells that the unit can move to from its current position.
        /// </summary>
        /// <param name="cells">The collection of cells to consider for movement.</param>
        /// <returns>A collection of cells the unit can move to.</returns>
        List<ICell> GetAvailableDestinations(IEnumerable<ICell> cells);

        /// <summary>
        /// Finds a path to the specified destination cell.
        /// </summary>
        /// <param name="destination">The target cell.</param>
        /// <param name="cellManager">The cell manager.</param>
        /// <returns>A collection of cells representing the path to the destination. Empty enumerable if there is no path.</returns>
        List<ICell> FindPath(ICell destination, ICellManager cellManager);

        /// <summary>
        /// Creates a graph representation of the grid for pathfinding purposes.
        /// </summary>
        /// <param name="cellManager">The cell manager.</param>
        /// <returns>A dictionary representing the graph edges with movement costs.</returns>
        Dictionary<ICell, Dictionary<ICell, float>> GetGraphEdges(ICellManager cellManager);

        /// <summary>
        /// Caches potential paths for the unit to improve movement performance.
        /// </summary>
        /// <param name="cellManager">The cell manager responsible for managing the grid.</param>
        void CachePaths(ICellManager cellManager);

        /// <summary>
        /// Invalidates any cached paths, typically when the game state changes.
        /// </summary>
        void InvalidateCache();

        /// <summary>
        /// Animates the movement of the unit along a specified path.
        /// </summary>
        /// <param name="path">The path that the unit will follow.</param>
        /// <param name="destination">The destination cell of the unit.</param>
        /// <returns>A task representing the asynchronous movement animation.</returns>
        Task MovementAnimation(IEnumerable<ICell> path, ICell destination);
    }

    /// <summary>
    /// Event arguments for when a unit has moved from one cell to another.
    /// </summary>
    public readonly struct UnitMovedEventArgs
    {
        public readonly IUnit AffectedUnit;
        public readonly ICell SourceCell;
        public readonly ICell TargetCell;
        public readonly IEnumerable<ICell> Path;

        public UnitMovedEventArgs(IUnit affectedUnit, ICell sourceCell, ICell targetCell, IEnumerable<ICell> path)
        {
            SourceCell = sourceCell;
            TargetCell = targetCell;
            Path = path;
            AffectedUnit = affectedUnit;
        }
    }

    /// <summary>
    /// Event arguments for when a unit's world position changes.
    /// </summary>
    public readonly struct UnitPositionChangedEventArgs
    {
        public readonly IUnit AffectedUnit;
        public readonly Vector3Impl PreviousPosition;
        public readonly Vector3Impl CurrentPosition;

        public UnitPositionChangedEventArgs(IUnit affectedUnit, Vector3Impl previousPosition, Vector3Impl currentPosition)
        {
            PreviousPosition = previousPosition;
            CurrentPosition = currentPosition;
            AffectedUnit = affectedUnit;
        }
    }

    /// <summary>
    /// Event arguments for when a unit changes from one grid cell to another.
    /// </summary>
    public readonly struct UnitChangedGridPositionEventArgs
    {
        public readonly IUnit AffectedUnit;
        public readonly ICell LeftCell;
        public readonly ICell EnteredCell;

        public UnitChangedGridPositionEventArgs(IUnit affectedUnit, ICell leftCell, ICell enteredCell)
        {
            AffectedUnit = affectedUnit;
            LeftCell = leftCell;
            EnteredCell = enteredCell;
        }
    }
}
