using System;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.AI.Evaluators
{
    /// <summary>
    /// Evaluates a position based on the distance from the current position of the unit, considering the maximum turns to reach the destination.
    /// </summary>
    public readonly struct DistancePositionEvaluator : IPositionEvaluator
    {
        public readonly float Weight { get; }

        /// <summary>
        /// The maximum distance to destination, used as a threshold for evaluation.
        /// </summary>
        private readonly float _maxDistance;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistancePositionEvaluator"/> struct with the specified weight and maximum destination.
        /// </summary>
        /// <param name="weight">The weight of this evaluator in the scoring system.</param>
        /// <param name="maxDistance">The maximum distance to the destination.</param>
        public DistancePositionEvaluator(float weight, float maxDistance)
        {
            Weight = weight;
            _maxDistance = maxDistance;
        }

        public void Initialize(IUnit evaluatingUnit, IGridController gridController)
        {
        }

        /// <summary>
        /// Evaluates a position based on the distance to the specified cell.
        /// </summary>
        /// <param name="evaluatedCell">The cell to evaluate.</param>
        /// <param name="evaluatingUnit">The unit performing the evaluation.</param>
        /// <param name="gridController">The grid controller managing the game state.</param>
        /// <returns>A float value representing the evaluation score, based on the distance to the destination.</returns>
        public readonly float EvaluatePosition(ICell evaluatedCell, IUnit evaluatingUnit, IGridController gridController)
        {
            return Math.Min(evaluatedCell.GetDistance(evaluatingUnit.CurrentCell) / _maxDistance, 1);
        }
    }
}
