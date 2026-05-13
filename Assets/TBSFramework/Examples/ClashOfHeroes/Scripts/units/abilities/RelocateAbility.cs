using System.Collections.Generic;
using System.Linq;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Controllers.GridStates;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.MobileDemo;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities
{
    /// <summary>
    /// Ability that relocates a unit to a specified position on the grid, bypassing pathfinding. 
    /// Suitable for abilities like teleportation, leaping, etc.
    /// </summary>
    public class RelocateAbility : ClashOfHeroesAbility
    {
        /// <summary>
        /// Indicates if a move action requires confirmation (double-tap).
        /// </summary>
        [SerializeField] private bool _withConfirmation;
        private bool _isConfirmed;
        private ICell _confirmedTarget;
        /// <summary>
        /// Enables an optimized control scheme for touch devices, improving usability on mobile platforms. 
        /// Best used in combination with <see cref="_withConfirmation"/>.
        /// </summary>
        [SerializeField] private bool _useTouchOptimizedControls;
        private ICell _currentTarget;

        /// <summary>
        /// The selector responsible for determining valid destinations cells for the ability.
        /// </summary>
        [SerializeField] private TargetCellSelector _cellSelector;
        [SerializeField] private int _actionCost = 0;
        private HashSet<ICell> _cellsInRange;

        public override void OnAbilitySelected(IGridController gridController)
        {
            _cellsInRange = gridController.CellManager.GetCells().Where(c => _cellSelector.IsValidTarget(c, UnitReference)).ToHashSet();
        }

        public override void OnAbilityDeselected(IGridController gridController)
        {
            _cellsInRange = new HashSet<ICell>();
            _isConfirmed = false;
            _currentTarget = null;
        }

        public override void Display(IGridController gridController)
        {
            gridController.CellManager.MarkAsReachable(_cellsInRange);
        }

        public override void CleanUp(IGridController gridController)
        {
            gridController.CellManager.UnMark(_cellsInRange);
        }

        public override void OnCellClicked(ICell cell, IGridController gridController)
        {
            if (!_cellsInRange.Contains(cell) || Charges <= 0)
            {
                gridController.GridState = new GridStateAwaitInput();
                return;
            }

            if (_useTouchOptimizedControls)
            {
                if (_currentTarget != null)
                {
                    gridController.CellManager.MarkAsReachable(_currentTarget);
                }
                gridController.CellManager.MarkAsPath(new ICell[1] { cell }, UnitReference.CurrentCell);
                _currentTarget = cell;
            }

            if (_withConfirmation && (!_isConfirmed || !_confirmedTarget.Equals(cell)))
            {
                _isConfirmed = true;
                _confirmedTarget = cell;
                return;
            }

            HumanExecuteAbility(
                new RelocateCommand(
                    cell,
                    _actionCost,
                    async () => await (UnitReference as Unit)?.GetComponent<IRelocateHighlighter>()?.ApplyRelocateEffect(
                        new MoveHighlightParams(
                            UnitReference.CurrentCell,
                            cell,
                            Enumerable.Empty<ICell>()
                        )
                    )
                ),
                gridController
            );
        }

        public override void OnCellHighlighted(ICell cell, IGridController gridController)
        {
            if (_useTouchOptimizedControls) return;
            if (_cellsInRange.Contains(cell))
            {
                gridController.CellManager.MarkAsPath(new ICell[1] { cell }, UnitReference.CurrentCell);
            }
        }

        public override void OnCellDehighlighted(ICell cell, IGridController gridController)
        {
            if (_useTouchOptimizedControls)
            {
                if (cell.Equals(_currentTarget))
                {
                    gridController.CellManager.MarkAsPath(new ICell[1] { _currentTarget }, UnitReference.CurrentCell);
                }
                else if (_cellsInRange.Contains(cell))
                {
                    gridController.CellManager.MarkAsReachable(cell);
                }
            }
            else if (_cellsInRange.Contains(cell))
            {
                gridController.CellManager.MarkAsReachable(cell);
            }
        }

        public override void OnUnitClicked(IUnit unit, IGridController gridController)
        {
            if (gridController.UnitManager.GetFriendlyUnits(UnitReference.PlayerNumber).Contains(unit))
            {
                gridController.GridState = new GridStateUnitSelected(unit, unit.GetBaseAbilities());
            }
            else
            {
                gridController.GridState = new GridStateAwaitInput();
            }
        }
    }
}