using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.AI.Evaluators
{
    /// <summary>
    /// Represents an interface for evaluating positions on the grid, used by the AI to determine the best position to move to.
    /// </summary>
    public interface IPositionEvaluator
    {
        /// <summary>
        /// Gets the weight of the evaluator, used to influence the overall evaluation score.
        /// </summary>
        float Weight { get; }

        /// <summary>
        /// Initializes the evaluator with the necessary context, such as the unit being evaluated and the grid controller.
        /// Initialization is run each time a MoveActionNode is executed.
        /// </summary>
        /// <param name="evaluatingUnit">The unit that is evaluating potential positions.</param>
        /// <param name="gridController">The grid controller.</param>
        void Initialize(IUnit evaluatingUnit, IGridController gridController);

        /// <summary>
        /// Evaluates the specified cell to determine its suitability as a position for the unit.
        /// </summary>
        /// <param name="evaluatedCell">The cell to evaluate.</param>
        /// <param name="evaluatingUnit">The unit performing the evaluation.</param>
        /// <param name="gridController">The grid controller managing the game state.</param>
        /// <returns>A float value representing the score of the evaluated position.</returns>
        float EvaluatePosition(ICell evaluatedCell, IUnit evaluatingUnit, IGridController gridController);
    }
}
