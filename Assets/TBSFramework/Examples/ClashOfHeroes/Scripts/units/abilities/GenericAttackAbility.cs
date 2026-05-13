using System;
using System.Collections.Generic;
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
    /// A generic attack ability with a configurable range and direct damage.
    /// This ability bypasses the default unit damage calculation and applies a specified damage value directly to the target.
    /// </summary>
    public class GenericAttackAbility : ClashOfHeroesAbility
    {
        [SerializeField] private int _range;
        [SerializeField] private int _damage;

        private HashSet<IUnit> _attackableUnits;

        public override void OnAbilitySelected(IGridController gridController)
        {
            var enemyUnits = gridController.UnitManager.GetEnemyUnits(gridController.TurnContext.CurrentPlayer);
            _attackableUnits = new HashSet<IUnit>(enemyUnits.Where(u => u.CurrentCell.GetDistance(UnitReference.CurrentCell) <= _range));
        }

        public override async void Display(IGridController gridController)
        {
            await gridController.UnitManager.MarkAsTargetable(_attackableUnits);
        }

        public override void CleanUp(IGridController gridController)
        {
            gridController.UnitManager.UnMark(_attackableUnits);
        }

        public override void OnUnitClicked(IUnit unit, IGridController gridController)
        {
            if (_attackableUnits.Contains(unit))
            {
                HumanExecuteAbility(new AttackCommand(unit, _damage), gridController);
            }
            else if (gridController.UnitManager.GetFriendlyUnits(UnitReference.PlayerNumber).Contains(unit))
            {
                gridController.GridState = new GridStateUnitSelected(unit, unit.GetBaseAbilities());
            }
        }
        public override void OnCellClicked(ICell cell, IGridController gridController)
        {
            gridController.GridState = new GridStateAwaitInput();
        }
    }
}