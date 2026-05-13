using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Units.Abilities;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities
{
    /// <summary>
    /// An ability that deals damage to multiple targets within a radius, centered on a selected cell.
    /// </summary>
    public class AreaEffectDamageAbility : ClashOfHeroesAbility
    {
        [SerializeField] private int _range;
        [SerializeField] private int _radius;
        [SerializeField] private int _damage;
        [SerializeField] private bool _withConfirmation;
        private bool _isConfirmed;
        private ICell _confirmedTarget;

        private IEnumerable<ICell> _cellsInRange;
        private IEnumerable<IUnit> _unitsInRange;

        public override void OnAbilitySelected(IGridController gridController)
        {
            _isConfirmed = false;
            _confirmedTarget = null;

            _cellsInRange = Enumerable.Empty<ICell>();
            _unitsInRange = Enumerable.Empty<IUnit>();
        }

        public override void OnCellClicked(ICell cell, IGridController gridController)
        {
            if (!_withConfirmation || (_isConfirmed && cell.Equals(_confirmedTarget)))
            {
                HumanExecuteAbility(
                    new MultipleTargetAttackCommand(_unitsInRange, _damage, 0,
                        () => gridController.UnitManager.MarkAsAttacking(UnitReference, _unitsInRange.FirstOrDefault()),
                        (u) =>
                        {
                            var tcs = new TaskCompletionSource<Task>();
                            gridController.UnitManager.MarkAsDefending(u, UnitReference);

                            tcs.SetResult(Task.CompletedTask);

                            return tcs.Task;
                        }
                    ),
                    gridController
                );
            }
            else
            {
                if (!cell.Equals(_confirmedTarget))
                {
                    Dehighlight(gridController);
                }

                Highlight(cell, gridController);
                _confirmedTarget = cell;
                _isConfirmed = true;
            }
        }

        public override void OnUnitClicked(IUnit unit, IGridController gridController)
        {
            OnCellClicked(unit.CurrentCell, gridController);
        }

        public override void OnCellHighlighted(ICell cell, IGridController gridController)
        {
            if (!_withConfirmation)
            {
                Highlight(cell, gridController);
            }

        }

        private void Highlight(ICell cell, IGridController gridController)
        {
            _cellsInRange = gridController.CellManager.GetCells().Where(c => c.GetDistance(cell) <= _radius);
            _unitsInRange = _cellsInRange.SelectMany(c => c.CurrentUnits).ToList();

            gridController.CellManager.MarkAsReachable(_cellsInRange);
            gridController.UnitManager.MarkAsTargetable(_unitsInRange);
        }

        private void Dehighlight(IGridController gridController)
        {
            gridController.CellManager.UnMark(_cellsInRange);
            gridController.UnitManager.UnMark(_unitsInRange.Where(u => !u.PlayerNumber.Equals(UnitReference.PlayerNumber)));
            gridController.UnitManager.MarkAsFriendly(_unitsInRange.Where(u => u.PlayerNumber.Equals(UnitReference.PlayerNumber)));

        }
        public override void OnCellDehighlighted(ICell cell, IGridController gridController)
        {
            if (!_withConfirmation)
            {
                Dehighlight(gridController);
            }
            if (_cellsInRange.Contains(cell))
            {
                gridController.CellManager.MarkAsReachable(cell);
            }
        }

        public override void OnUnitHighlighted(IUnit unit, IGridController gridController)
        {
            OnCellHighlighted(unit.CurrentCell, gridController);
        }

        public override void OnUnitDehighlighted(IUnit unit, IGridController gridController)
        {
            OnCellDehighlighted(unit.CurrentCell, gridController);
        }

        public override void CleanUp(IGridController gridController)
        {
            Dehighlight(gridController);
        }
    }
}