using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.AI.BehaviourTrees
{
    /// <summary>
    /// Represents a node in a behavior tree that checks if the health of a unit meets a certain threshold.
    /// </summary>
    public readonly struct HealthThresholdNode : ITreeNode
    {
        /// <summary>
        /// The unit whose health will be evaluated.
        /// </summary>
        private readonly IUnit _unit;

        /// <summary>
        /// The health threshold to check against, represented as a fraction of maximum health.
        /// </summary>
        private readonly float _threshold;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthThresholdNode"/> struct with the specified unit and health threshold.
        /// </summary>
        /// <param name="unit">The unit whose health will be checked.</param>
        /// <param name="threshold">The health threshold as a fraction of the unit's maximum health.</param>
        public HealthThresholdNode(IUnit unit, float threshold)
        {
            _unit = unit;
            _threshold = threshold;
        }

        /// <summary>
        /// Executes the health threshold node by checking if the unit's current health is above the specified threshold.
        /// </summary>
        /// <returns>A task representing the execution, with a boolean result indicating whether the unit's health meets or exceeds the threshold.</returns>
        public readonly Task<bool> Execute(bool debugMode)
        {
            return Task.FromResult(_unit.Health / _unit.MaxHealth >= _threshold);
        }
    }
}
