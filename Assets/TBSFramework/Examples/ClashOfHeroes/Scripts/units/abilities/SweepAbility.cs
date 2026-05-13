using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Controllers.GridStates;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Units.Abilities;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities
{
    /// <summary>
    /// An ability that deals damage to all adjacent enemy units.
    /// </summary>
    public class SweepAbility : ClashOfHeroesAbility
    {
        [SerializeField] private int _damage;
        private HashSet<IUnit> _unitsInRange;

        public override void OnAbilitySelected(IGridController gridController)
        {
            _unitsInRange = UnitReference.CurrentCell.GetNeighbours(gridController.CellManager).SelectMany(c => c.CurrentUnits).ToHashSet();
        }

        public override void Display(IGridController gridController)
        {
            gridController.UnitManager.MarkAsTargetable(_unitsInRange);
        }

        public override void CleanUp(IGridController gridController)
        {
            gridController.UnitManager.UnMark(_unitsInRange.Where(u => !u.PlayerNumber.Equals(UnitReference.PlayerNumber)));
            gridController.UnitManager.MarkAsFriendly(_unitsInRange.Where(u => u.PlayerNumber.Equals(UnitReference.PlayerNumber)));
        }

        public override void OnUnitClicked(IUnit unit, IGridController gridController)
        {
            if (_unitsInRange.Contains(unit))
            {
                var clickedUnit = unit;
                var centerPosition = UnitReference.WorldPosition;

                var _unitsOrdered = _unitsInRange.OrderBy(unit =>
                    (-Math.Atan2(unit.WorldPosition.z - centerPosition.z, unit.WorldPosition.x - centerPosition.x)
                      - (-Math.Atan2(clickedUnit.WorldPosition.z - centerPosition.z, clickedUnit.WorldPosition.x - centerPosition.x))
                      + 2 * Math.PI) % (2 * Math.PI)
                );

                HumanExecuteAbility(
                    new MultipleTargetAttackCommand(
                        _unitsOrdered,
                        _damage,
                        0,
                        async () => await (UnitReference as Unit)?.GetComponent<ISweepHighlighter>()?.ApplySweepEffect(
                            (UnitReference as Unit).gameObject,
                            new CombatHighlightParams(UnitReference as Unit, unit as Unit)
                        ),
                        async (u) =>
                        {
                            _ = gridController.UnitManager.MarkAsDefending(u, UnitReference);
                            await Awaitable.WaitForSecondsAsync(50f / 1000f);
                        }
                    ),
                    gridController
                );
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