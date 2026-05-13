using TurnBasedStrategyFramework.Common.Controllers;

namespace TurnBasedStrategyFramework.Common.Players
{
    /// <summary>
    /// Defines the contract for a player in the game, representing either a human or AI-controlled player.
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// Gets the unique identifier for this player.
        /// </summary>
        int PlayerNumber { get; set; }

        /// <summary>
        /// Gets the type of the player, indicating whether they are human or automated (AI).
        /// </summary>
        PlayerType PlayerType { get; set; }

        /// <summary>
        /// Initializes the player with the specified grid controller. 
        /// This method is called once when the game starts to set up the player's state and context.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        void Initialize(GridController gridController);

        /// <summary>
        /// Allows the current player to make actions during their turn. 
        /// For human players, this typically involves awaiting user input, while for AI players, it involves decision-making logic.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        void Play(GridController gridController);
    }

    /// <summary>
    /// Enum representing the different types of players in the game.
    /// </summary>
    public enum PlayerType
    {
        /// <summary>
        /// Represents a player controlled by a human.
        /// </summary>
        HumanPlayer,

        /// <summary>
        /// Represents a player controlled by AI (automated).
        /// </summary>
        AutomatedPlayer
    }
}
