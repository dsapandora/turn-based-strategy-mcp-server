using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Cells;

namespace TurnBasedStrategyFramework.Common.AI.BehaviourTrees
{
    /// <summary>
    /// Represents a node in a behavior tree that checks if a specific cell is taken.
    /// </summary>
    public readonly struct CellTakenNode : ITreeNode
    {
        /// <summary>
        /// The cell to be checked by this node.
        /// </summary>
        private readonly ICell _cell;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellTakenNode"/> struct with the specified cell.
        /// </summary>
        /// <param name="cell">The cell to check if it is taken.</param>
        public CellTakenNode(ICell cell)
        {
            _cell = cell;
        }

        /// <summary>
        /// Executes the cell taken node by returning whether the specified cell is currently taken.
        /// </summary>
        /// <returns>A task representing the execution, with a boolean result indicating if the cell is taken.</returns>
        public Task<bool> Execute(bool debugMode)
        {
            return Task.FromResult(_cell.IsTaken);
        }
    }
}
