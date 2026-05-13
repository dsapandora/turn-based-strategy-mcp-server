using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Controllers.GridStates;

namespace TurnBasedStrategyFramework.Common.Players
{
    /// <summary>
    /// Represents a human player in the game, allowing interaction through user input.
    /// </summary>
    public abstract class HumanPlayer : IPlayer
    {
        public abstract int PlayerNumber { get; set; }
        public PlayerType PlayerType { get; set; }

        public void Initialize(GridController gridController)
        {
        }

        public void Play(GridController gridController)
        {
            gridController.GridState = new GridStateAwaitInput();
        }
    }
}