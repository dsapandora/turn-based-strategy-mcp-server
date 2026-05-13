using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Units.Abilities;

namespace TurnBasedStrategyFramework.Common.Controllers.GridStates
{
    /// <summary>
    /// Represents the state of the grid when a unit whith specific set of abilities is selected by the human player.
    /// </summary>
    public class GridStateUnitSelected : GridState
    {
        /// <summary>
        /// The unit that has been selected.
        /// </summary>
        private readonly IUnit _selectedUnit;

        /// <summary>
        /// The abilities available to the selected unit.
        /// </summary>
        private readonly IEnumerable<IAbility> _abilities;

        private bool _canPerformAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridStateUnitSelected"/> class with the selected unit and its abilities.
        /// </summary>
        /// <param name="selectedUnit">The unit that is selected.</param>
        /// <param name="abilities">The abilities available to the selected unit.</param>
        public GridStateUnitSelected(IUnit selectedUnit, IEnumerable<IAbility> abilities)
        {
            _selectedUnit = selectedUnit;
            _abilities = abilities;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridStateUnitSelected"/> class with the selected unit and a single ability.
        /// </summary>
        /// <param name="selectedUnit">The unit that is selected.</param>
        /// <param name="ability">A single ability available to the selected unit.</param>
        public GridStateUnitSelected(IUnit selectedUnit, IAbility ability) : this(selectedUnit, new IAbility[] { ability })
        {
        }

        /// <summary>
        /// Called when the state is entered.
        /// Marks the selected unit as active and displays available abilities.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        public override async void OnStateEnter(GridController gridController)
        {
            _canPerformAction = false;

            _selectedUnit.InvokeUnitSelected();
            foreach (var ability in _abilities)
            {
                ability.OnAbilitySelected(gridController);
                ability.InvokeAbilitySelected();

                if (!ability.CanPerform(gridController))
                {
                    continue;
                }
                _canPerformAction = true;
                ability.Display(gridController);
            }

            if (_canPerformAction)
            {
                await gridController.UnitManager.MarkAsSelected(_selectedUnit);
            }
            else
            {
                await gridController.UnitManager.MarkAsFinished(new IUnit[] { _selectedUnit });
            }
        }

        /// <summary>
        /// Called when the state is exited.
        /// Cleans up ability indicators and marks the unit appropriately.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        public override async void OnStateExit(GridController gridController)
        {
            _selectedUnit.InvokeUnitDeselected();
            if (_canPerformAction)
            {
                await gridController.UnitManager.MarkAsFriendly(new IUnit[] { _selectedUnit });
            }
            foreach (var ability in _abilities)
            {
                ability.CleanUp(gridController);
                ability.OnAbilityDeselected(gridController);
                ability.InvokeAbilityDeselected();
            }
        }

        /// <summary>
        /// Called when a unit is clicked.
        /// Triggers appropriate responses from the selected abilities.
        /// </summary>
        /// <param name="unit">The unit that was clicked.</param>
        /// <param name="gridController">The grid controller.</param>
        public override void OnUnitClicked(IUnit unit, GridController gridController)
        {
            foreach (var ability in _abilities)
            {
                ability.OnUnitClicked(unit, gridController);
            }
        }

        /// <summary>
        /// Called when a unit is highlighted.
        /// Triggers appropriate responses from the selected abilities.
        /// </summary>
        /// <param name="unit">The unit that was highlighted.</param>
        /// <param name="gridController">The grid controller.</param>
        public override void OnUnitHighlighted(IUnit unit, GridController gridController)
        {
            foreach (var ability in _abilities)
            {
                ability.OnUnitHighlighted(unit, gridController);
            }
        }

        /// <summary>
        /// Called when a unit is dehighlighted.
        /// Triggers appropriate responses from the selected abilities.
        /// </summary>
        /// <param name="unit">The unit that was dehighlighted.</param>
        /// <param name="gridController">The grid controller.</param>
        public override void OnUnitDehighlighted(IUnit unit, GridController gridController)
        {
            foreach (var ability in _abilities)
            {
                ability.OnUnitDehighlighted(unit, gridController);
            }
        }

        /// <summary>
        /// Called when a cell is clicked.
        /// Triggers appropriate responses from the selected abilities.
        /// </summary>
        /// <param name="cell">The cell that was clicked.</param>
        /// <param name="gridController">The grid controller.</param>
        public override void OnCellClicked(ICell cell, GridController gridController)
        {
            foreach (var ability in _abilities)
            {
                ability.OnCellClicked(cell, gridController);
            }
        }

        /// <summary>
        /// Called when a cell is highlighted.
        /// Triggers appropriate responses from the selected abilities.
        /// </summary>
        /// <param name="cell">The cell that was highlighted.</param>
        /// <param name="gridController">The grid controller.</param>
        public override void OnCellHighlighted(ICell cell, GridController gridController)
        {
            base.OnCellHighlighted(cell, gridController);
            foreach (var ability in _abilities)
            {
                ability.OnCellHighlighted(cell, gridController);
            }
        }

        /// <summary>
        /// Called when a cell is dehighlighted.
        /// Triggers appropriate responses from the selected abilities.
        /// </summary>
        /// <param name="cell">The cell that was dehighlighted.</param>
        /// <param name="gridController">The grid controller.</param>
        public override void OnCellDehighlighted(ICell cell, GridController gridController)
        {
            base.OnCellDehighlighted(cell, gridController);
            foreach (var ability in _abilities)
            {
                ability.OnCellDehighlighted(cell, gridController);
            }
        }
    }
}