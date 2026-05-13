using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.AI.Evaluators;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Units.Abilities;

namespace TurnBasedStrategyFramework.Common.AI.BehaviourTrees
{
    /// <summary>
    /// Represents a move action node in a behavior tree, responsible for evaluating potential movement destinations and executing the move action for a unit.
    /// </summary>
    public class MoveActionNode : ITreeNode
    {
        private readonly IUnit _unit;
        private readonly IEnumerable<IPositionEvaluator> _positionEvaluators;
        private readonly IGridController _gridController;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveActionNode"/> class with the specified unit, grid controller, and position evaluators.
        /// </summary>
        /// <param name="unit">The unit that will perform the movement.</param>
        /// <param name="gridController">The grid controller to manage interactions.</param>
        /// <param name="positionEvaluators">The collection of position evaluators used to assess the value of potential movement destinations.</param>
        public MoveActionNode(IUnit unit, IGridController gridController, IEnumerable<IPositionEvaluator> positionEvaluators)
        {
            _unit = unit;
            _positionEvaluators = positionEvaluators;
            _gridController = gridController;
        }

        /// <summary>
        /// Executes the move action by evaluating potential destinations and moving the unit to the best available position.
        /// </summary>
        /// <returns>A task representing the execution, with a boolean result indicating success or failure of the move action.</returns>
        public Task<bool> Execute(bool debugMode)
        {
            foreach (var positionEvaluator in _positionEvaluators)
            {
                positionEvaluator.Initialize(_unit, _gridController);
            }

            var scores = _gridController.CellManager.GetCells()
                .Where(c => _unit.IsCellMovableTo(c) || c.Equals(_unit.CurrentCell))
                .Select(cell =>
                {
                    float scoreSum = _positionEvaluators.Sum(evaluator =>
                        evaluator.EvaluatePosition(cell, _unit, _gridController) * evaluator.Weight);
                    return (cell, scoreSum);
                })
                .OrderByDescending(e => e.scoreSum)
                .ToList();

            _unit.CachePaths(_gridController.CellManager);
            var topDestination = scores.First(e => e.cell.Equals(_unit.CurrentCell) || _unit.FindPath(e.cell, _gridController.CellManager).Any()).cell;

            if (topDestination.Equals(_unit.CurrentCell))
            {
                return Task.FromResult(false);
            }

            var fullPath = _unit.FindPath(topDestination, _gridController.CellManager).ToList();
            var availableDestinations = _unit.GetAvailableDestinations(fullPath);

            var reachablePath = new List<ICell>();
            for (int i = fullPath.Count - 1; i >= 0; i--)
            {
                var cell = fullPath[i];
                if (availableDestinations.Contains(cell))
                {
                    reachablePath = fullPath.Take(i + 1).ToList();
                    break;
                }
            }

            if (!reachablePath.Any())
            {
                return Task.FromResult(false);
            }

            var bestCell = scores
                .Where(s => reachablePath.Contains(s.cell))
                .OrderByDescending(s => s.scoreSum)
                .First().cell;

            var trimmedPath = reachablePath.TakeWhile(c => !c.Equals(bestCell)).Append(bestCell).ToList();

            var tcs = new TaskCompletionSource<bool>();
            _unit.AIExecuteAbility(new MoveCommand(_unit.CurrentCell, bestCell, trimmedPath), _gridController, tcs);
            return tcs.Task;
        }
    }
}
