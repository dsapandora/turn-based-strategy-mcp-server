using System;
using System.Threading.Tasks;

namespace TurnBasedStrategyFramework.Common.AI.BehaviourTrees
{
    /// <summary>
    /// Represents a function node in a behavior tree, used to execute a provided function.
    /// <remarks>
    /// The <see cref="FuncNode"/> is a utility node for executing arbitrary logic, often used for prototyping or 
    /// integrating custom behaviors without creating a dedicated node class. It acts as an "inline action" node, 
    /// simplifying the inclusion of one-off or unique actions.
    /// </remarks>
    /// </summary>
    public readonly struct FuncNode : ITreeNode
    {
        /// <summary>
        /// The function to be executed by this node.
        /// </summary>
        private readonly Func<Task<bool>> _func;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncNode"/> struct with the specified function.
        /// </summary>
        /// <param name="func">The function that this node will execute.</param>
        public FuncNode(Func<Task<bool>> func)
        {
            _func = func;
        }

        /// <summary>
        /// Executes the function node by invoking the provided function.
        /// </summary>
        /// <returns>A task representing the execution, with a boolean result indicating success or failure.</returns>
        public Task<bool> Execute(bool debugMode)
        {
            return _func();
        }
    }
}
