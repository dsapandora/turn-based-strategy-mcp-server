using System.Linq;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.AI.Evaluators
{
    /// <summary>
    /// Evaluates a target based on the potential damage that can be dealt to that target by the evaluating unit.
    /// </summary>
    public struct DamageDealtTargetEvaluator : ITargetEvaluator
    {
        /// <summary>
        /// The maximum possible damage that can be dealt to any enemy unit in range, used for normalization.
        /// </summary>
        private float _maxPossibleDamage;

        public readonly float Weight { get; }

        public DamageDealtTargetEvaluator(float weight)
        {
            Weight = weight;
            _maxPossibleDamage = 0;
        }

        /// <summary>
        /// Initializes the evaluator by determining the maximum possible damage the evaluating unit can deal to any enemy unit in range.
        /// </summary>
        /// <param name="evaluatingUnit">The unit performing the evaluation.</param>
        /// <param name="gridController">The grid controller managing the game state.</param>
        public void Initialize(IUnit evaluatingUnit, IGridController gridController)
        {
            var enemyUnits = gridController.UnitManager.GetEnemyUnits(evaluatingUnit.PlayerNumber);
            var enemyUnitsInRange = enemyUnits.Where(u => evaluatingUnit.IsUnitAttackable(u, u.CurrentCell, evaluatingUnit.CurrentCell));
            _maxPossibleDamage = enemyUnitsInRange.Max(u => evaluatingUnit.CalculateTotalDamage(u));
        }

        /// <summary>
        /// Evaluates the specified target based on the potential damage that can be dealt by the evaluating unit.
        /// </summary>
        /// <param name="evaluatedTarget">The target unit to evaluate.</param>
        /// <param name="evaluatingUnit">The unit performing the evaluation.</param>
        /// <param name="gridController">The grid controller managing the game state.</param>
        /// <returns>A float value representing the score of the evaluated target, normalized by the maximum possible damage.</returns>
        public readonly float EvaluateTarget(IUnit evaluatedTarget, IUnit evaluatingUnit, IGridController gridController)
        {
            return evaluatingUnit.CalculateTotalDamage(evaluatedTarget) / _maxPossibleDamage;
        }
    }
}
