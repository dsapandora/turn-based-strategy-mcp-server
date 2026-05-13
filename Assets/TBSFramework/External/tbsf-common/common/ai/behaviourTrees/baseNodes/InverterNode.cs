using System.Threading.Tasks;

namespace TurnBasedStrategyFramework.Common.AI.BehaviourTrees
{
    /// <summary>
    /// Represents an inverter node in a behavior tree, which inverts the result of its child node.
    /// </summary>
    public readonly struct InverterNode : ITreeNode
    {
        /// <summary>
        /// The child node whose result will be inverted by this inverter node.
        /// </summary>
        private readonly ITreeNode _node;

        /// <summary>
        /// Initializes a new instance of the <see cref="InverterNode"/> struct with the specified child node.
        /// </summary>
        /// <param name="node">The child node whose result will be inverted.</param>
        /// <param name="logFn">Function for logging messages when debug mode is enabled.</param>
        public InverterNode(ITreeNode node)
        {
            _node = node;
        }

        /// <summary>
        /// Executes the inverter node by executing the child node and inverting its result.
        /// If the child returns true, this node returns false, and vice versa.
        /// </summary>
        /// <returns>A task representing the execution, with a boolean result indicating the inverted success or failure of the child node.</returns>
        public readonly async Task<bool> Execute(bool debugMode)
        {
            var result = await _node.Execute(debugMode);
            return !result;
        }
    }
}
