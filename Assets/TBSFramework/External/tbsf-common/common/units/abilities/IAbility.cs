using System;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;

namespace TurnBasedStrategyFramework.Common.Units.Abilities
{
    /// <summary>
    /// Represents an interface for defining abilities that units can perform in the game.
    /// Includes methods for initialization, ability execution, and handling various grid interactions.
    /// </summary>
    public interface IAbility
    {
        event Action<IAbility> AbilitySelected;
        event Action<IAbility> AbilityDeselected;

        void InvokeAbilitySelected();
        void InvokeAbilityDeselected();

        /// <summary>
        /// Gets or sets the reference to the unit that owns this ability.
        /// </summary>
        IUnit UnitReference { get; set; }

        /// <summary>
        /// Initializes the ability with necessary setup steps.
        /// This method is called once when the ability is registered by the unit.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        void Initialize(IGridController gridController);

        /// <summary>
        /// Displays the ability on the grid, typically by marking relevant cells or units.
        /// This is often used for showing movement ranges, attackable targets, or other actionable areas.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        void Display(IGridController gridController);

        /// <summary>
        /// Cleans up any visual indicators or temporary effects related to the ability.
        /// This includes removing highlights or markings made during the display phase.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        void CleanUp(IGridController gridController);

        /// <summary>
        /// Called when a unit is clicked while this ability is selected.
        /// </summary>
        /// <param name="unit">The unit that was clicked.</param>
        /// <param name="gridController">The grid controller.</param>
        void OnUnitClicked(IUnit unit, IGridController gridController);

        /// <summary>
        /// Called when a unit is highlighted while this ability is selected.
        /// </summary>
        /// <param name="unit">The unit that was highlighted.</param>
        /// <param name="gridController">The grid controller.</param>
        void OnUnitHighlighted(IUnit unit, IGridController gridController);

        /// <summary>
        /// Called when a unit is dehighlighted while this ability is selected.
        /// </summary>
        /// <param name="unit">The unit that was dehighlighted.</param>
        /// <param name="gridController">The grid controller.</param>
        void OnUnitDehighlighted(IUnit unit, IGridController gridController);

        /// <summary>
        /// Called when the unit associated with this ability is destroyed.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        void OnUnitDestroyed(IGridController gridController);

        /// <summary>
        /// Called when a cell is clicked while this ability is selected.
        /// </summary>
        /// <param name="cell">The cell that was clicked.</param>
        /// <param name="gridController">The grid controller.</param>
        void OnCellClicked(ICell cell, IGridController gridController);

        /// <summary>
        /// Called when a cell is selected (highlighted) while this ability is active.
        /// </summary>
        /// <param name="cell">The cell that was selected.</param>
        /// <param name="gridController">The grid controller.</param>
        void OnCellHighlighted(ICell cell, IGridController gridController);

        /// <summary>
        /// Called when a cell is deselected (dehighlighted) while this ability is active.
        /// </summary>
        /// <param name="cell">The cell that was deselected.</param>
        /// <param name="gridController">The grid controller.</param>
        void OnCellDehighlighted(ICell cell, IGridController gridController);

        /// <summary>
        /// Called when the unit associated with this ability is selected.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        void OnAbilitySelected(IGridController gridController);

        /// <summary>
        /// Called when the unit associated with this ability is deselected.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        void OnAbilityDeselected(IGridController gridController);

        /// <summary>
        /// Called at the start of the unit's turn to initialize the ability's state.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        void OnTurnStart(IGridController gridController);

        /// <summary>
        /// Called at the end of the unit's turn.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        void OnTurnEnd(IGridController gridController);

        /// <summary>
        /// Determines if the ability can currently be performed.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        /// <returns>True if the ability can be performed, otherwise false.</returns>
        bool CanPerform(IGridController gridController);
    }

    /// <summary>
    /// Represents event arguments for when an ability is used, containing details about the command and actions to execute before and after.
    /// </summary>
    public readonly struct AbilityUsedEventArgs
    {
        /// <summary>
        /// Unit executing the ability
        /// </summary>
        public readonly IUnit Unit;

        /// <summary>
        /// The command to execute when the ability is used.
        /// </summary>
        public readonly ICommand Command;

        /// <summary>
        /// The action to perform before the command is executed.
        /// </summary>
        public readonly Func<IGridController, Task> PreAction;

        /// <summary>
        /// The action to perform after the command is executed.
        /// </summary>
        public readonly Func<IGridController, Task> PostAction;

        /// <summary>
        /// Indicates whether the action was triggered by a remote player.
        /// </summary>
        public readonly bool IsNetworkInvoked;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbilityUsedEventArgs"/> struct.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="preAction">The action to perform before the command is executed.</param>
        /// <param name="postAction">The action to perform after the command is executed.</param>
        /// <param name="isNetworkInvoked">Indicates whether the action was triggered by a remote player.
        public AbilityUsedEventArgs(IUnit unit, ICommand command, Func<IGridController, Task> preAction, Func<IGridController, Task> postAction, bool isNetworkInvoked = false)
        {
            Unit = unit;
            Command = command;
            PreAction = preAction;
            PostAction = postAction;
            IsNetworkInvoked = isNetworkInvoked;
        }
    }
}
