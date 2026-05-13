using System.Threading.Tasks;

namespace TurnBasedStrategyFramework.Common.AI.BehaviourTrees
{
    /// <summary>
    /// Represents a succeder node in a behavior tree, which always returns success regardless of the child node's result.
    /// </summary>
    public readonly struct SuccederNode : ITreeNode
    {
        /// <summary>
        /// The child node whose result will be ignored by this succeder node.
        /// </summary>
        private readonly ITreeNode _node;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuccederNode"/> struct with the specified child node.
        /// </summary>
        /// <param name="node">The child node whose result will be ignored.</param>
        public SuccederNode(ITreeNode node)
        {
            _node = node;
        }

        /// <summary>
        /// Executes the succeder node by executing the child node and always returning true.
        /// The result of the child node is effectively ignored.
        /// </summary>
        /// <returns>A task representing the execution, with a boolean result always indicating success.</returns>
        public readonly async Task<bool> Execute(bool debugMode)
        {
            await _node.Execute(debugMode);
            return true;
        }
    }
}
