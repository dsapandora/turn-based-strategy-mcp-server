using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Controllers.GridStates;

namespace TurnBasedStrategyFramework.Common.Units.Abilities
{
    /// <summary>
    /// Represents an implementation of an attack ability for a unit.
    /// This ability allows a unit to attack enemy units within range and handles the visual representation of attackable units.
    /// </summary>
    public class AttackAbilityImpl : IAbility
    {
        public event Action<IAbility> AbilitySelected;
        public event Action<IAbility> AbilityDeselected;

        /// <summary>
        /// A set of units that are attackable by the unit with this ability.
        /// </summary>
        private HashSet<IUnit> _attackableUnits;

        /// <summary>
        /// Gets or sets the reference to the unit that owns this ability.
        /// </summary>
        public IUnit UnitReference { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackAbilityImpl"/> class with the specified unit reference.
        /// </summary>
        /// <param name="unitReference">The unit that owns this ability.</param>
        public AttackAbilityImpl(IUnit unitReference)
        {
            UnitReference = unitReference;
        }

        /// <summary>
        /// Called when this ability is selected by the unit, identifying attackable enemy units.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        public void OnAbilitySelected(IGridController gridController)
        {
            var enemyUnits = gridController.UnitManager.GetEnemyUnits(gridController.TurnContext.CurrentPlayer);
            _attackableUnits = new HashSet<IUnit>(enemyUnits.Where(u => UnitReference.IsUnitAttackable(u, u.CurrentCell, UnitReference.CurrentCell)));
        }

        /// <summary>
        /// Displays the ability on the grid, highlighting attackable enemy units.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        public async void Display(IGridController gridController)
        {
            await gridController.UnitManager.MarkAsTargetable(_attackableUnits);
        }

        /// <summary>
        /// Cleans up any visual indicators or temporary effects related to this ability, such as removing highlights from attackable units.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        public void CleanUp(IGridController gridController)
        {
            gridController.UnitManager.UnMark(_attackableUnits);
        }

        /// <summary>
        /// Called when a unit is clicked while this ability is selected.
        /// Executes the attack if the clicked unit is attackable.
        /// </summary>
        /// <param name="unit">The unit that was clicked.</param>
        /// <param name="gridController">The grid controller.</param>
        public void OnUnitClicked(IUnit unit, IGridController gridController)
        {
            if (UnitReference.ActionPoints > 0 && _attackableUnits.Contains(unit))
            {
                UnitReference.HumanExecuteAbility(new AttackCommand(unit, UnitReference.CalculateTotalDamage(unit)), gridController);
            }
            else if (gridController.UnitManager.GetFriendlyUnits(UnitReference.PlayerNumber).Contains(unit))
            {
                gridController.GridState = new GridStateUnitSelected(unit, unit.GetBaseAbilities());
            }
        }

        /// <summary>
        /// Determines whether the unit can perform the attack ability.
        /// Returns true if the unit has action points and there are attackable enemy units within range; otherwise, returns false.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        /// <returns>True if the attack can be performed; otherwise, false.</returns>
        public bool CanPerform(IGridController gridController)
        {
            if (UnitReference.ActionPoints <= 0)
            {
                return false;
            }

            var enemyUnits = gridController.UnitManager.GetEnemyUnits(gridController.PlayerManager.GetPlayerByNumber(UnitReference.PlayerNumber));
            var attackableUnits = enemyUnits.Where(u => UnitReference.IsUnitAttackable(u, u.CurrentCell, UnitReference.CurrentCell));
            return attackableUnits.Any();
        }

        public void Initialize(IGridController gridController)
        {
        }

        public void OnUnitHighlighted(IUnit unit, IGridController gridController)
        {
        }

        public void OnUnitDehighlighted(IUnit unit, IGridController gridController)
        {
        }

        public void OnUnitDestroyed(IGridController gridController)
        {
        }

        public void OnCellClicked(ICell cell, IGridController gridController)
        {
        }

        public void OnCellHighlighted(ICell cell, IGridController gridController)
        {
        }

        public void OnCellDehighlighted(ICell cell, IGridController gridController)
        {
        }

        public void OnAbilityDeselected(IGridController gridController)
        {
        }

        public void OnTurnStart(IGridController gridController)
        {
        }

        public void OnTurnEnd(IGridController gridController)
        {
        }

        public void InvokeAbilitySelected()
        {
            AbilitySelected.Invoke(this);
        }

        public void InvokeAbilityDeselected()
        {
            AbilityDeselected.Invoke(this);
        }
    }
}