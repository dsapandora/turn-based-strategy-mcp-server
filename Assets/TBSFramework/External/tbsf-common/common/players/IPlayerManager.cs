using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Controllers;

namespace TurnBasedStrategyFramework.Common.Players
{
    /// <summary>
    /// Defines the contract for managing players in the game, providing methods to retrieve players by various criteria.
    /// </summary>
    public interface IPlayerManager
    {
        /// <summary>
        /// Initializes the PlayerManager when the game start.
        /// </summary>
        void Initialize(GridController gridController);

        /// <summary>
        /// Retrieves all players participating in the game.
        /// </summary>
        /// <returns>An enumerable collection of all players.</returns>
        IEnumerable<IPlayer> GetPlayers();

        /// <summary>
        /// Retrieves a player based on their unique player number.
        /// </summary>
        /// <param name="playerNumber">The unique identifier of the player.</param>
        /// <returns>The player with the specified number, or <c>null</c> if no player matches the given number.</returns>
        IPlayer GetPlayerByNumber(int playerNumber);
    }
}
