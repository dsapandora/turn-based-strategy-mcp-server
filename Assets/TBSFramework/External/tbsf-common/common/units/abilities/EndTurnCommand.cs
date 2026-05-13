using System.Collections.Generic;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Controllers;

namespace TurnBasedStrategyFramework.Common.Units.Abilities
{
    /// <summary>
    /// Represents a command to end the current player's turn.
    /// </summary>
    public class EndTurnCommand : ICommand
    {
        public Task Execute(IUnit unit, IGridController controller)
        {
            controller.MakeTurnTransition();
            return Task.CompletedTask;
        }

        public Task Undo(IUnit unit, IGridController controller)
        {
            return Task.CompletedTask;
        }

        public Dictionary<string, object> Serialize()
        {
            return new Dictionary<string, object> { };
        }

        public ICommand Deserialize(Dictionary<string, object> actionParams, IGridController gridController)
        {
            throw new System.NotImplementedException();
        }
    }
}