using System.Collections.Generic;
using System.Threading.Tasks;

namespace TurnBasedStrategyFramework.Common.AI.BehaviourTrees
{
    /// <summary>
    /// Represents a sequence node in a behavior tree, which iterates over its child nodes until one fails.
    /// </summary>
    public readonly struct SequenceNode : ITreeNode
    {
        /// <summary>
        /// The collection of child nodes associated with this sequence node.
        /// </summary>
        private readonly IEnumerable<ITreeNode> _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceNode"/> struct with the specified child nodes.
        /// </summary>
        /// <param name="children">The child nodes that this sequence will evaluate.</param>
        public SequenceNode(IEnumerable<ITreeNode> children)
        {
            _children = children;
        }

        /// <summary>
        /// Executes the sequence node by iterating through its child nodes until one fails.
        /// If all child nodes succeed, the sequence node returns success.
        /// </summary>
        /// <returns>A task representing the execution, with a boolean result indicating whether all child nodes succeeded.</returns>
        public readonly async Task<bool> Execute(bool debugMode)
        {
            foreach (var child in _children)
            {
                if (!await child.Execute(debugMode))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
