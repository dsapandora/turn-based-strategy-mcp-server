using System.Collections.Generic;
using System.Data;
using System.Linq;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Controllers.GridStates;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Units.Abilities;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities
{
    /// <summary>
    /// An ability that deals damage to enemy units based on a percentage of their maximum health.
    /// </summary>
    public class PercentageDamageAbility : ClashOfHeroesAbility
    {
        [SerializeField] private float _damageFraction;
        [SerializeField] private int _range;

        private IEnumerable<IUnit> _unitsInRange;

        public override void OnAbilitySelected(IGridController gridController)
        {
            _unitsInRange = gridController.UnitManager.GetEnemyUnits(UnitReference.PlayerNumber).Where(u => u.CurrentCell.GetDistance(UnitReference.CurrentCell) <= _range);
        }

        public override void Display(IGridController gridController)
        {
            gridController.UnitManager.MarkAsTargetable(_unitsInRange);
        }

        public override void CleanUp(IGridController gridController)
        {
            gridController.UnitManager.UnMark(_unitsInRange);
        }

        public override void OnCellClicked(ICell cell, IGridController gridController)
        {
            gridController.GridState = new GridStateAwaitInput();
        }

        public override void OnUnitClicked(IUnit unit, IGridController gridController)
        {
            if (_unitsInRange.Contains(unit))
            {
                HumanExecuteAbility(new AttackCommand(unit, unit.MaxHealth * _damageFraction, 0), gridController);
            }
            else
            {
                gridController.GridState = new GridStateAwaitInput();
            }
        }
    }
}