namespace TurnBasedStrategyFramework.Common.Controllers.GridStates
{
    /// <summary>
    /// Represents the state of the grid when the game has ended.
    /// Prevents any further interaction or state changes.
    /// </summary>
    public class GridStateGameEnded : GridState
    {
        /// <summary>
        /// Prevents any state transition once the game has ended.
        /// </summary>
        /// <param name="nextState">The next state to transition to, which will be ignored.</param>
        /// <returns>Returns the current game-ended state to ensure no transition occurs.</returns>
        public override GridState MakeTransition(GridState nextState)
        {
            return this;
        }

        /// <summary>
        /// Prevents ending the turn since the game has already ended.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        public override void EndTurn(GridController gridController, bool isNetworkInvoked = false)
        {
        }
    }
}