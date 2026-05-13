using System.Collections.Generic;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.AI.Evaluators;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.AI.BehaviourTrees
{
    /// <summary>
    /// Represents an attack sequence node in a behavior tree, responsible for executing a sequence of actions to perform an attack.
    /// This node first checks if there are enemies in range and then performs the attack.
    /// </summary>
    public readonly struct AttackSequenceNode : ITreeNode
    {
        /// <summary>
        /// The unit that will execute the attack sequence.
        /// </summary>
        private readonly IUnit _unit;

        /// <summary>
        /// The grid controller responsible for managing the game state.
        /// </summary>
        private readonly IGridController _gridController;

        /// <summary>
        /// The evaluators used to determine the best target for the attack.
        /// </summary>
        private readonly IList<ITargetEvaluator> _targetEvaluators;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackSequenceNode"/> struct with the specified unit, grid controller, and target evaluators.
        /// </summary>
        /// <param name="unit">The unit that will perform the attack.</param>
        /// <param name="gridController">The grid controller.</param>
        /// <param name="targetEvaluators">The collection of target evaluators used to assess the value of potential targets.</param>
        public AttackSequenceNode(IUnit unit, IGridController gridController, IList<ITargetEvaluator> targetEvaluators)
        {
            _unit = unit;
            _gridController = gridController;
            _targetEvaluators = targetEvaluators;
        }

        /// <summary>
        /// Executes the attack sequence by first checking for enemies in range and then initiating an attack if possible.
        /// </summary>
        /// <returns>A task representing the execution, with a boolean result indicating success.</returns>
        public Task<bool> Execute(bool debugMode)
        {
            return new SequenceNode(new List<ITreeNode>
            {
                new EnemiesInRangeNode(_unit, _gridController),
                new AttackActionNode(_unit, _gridController, _targetEvaluators)
            }).Execute(debugMode);
        }
    }
}
