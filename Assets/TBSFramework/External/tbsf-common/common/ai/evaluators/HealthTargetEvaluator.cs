using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.AI.Evaluators
{
    /// <summary>
    /// Evaluates a target based on its current health, giving preference to units with lower health.
    /// </summary>
    public readonly struct HealthTargetEvaluator : ITargetEvaluator
    {
        public readonly float Weight { get; }

        public HealthTargetEvaluator(float weight)
        {
            Weight = weight;
        }

        public readonly void Initialize(IUnit evaluatingUnit, IGridController gridController)
        {
        }

        /// <summary>
        /// Evaluates the specified target based on its current health.
        /// A lower health value results in a higher evaluation score, making it a more attractive target.
        /// </summary>
        /// <param name="evaluatedTarget">The target unit to evaluate.</param>
        /// <param name="evaluatingUnit">The unit performing the evaluation.</param>
        /// <param name="gridController">The grid controller managing the game state.</param>
        /// <returns>A float value representing the evaluation score of the target, based on its remaining health.</returns>
        public readonly float EvaluateTarget(IUnit evaluatedTarget, IUnit evaluatingUnit, IGridController gridController)
        {
            return 1 - (evaluatedTarget.Health / evaluatedTarget.MaxHealth);
        }
    }
}
