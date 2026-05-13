using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;

namespace TurnBasedStrategyFramework.Common.Units.Abilities
{
    /// <summary>
    /// Represents an ability that highlights the attack range of a unit, indicating reachable cells and attackable enemy units.
    /// </summary>
    public class AttackRangeHighlightAbilityImpl : IAbility
    {
        public event Action<IAbility> AbilitySelected;
        public event Action<IAbility> AbilityDeselected;

        /// <summary>
        /// A collection of available destination cells that the unit can move to.
        /// </summary>
        private HashSet<ICell> _availableDestinations;

        /// <summary>
        /// A collection of enemy units that are attackable from the selected cell.
        /// </summary>
        private IEnumerable<IUnit> _attackableUnits;

        /// <summary>
        /// Gets or sets the reference to the unit that owns this ability.
        /// </summary>
        public IUnit UnitReference { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackRangeHighlightAbilityImpl"/> class.
        /// </summary>
        /// <param name="unitReference">The unit that owns this ability.</param>
        public AttackRangeHighlightAbilityImpl(IUnit unitReference)
        {
            UnitReference = unitReference;
        }

        /// <summary>
        /// Called when the unit associated with this ability is selected.
        /// Identifies available destinations and resets attackable units.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        public void OnAbilitySelected(IGridController gridController)
        {
            _attackableUnits = Enumerable.Empty<IUnit>();
            _availableDestinations = UnitReference.GetAvailableDestinations(gridController.CellManager.GetCells()).ToHashSet();
        }

        /// <summary>
        /// Cleans up the visual indicators for attackable units.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        public void CleanUp(IGridController gridController)
        {
            gridController.UnitManager.UnMark(_attackableUnits);
        }

        /// <summary>
        /// Called when a cell is selected.
        /// Identifies attackable units from the selected cell and marks them as reachable enemies.
        /// </summary>
        /// <param name="cell">The cell that was selected.</param>
        /// <param name="gridController">The grid controller.</param>
        public void OnCellHighlighted(ICell cell, IGridController gridController)
        {
            if (_availableDestinations.Contains(cell) && UnitReference.ActionPoints > 0)
            {
                var enemyUnits = gridController.UnitManager.GetEnemyUnits(UnitReference.PlayerNumber);
                _attackableUnits = enemyUnits.Where(u => UnitReference.IsUnitAttackable(u, u.CurrentCell, cell));
                gridController.UnitManager.MarkAsTargetable(_attackableUnits);
            }
        }

        /// <summary>
        /// Called when a cell is deselected.
        /// Unmarks the previously attackable units and resets attackable units to those available from the unit's current position.
        /// </summary>
        /// <param name="cell">The cell that was deselected.</param>
        /// <param name="gridController">The grid controller.</param>
        public void OnCellDehighlighted(ICell cell, IGridController gridController)
        {
            if (_availableDestinations.Contains(cell) && UnitReference.ActionPoints > 0)
            {
                gridController.UnitManager.UnMark(_attackableUnits);
                var enemyUnits = gridController.UnitManager.GetEnemyUnits(UnitReference.PlayerNumber);
                var _attackableUnitsLocal = enemyUnits.Where(u => UnitReference.IsUnitAttackable(u, u.CurrentCell, UnitReference.CurrentCell));
                gridController.UnitManager.MarkAsTargetable(_attackableUnitsLocal);
                _attackableUnits = Enumerable.Empty<IUnit>();
            }
        }

        public void Initialize(IGridController gridController)
        {
        }

        public void Display(IGridController gridController)
        {
        }

        public void OnAbilityDeselected(IGridController gridController)
        {
        }

        public void OnUnitClicked(IUnit unit, IGridController gridController)
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

        public void OnTurnStart(IGridController gridController)
        {
        }

        public void OnTurnEnd(IGridController gridController)
        {
        }

        public bool CanPerform(IGridController gridController)
        {
            return false;
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
