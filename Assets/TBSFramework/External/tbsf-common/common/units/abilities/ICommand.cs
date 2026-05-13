using System.Collections.Generic;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Controllers;

namespace TurnBasedStrategyFramework.Common.Units.Abilities
{
    /// <summary>
    /// Defines a command that can be executed and undone in the game.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes the command for the specified unit.
        /// </summary>
        /// <param name="unit">The unit performing the command.</param>
        /// <param name="controller">The grid controller.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Execute(IUnit unit, IGridController controller);

        /// <summary>
        /// Reverts the effects of the command for the specified unit, effectively undoing the action.
        /// </summary>
        /// <param name="unit">The unit for which the command is being undone.</param>
        /// <param name="controller">The grid controller.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Undo(IUnit unit, IGridController controller);

        /// <summary>
        /// Encapsulates the command's parameters into a dictionary for network transmission.
        /// </summary>
        /// <returns>A dictionary with string keys and values containing the serialized command parameters.</returns>
        Dictionary<string, object> Serialize();

        /// <summary>
        /// Reconstructs a command from the provided serialized data and game context.
        /// </summary>
        /// <param name="actionParams">A dictionary containing the serialized command parameters.</param>
        /// <param name="gridController">The grid controller.</param>
        /// <returns>The reconstructed command instance.</returns>
        ICommand Deserialize(Dictionary<string, object> actionParams, IGridController gridController);
    }
}
