using System;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.AI.Evaluators
{
    /// <summary>
    /// Evaluates a position with a random score, used to introduce randomness in AI decision making.
    /// </summary>
    public readonly struct RandomPositionEvaluator : IPositionEvaluator
    {
        public readonly float Weight { get; }

        private readonly Random _rng;

        public RandomPositionEvaluator(float weight)
        {
            Weight = weight;
            _rng = new Random();
        }

        public void Initialize(IUnit evaluatingUnit, IGridController gridController)
        {
        }

        /// <summary>
        /// Evaluates a position with a random value to introduce variation in the AI's position choices.
        /// </summary>
        /// <param name="evaluatedCell">The cell to evaluate.</param>
        /// <param name="evaluatingUnit">The unit performing the evaluation.</param>
        /// <param name="gridController">The grid controller managing the game state.</param>
        /// <returns>A float value representing the random evaluation score for the specified position, ranging between -1 and 1.</returns>
        public readonly float EvaluatePosition(ICell evaluatedCell, IUnit evaluatingUnit, IGridController gridController)
        {
            return (float)_rng.NextDouble() * 2 - 1;
        }
    }
}
