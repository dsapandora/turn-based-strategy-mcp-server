using System.Collections.Generic;
using System.Threading.Tasks;

namespace TurnBasedStrategyFramework.Common.AI.BehaviourTrees
{
    /// <summary>
    /// Represents a selector node in a behavior tree, which iterates over its child nodes until one succeeds.
    /// </summary>
    public readonly struct SelectorNode : ITreeNode
    {
        /// <summary>
        /// The collection of child nodes associated with this selector node.
        /// </summary>
        private readonly IEnumerable<ITreeNode> _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorNode"/> struct with the specified child nodes.
        /// </summary>
        /// <param name="children">The child nodes that this selector will evaluate.</param>
        public SelectorNode(IEnumerable<ITreeNode> children)
        {
            _children = children;
        }

        /// <summary>
        /// Executes the selector node by iterating through its child nodes until one succeeds.
        /// </summary>
        /// <returns>A task representing the execution, with a boolean result indicating whether any child node succeeded.</returns>
        public readonly async Task<bool> Execute(bool debugMode)
        {
            foreach (var child in _children)
            {
                if (await child.Execute(debugMode))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
