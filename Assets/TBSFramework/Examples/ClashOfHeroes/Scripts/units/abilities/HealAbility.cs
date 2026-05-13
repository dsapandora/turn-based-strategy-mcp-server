using System.Collections.Generic;
using System.Linq;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Controllers.GridStates;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities
{
    /// <summary>
    /// An ability that heals friendly units within a specified range.
    /// </summary>
    public class HealAbility : ClashOfHeroesAbility
    {
        [SerializeField] private int _healAmount;
        [SerializeField] private int _healRange;

        private IEnumerable<IUnit> _unitsInRange;

        public override void OnAbilitySelected(IGridController gridController)
        {
            _unitsInRange = gridController.UnitManager.GetFriendlyUnits(UnitReference.PlayerNumber)
                .Where(u => u.CurrentCell.GetDistance(UnitReference.CurrentCell) <= _healRange && u.Health < u.MaxHealth);
        }

        public override void Display(IGridController gridController)
        {
            gridController.UnitManager.MarkAsTargetable(_unitsInRange);
        }

        public override void CleanUp(IGridController gridController)
        {
            gridController.UnitManager.MarkAsFriendly(_unitsInRange);
        }

        public override void OnUnitClicked(IUnit unit, IGridController gridController)
        {
            if (_unitsInRange.Contains(unit))
            {
                HumanExecuteAbility(new HealCommand(_healAmount, unit, () => (UnitReference as Unit).GetComponent<IHealHighlighter>().ApplyHealEffect()), gridController);
            }
            else if (gridController.UnitManager.GetFriendlyUnits(UnitReference.PlayerNumber).Contains(unit))
            {
                gridController.GridState = new GridStateUnitSelected(unit, unit.GetBaseAbilities());
            }
            else
            {
                gridController.GridState = new GridStateAwaitInput();
            }
        }
        public override void OnCellClicked(ICell cell, IGridController gridController)
        {
            gridController.GridState = new GridStateAwaitInput();
        }
    }
}