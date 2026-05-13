using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.AI.Evaluators
{
    /// <summary>
    /// Represents an interface for evaluating targets, used by the AI to determine the best unit to target.
    /// </summary>
    public interface ITargetEvaluator
    {
        /// <summary>
        /// Gets the weight of the evaluator, used to influence the overall evaluation score.
        /// </summary>
        float Weight { get; }

        /// <summary>
        /// Initializes the evaluator with the necessary context, such as the unit being evaluated and the grid controller.
        /// </summary>
        /// <param name="evaluatingUnit">The unit that is evaluating potential targets.</param>
        /// <param name="gridController">The grid controller managing the game state.</param>
        void Initialize(IUnit evaluatingUnit, IGridController gridController);

        /// <summary>
        /// Evaluates the specified target unit to determine its suitability as a target for the evaluating unit.
        /// </summary>
        /// <param name="evaluatedTarget">The target unit to evaluate.</param>
        /// <param name="evaluatingUnit">The unit performing the evaluation.</param>
        /// <param name="gridController">The grid controller.</param>
        /// <returns>A float value representing the score of the evaluated target.</returns>
        float EvaluateTarget(IUnit evaluatedTarget, IUnit evaluatingUnit, IGridController gridController);
    }
}
