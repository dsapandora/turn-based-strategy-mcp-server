using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.AI.Evaluators;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Units.Abilities;

namespace TurnBasedStrategyFramework.Common.AI.BehaviourTrees
{
    /// <summary>
    /// Represents an attack action node in a behavior tree, responsible for executing an attack action by evaluating and selecting a target.
    /// </summary>
    public class AttackActionNode : ITreeNode
    {
        /// <summary>
        /// The unit that will execute the attack.
        /// </summary>
        private readonly IUnit _unit;

        /// <summary>
        /// The grid controller responsible for managing the game state.
        /// </summary>
        private readonly IGridController _gridController;

        /// <summary>
        /// The evaluators used to determine the best target for the attack.
        /// </summary>
        private readonly IEnumerable<ITargetEvaluator> _targetEvaluators;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackActionNode"/> class with the specified unit, grid controller, and target evaluators.
        /// </summary>
        /// <param name="unit">The unit that will perform the attack.</param>
        /// <param name="gridController">The grid controller.</param>
        /// <param name="targetEvaluators">The collection of target evaluators used to assess the value of potential targets.</param>
        public AttackActionNode(IUnit unit, IGridController gridController, IEnumerable<ITargetEvaluator> targetEvaluators)
        {
            _unit = unit;
            _gridController = gridController;
            _targetEvaluators = targetEvaluators;
        }

        /// <summary>
        /// Executes the attack action by selecting the best target based on the provided evaluators and initiating the attack.
        /// </summary>
        /// <returns>A task representing the execution, with a boolean result indicating success.</returns>
        public Task<bool> Execute(bool debugMode)
        {
            if (_unit.ActionPoints <= 0)
            {
                return Task.FromResult(false);
            }

            var enemyUnits = _gridController.UnitManager.GetEnemyUnits(_unit.PlayerNumber);
            var attackableUnits = enemyUnits.Where(u => _unit.IsUnitAttackable(u, u.CurrentCell, _unit.CurrentCell));

            foreach (var evaluator in _targetEvaluators)
            {
                evaluator.Initialize(_unit, _gridController);
            }

            IUnit target = null;
            float maxScore = float.MinValue;
            foreach (var unit in attackableUnits)
            {
                float currentScore = _targetEvaluators.Sum(e => e.EvaluateTarget(unit, _unit, _gridController));
                if (currentScore > maxScore)
                {
                    maxScore = currentScore;
                    target = unit;
                }
            }

            var tcs = new TaskCompletionSource<bool>();

            _unit.AIExecuteAbility(new AttackCommand(target, _unit.CalculateTotalDamage(target)), _gridController, tcs);
            return tcs.Task;
        }
    }
}
