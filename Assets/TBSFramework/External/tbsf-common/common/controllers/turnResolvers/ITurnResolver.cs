using System;
using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Players;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.Controllers.TurnResolvers
{
    /// <summary>
    /// Defines the contract for resolving turns in the game, including determining the start and transition between turns.
    /// </summary>
    public interface ITurnResolver
    {
        /// <summary>
        /// Resolves the start of the game to determine the initial turn context.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        /// <returns>The initial turn context.</returns>
        TurnContext ResolveStart(GridController gridController);

        /// <summary>
        /// Resolves the next turn in the game.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        /// <returns>The turn context for the next turn.</returns>
        TurnContext ResolveTurn(GridController gridController);
    }

    /// <summary>
    /// Represents the context for a player's turn, including the current player and the units available to act.
    /// </summary>
    public readonly struct TurnContext
    {
        /// <summary>
        /// Gets the player whose turn it currently is.
        /// </summary>
        public readonly IPlayer CurrentPlayer { get; }

        /// <summary>
        /// Gets a function that returns the units available to the current player during their turn.
        /// </summary>
        public readonly Func<IEnumerable<IUnit>> PlayableUnits { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TurnContext"/> struct with the specified player and playable units.
        /// </summary>
        /// <param name="nextPlayer">The player whose turn it is.</param>
        /// <param name="playableUnits">A function that provides the units available for the player's turn.</param>
        public TurnContext(IPlayer nextPlayer, Func<IEnumerable<IUnit>> playableUnits)
        {
            CurrentPlayer = nextPlayer;
            PlayableUnits = playableUnits;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TurnContext"/> struct with the specified player and a collection of playable units.
        /// </summary>
        /// <param name="nextPlayer">The player whose turn it is.</param>
        /// <param name="playableUnits">The units available for the player's turn.</param>
        public TurnContext(IPlayer nextPlayer, IEnumerable<IUnit> playableUnits) : this(nextPlayer, () => playableUnits)
        {
        }
    }
}