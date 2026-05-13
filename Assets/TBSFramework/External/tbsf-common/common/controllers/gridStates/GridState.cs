using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.Controllers.GridStates
{
    /// <summary>
    /// Represents the base class for the state of the grid, handling interactions between units and cells as well as transitions between different states.
    /// </summary>
    public class GridState
    {
        /// <summary>
        /// Initiates a transition to the specified next GridState.
        /// </summary>
        /// <param name="nextState">The next state to transition to.</param>
        /// <returns>The next GridState after the transition.</returns>
        public virtual GridState MakeTransition(GridState nextState)
        {
            return nextState;
        }

        /// <summary>
        /// Called when the grid state is entered.
        /// This method can be overridden to provide behavior specific to entering a particular state.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        public virtual void OnStateEnter(GridController gridController)
        {
        }

        /// <summary>
        /// Called when the grid state is exited.
        /// This method can be overridden to provide behavior specific to exiting a particular state.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        public virtual void OnStateExit(GridController gridController)
        {
        }

        /// <summary>
        /// Called when a cell is clicked.
        /// </summary>
        /// <param name="cell">The cell that was clicked.</param>
        /// <param name="gridController">The grid controller.</param>
        public virtual void OnCellClicked(ICell cell, GridController gridController)
        {
        }

        /// <summary>
        /// Called when a cell is dehighlighted.
        /// </summary>
        /// <param name="cell">The cell that was dehighlighted.</param>
        /// <param name="gridController">The grid controller.</param>
        public virtual void OnCellDehighlighted(ICell cell, GridController gridController)
        {
            gridController.CellManager.UnMarkAsHighlighted(cell);
        }

        /// <summary>
        /// Called when a cell is highlighted.
        /// </summary>
        /// <param name="cell">The cell that was highlighted.</param>
        /// <param name="gridController">The grid controller.</param>
        public virtual void OnCellHighlighted(ICell cell, GridController gridController)
        {
            gridController.CellManager.MarkAsHighlighted(cell);
        }

        /// <summary>
        /// Called when a unit is dehighlighted.
        /// </summary>
        /// <param name="unit">The unit that was dehighlighted.</param>
        /// <param name="gridController">The grid controller.</param>
        public virtual void OnUnitDehighlighted(IUnit unit, GridController gridController)
        {
        }

        /// <summary>
        /// Called when a unit is highlighted.
        /// </summary>
        /// <param name="unit">The unit that was highlighted.</param>
        /// <param name="gridController">The grid controller.</param>
        public virtual void OnUnitHighlighted(IUnit unit, GridController gridController)
        {
        }

        /// <summary>
        /// Called when a unit is clicked.
        /// </summary>
        /// <param name="unit">The unit that was clicked.</param>
        /// <param name="gridController">The grid controller.</param>
        public virtual void OnUnitClicked(IUnit unit, GridController gridController)
        {
        }

        /// <summary>
        /// Ends the current turn.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        public virtual void EndTurn(GridController gridController, bool isNetworkInvoked = false)
        {
            gridController.MakeTurnTransition(isNetworkInvoked);
        }
    }
}