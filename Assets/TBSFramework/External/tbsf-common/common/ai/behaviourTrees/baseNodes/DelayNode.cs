using System.Threading.Tasks;

namespace TurnBasedStrategyFramework.Common.AI.BehaviourTrees
{
    /// <summary>
    /// Represents a delay node in a behavior tree, used to introduce a delay before executing the child node.
    /// </summary>
    public struct DelayNode : ITreeNode
    {
        /// <summary>
        /// The child node to be executed after the delay.
        /// </summary>
        private readonly ITreeNode _node;

        /// <summary>
        /// The number of turns to delay before executing the child node.
        /// </summary>
        private readonly int _delay;

        /// <summary>
        /// Tracks the number of turns that have passed since the delay started.
        /// </summary>
        private int _turnsPassed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayNode"/> struct with the specified child node and delay duration.
        /// </summary>
        /// <param name="node">The child node that will be executed after the delay.</param>
        /// <param name="delay">The number of turns to delay the execution of the child node.</param>
        public DelayNode(ITreeNode node, int delay)
        {
            _node = node;
            _delay = delay;
            _turnsPassed = 0;
        }

        /// <summary>
        /// Executes the delay node. If the delay period has not passed, it returns false.
        /// Once the delay has elapsed, it executes the child node.
        /// </summary>
        /// <returns>A task representing the execution, with a boolean result indicating success or failure of the child node.</returns>
        public async Task<bool> Execute(bool debugMode)
        {
            if (_turnsPassed < _delay)
            {
                _turnsPassed++;
                return false;
            }
            else
            {
                _turnsPassed = 0;
                return await _node.Execute(debugMode);
            }
        }
    }
}
