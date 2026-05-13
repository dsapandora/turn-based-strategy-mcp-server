using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Pathfinding.Algorithms;

namespace TurnBasedStrategyFramework.Common.Units
{
    /// <summary>
    /// Represents the movement component of a unit, providing pathfinding, movement validation, and caching mechanisms.
    /// </summary>
    public abstract class MoveComponent
    {
        /// <summary>
        /// Reference to the unit that owns this movement component.
        /// </summary>
        protected readonly IUnit _unitReference;

        /// <summary>
        /// Caches the paths from the current cell to other cells for movement efficiency.
        /// </summary>
        private Dictionary<ICell, ICell> _pathCache;
        private Dictionary<ICell, float> _costSoFar;

        /// <summary>
        /// Instance of the DijkstraPathfinding algorithm used for calculating paths.
        /// </summary>
        private static DijkstraPathfinding pathfinder = new DijkstraPathfinding();

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveComponent"/> class with a reference to the owning unit.
        /// </summary>
        /// <param name="unitReference">The unit that owns this movement component.</param>
        public MoveComponent(IUnit unitReference)
        {
            _unitReference = unitReference;
        }

        /// <summary>
        /// Indicates if the unit is capable of moving to the cell given as parameter.
        /// </summary>
        /// <param name="cell">The cell to check for movement eligibility.</param>
        /// <returns>True if the unit can move to the cell, otherwise false.</returns>
        public bool IsCellMovableTo(ICell cell)
        {
            return !cell.IsTaken;
        }

        /// <summary>
        /// Determines if the unit can traverse from one cell to an adjacent cell.
        /// </summary>
        /// <param name="source">The current cell the unit is in.</param>
        /// <param name="destination">The neighboring cell the unit wants to move into.</param>
        /// <returns>True if the unit can traverse from source to destination; otherwise, false.</returns>
        public bool IsCellTraversable(ICell source, ICell destination)
        {
            return !destination.IsTaken;

        }

        /// <summary>
        /// Calculates the movement cost required for the unit to move from the source cell to the destination cell.
        /// </summary>
        /// <param name="source">The starting cell.</param>
        /// <param name="destination">The cell the unit wants to move to.</param>
        /// <returns>The numeric movement cost for moving between the two cells.</returns>
        public float GetMovementCost(ICell source, ICell destination)
        {
            return destination.MovementCost;
        }

        /// <summary>
        /// Gets the available destination cells that the unit can move to based on its current movement points.
        /// </summary>
        /// <param name="cells">A collection of cells to check for potential movement.</param>
        /// <returns>An enumerable collection of cells that the unit can reach.</returns>
        public List<ICell> GetAvailableDestinations(IEnumerable<ICell> cells)
        {
            Debug.Assert(_pathCache != null, "Path cache is null, call CachePaths first");

            List<ICell> availableDestinations = new List<ICell>();
            foreach (var cell in cells)
            {
                if (!_unitReference.IsCellMovableTo(cell))
                {
                    continue;
                }
                if (_costSoFar.TryGetValue(cell, out var cost) && cost <= _unitReference.MovementPoints)
                {
                    availableDestinations.Add(cell);
                }
            }
            return availableDestinations;
        }

        /// <summary>
        /// Constructs a graph representation of cells and their movement costs for pathfinding purposes.
        /// </summary>
        /// <param name="cellManager">The cell manager that provides access to all cells.</param>
        /// <returns>A dictionary representing the graph edges, with cells as keys and movement costs as values.</returns>
        public Dictionary<ICell, Dictionary<ICell, float>> GetGraphEdges(ICellManager cellManager)
        {
            var allCells = cellManager.GetCells().ToList();
            var graph = new Dictionary<ICell, Dictionary<ICell, float>>(allCells.Count);

            foreach (var source in allCells)
            {
                var edges = new Dictionary<ICell, float>(6);
                foreach (var destination in source.GetNeighbours(cellManager))
                {
                    if (_unitReference.IsCellTraversable(source, destination) || destination.Equals(_unitReference.CurrentCell))
                    {
                        edges[destination] = _unitReference.GetMovementCost(source, destination);
                    }
                }

                if (edges.Count > 0 || source == _unitReference.CurrentCell)
                {
                    graph[source] = edges;
                }
            }
            return graph;
        }

        /// <summary>
        /// Finds the path from the unit's current position to the specified destination cell.
        /// </summary>
        /// <param name="destination">The destination cell.</param>
        /// <param name="cellManager">The cell manager that provides access to all cells.</param>
        /// <returns>An enumerable collection of cells representing the path to the destination, or an empty collection if no path is found.</returns>
        public List<ICell> FindPath(ICell destination, ICellManager cellManager)
        {
            Debug.Assert(_pathCache != null, "Path cache is null, call CachePaths first");
            return pathfinder.ReconstructPath(_unitReference.CurrentCell, destination, _pathCache, new List<ICell>(capacity:(int)_unitReference.MovementPoints));
        }

        /// <summary>
        /// Caches all paths from the unit's current cell to other cells for efficient movement calculations.
        /// </summary>
        /// <param name="cellManager">The cell manager that provides access to all cells.</param>
        public void CachePaths(ICellManager cellManager)
        {
            (_pathCache, _costSoFar) = pathfinder.FindAllPaths(_unitReference.GetGraphEdges(cellManager), _unitReference.CurrentCell);
        }

        /// <summary>
        /// Invalidates the cached paths, typically when the game state changes in a way that affects movement.
        /// </summary>
        public void InvalidateCache()
        {
            _pathCache = null;
            _costSoFar = null;
        }

        /// <summary>
        /// Performs the movement animation for the unit along a specified path to the destination cell.
        /// This method must be implemented by derived classes to provide specific animation logic.
        /// </summary>
        /// <param name="path">The path the unit will follow.</param>
        /// <param name="destination">The destination cell.</param>
        /// <returns>A task representing the asynchronous animation of the movement.</returns>
        public abstract Task MovementAnimation(IEnumerable<ICell> path, ICell destination);
    }
}