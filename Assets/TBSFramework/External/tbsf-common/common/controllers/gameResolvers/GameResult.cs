using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Players;

namespace TurnBasedStrategyFramework.Common.Controllers.GameResolvers
{
    /// <summary>
    /// Represents the result of the game, including the winning and losing players.
    /// </summary>
    public readonly struct GameResult
    {
        /// <summary>
        /// Gets the collection of players who won the game.
        /// </summary>
        public readonly IEnumerable<IPlayer> Winners { get; }

        /// <summary>
        /// Gets the collection of players who lost the game.
        /// </summary>
        public readonly IEnumerable<IPlayer> Losers { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameResult"/> struct with the specified winners and losers.
        /// </summary>
        /// <param name="winners">The collection of players who won the game.</param>
        /// <param name="losers">The collection of players who lost the game.</param>
        public GameResult(IEnumerable<IPlayer> winners, IEnumerable<IPlayer> losers)
        {
            Winners = winners;
            Losers = losers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameResult"/> struct with a single winner and multiple losers.
        /// </summary>
        /// <param name="winner">The player who won the game.</param>
        /// <param name="losers">The collection of players who lost the game.</param>
        public GameResult(IPlayer winner, IEnumerable<IPlayer> losers) : this(new List<IPlayer>() { winner }, losers)
        {
        }
    }
}