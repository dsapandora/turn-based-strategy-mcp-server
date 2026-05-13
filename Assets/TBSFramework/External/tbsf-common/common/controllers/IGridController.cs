using System;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers.GameResolvers;
using TurnBasedStrategyFramework.Common.Controllers.GridStates;
using TurnBasedStrategyFramework.Common.Controllers.TurnResolvers;
using TurnBasedStrategyFramework.Common.Players;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.Controllers
{
    /// <summary>
    /// Interface for managing the grid, units, players, and turns in the game.
    /// </summary>
    public interface IGridController
    {
        /// <summary>
        /// Gets or sets the cell manager responsible for managing cells within the grid.
        /// </summary>
        ICellManager CellManager { get; set; }

        /// <summary>
        /// Gets or sets the unit manager responsible for managing units in the game.
        /// </summary>
        IUnitManager UnitManager { get; set; }

        /// <summary>
        /// Gets or sets the player manager responsible for managing players in the game.
        /// </summary>
        IPlayerManager PlayerManager { get; set; }

        /// <summary>
        /// Gets or sets the turn resolver responsible for determining the order of player turns.
        /// </summary>
        ITurnResolver TurnResolver { get; set; }

        /// <summary>
        /// Gets the current turn context, which provides information about the current player and their playable units.
        /// </summary>
        TurnContext TurnContext { get; }

        /// <summary>
        /// Gets or sets the current state of the grid, managing transitions between different grid states.
        /// </summary>
        GridState GridState { get; set; }

        /// <summary>
        /// Triggered when the game starts.
        /// </summary>
        event Action GameStarted;

        /// <summary>
        /// Triggered when the game is initialized, meaning all relevant data is set up and ready.
        /// </summary>
        event Action GameInitialized;

        /// <summary>
        /// Triggered when the game ends, providing the game result.
        /// </summary>
        event Action<GameResult> GameEnded;

        /// <summary>
        /// Triggered when a new turn starts, providing the turn context.
        /// </summary>
        event Action<TurnTransitionParams> TurnStarted;

        /// <summary>
        /// Triggered when the current turn ends, providing the turn context.
        /// </summary>
        event Action<TurnTransitionParams> TurnEnded;

        /// <summary>
        /// Initializes the game, setting up the initial game state.
        /// </summary>
        /// <param name="isNetworkInvoked">Indicates whether the initialization was triggered by a network event.</param>
        void InitializeGame(bool isNetworkInvoked = false);

        /// <summary>
        /// Starts the game, initializing the first turn and invoking the <see cref="GameStarted"/> event.
        /// </summary>
        /// <param name="isNetworkInvoked">Indicates whether the start was triggered by a network event.</param>
        void StartGame(bool isNetworkInvoked = false);

        /// <summary>
        /// Initialize and start the game in one call.
        /// </summary>
        /// <param name="isNetworkInvoked">Indicates whether the start was triggered by a network event.</param>
        void InitializeAndStart(bool isNetworkInvoked = false);

        /// <summary>
        /// Ends the current turn.
        /// </summary>
        /// <param name="isNetworkInvoked">Indicates whether the end of the turn was triggered by a remote player.</param>
        void EndTurn(bool isNetworkInvoked = false);

        /// <summary>
        /// Handles the transition to the next turn, updating the current turn context and notifying relevant units and abilities.
        /// </summary>
        /// <param name="isNetworkInvoked">Indicates whether the transition was triggered by a remote player.</param>
        void MakeTurnTransition(bool isNetworkInvoked = false);

        /// <summary>
        /// Invokes the <see cref="GameEnded"/> event.
        /// </summary>
        /// <param name="gameResult">The result of the game.</param>
        void InvokeGameEnded(GameResult gameResult);
    }

    /// <summary>
    /// Represents parameters for a turn transition event.
    /// </summary>
    public readonly struct TurnTransitionParams
    {
        /// <summary>
        /// The context of the current turn.
        /// </summary>
        public readonly TurnContext TurnContext;

        /// <summary>
        /// Indicates whether the turn transition was triggered by a remote player.
        /// </summary>
        public readonly bool IsNetworkInvoked;

        /// <summary>
        /// Initializes a new instance of the <see cref="TurnTransitionParams"/> struct.
        /// </summary>
        /// <param name="turnContext">The context of the current turn.</param>
        /// <param name="isNetworkInvoked">Indicates whether the turn transition was triggered by a remote player.</param>
        public TurnTransitionParams(TurnContext turnContext, bool isNetworkInvoked)
        {
            TurnContext = turnContext;
            IsNetworkInvoked = isNetworkInvoked;
        }
    }
}
