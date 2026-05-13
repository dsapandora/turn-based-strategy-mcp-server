using System;
using System.Threading.Tasks;

namespace TurnBasedStrategyFramework.Common.AI.BehaviourTrees
{
    /// <summary>
    /// Represents a random decision node in a behavior tree, used to introduce probabilistic behavior.
    /// <remarks>
    /// The <see cref="RandomNode"/> is useful when you want to include non-deterministic behavior in a behavior tree. 
    /// The success or failure of the node is determined based on a random probability.
    /// </remarks>
    /// </summary>
    public readonly struct RandomNode : ITreeNode
    {
        /// <summary>
        /// The function that generates a random double value, should return a value between 0.0 (inclusive) and 1.0 (exclusive)
        /// </summary>
        private readonly Func<double> _rng;

        /// <summary>
        /// The success probability, between 0.0 and 1.0.
        /// </summary>
        private readonly double _successProbability;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomNode"/> struct with the specified random function and success probability.
        /// </summary>
        /// <param name="rng">A function that generates a random double between 0.0 (inclusive) and 1.0 (exclusive).</param>
        /// <param name="successProbability">The probability, between 0.0 and 1.0, of this node returning success.</param>
        public RandomNode(Func<double> rng, double successProbability)
        {
            _rng = rng;
            _successProbability = successProbability;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomNode"/> struct using a provided <see cref="Random"/> object.
        /// </summary>
        /// <param name="rng">An instance of <see cref="Random"/> to generate random double values.</param>
        /// <param name="successProbability">The probability, between 0.0 and 1.0 of this node returning success.</param>
        public RandomNode(Random rng, double successProbability)
            : this(() => rng.NextDouble(), successProbability)
        {
        }

        /// <summary>
        /// Executes the random node by generating a random value and comparing it against the success probability.
        /// </summary>
        /// <returns>
        /// A task representing the execution of the node. The task result is <c>true</c> if the generated random value is greater than or equal to <c>successProbability</c>, otherwise <c>false</c>.
        /// </returns>
        public Task<bool> Execute(bool debugMode)
        {
            return Task.FromResult(_rng() < _successProbability);
        }
    }
}
