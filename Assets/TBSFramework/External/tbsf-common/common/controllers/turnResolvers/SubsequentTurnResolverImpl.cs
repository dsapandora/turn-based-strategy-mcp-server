using System.Linq;

namespace TurnBasedStrategyFramework.Common.Controllers.TurnResolvers
{
    /// <summary>
    /// Implementation of <see cref="ITurnResolver"/> that handles resolving turns sequentially for all players in the game.
    /// </summary>
    public readonly struct SubsequentTurnResolverImpl : ITurnResolver
    {
        /// <summary>
        /// Resolves the start of the game by selecting the first player and their units.
        /// The first player is chosen based on the lowest player number.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        /// <returns>The turn context representing the initial player's turn.</returns>
        public readonly TurnContext ResolveStart(GridController gridController)
        {
            var nextPlayer = gridController.PlayerManager.GetPlayers().OrderBy(p => p.PlayerNumber).FirstOrDefault();
            var allowedUnits = gridController.UnitManager.GetUnits().Where(u => u.PlayerNumber == nextPlayer.PlayerNumber);
            return new TurnContext(nextPlayer, allowedUnits);
        }

        /// <summary>
        /// Resolves the next player's turn based on the current player's position in the turn order.
        /// It ensures that the turn moves sequentially to the next player who has units available.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        /// <returns>The turn context representing the next player's turn.</returns>
        public readonly TurnContext ResolveTurn(GridController gridController)
        {
            var numberOfPlayers = gridController.PlayerManager.GetPlayers().Count();
            var nextPlayerNumber = (gridController.TurnContext.CurrentPlayer.PlayerNumber + 1) % numberOfPlayers;

            while (!gridController.UnitManager.GetUnits().Where(u => u.PlayerNumber.Equals(nextPlayerNumber)).Any())
            {
                nextPlayerNumber = (nextPlayerNumber + 1) % numberOfPlayers;
            }

            var nextPlayer = gridController.PlayerManager.GetPlayers().FirstOrDefault(p => p.PlayerNumber == nextPlayerNumber);
            var allowedUnits = gridController.UnitManager.GetUnits().Where(u => u.PlayerNumber == nextPlayerNumber);

            return new TurnContext(nextPlayer, allowedUnits);
        }
    }
}