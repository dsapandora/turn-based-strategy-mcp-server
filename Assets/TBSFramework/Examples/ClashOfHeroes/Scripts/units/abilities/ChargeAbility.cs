using System.Collections.Generic;
using System.Linq;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Controllers.GridStates;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Utilities;
using TurnBasedStrategyFramework.Unity.Cells;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities
{
    /// <summary>
    /// Represents a charging attack ability for Clash of Heroes units. 
    /// Allows directional movement across a line of valid cells and deals damage along the way.
    /// </summary>
    public class ChargeAbility : ClashOfHeroesAbility
    {
        [SerializeField] private int _range;
        [SerializeField] private int _damage;
        /// <summary>
        /// Indicates if a charge action requires confirmation (double-tap).
        /// </summary>
        [SerializeField] private bool _withConfirmation;
        /// <summary>
        /// Enables an optimized control scheme for touch devices, improving usability on mobile platforms. 
        /// Best used in combination with <see cref="_withConfirmation"/>.
        /// </summary>
        [SerializeField] private bool _useTouchOptimizedControls;

        private IEnumerable<ICell> _cellsInRange { get; set; }
        private IEnumerable<ICell> _currentLine { get; set; }
        private bool _isConfirmed = false;
        private ICell _confirmedTarget;

        private HexGridType _gridType;

        public override void OnAbilitySelected(IGridController gridController)
        {
            _gridType = (gridController.CellManager.GetCells().First() as Hexagon).GridType;
            _currentLine = Enumerable.Empty<ICell>();
            _cellsInRange = gridController.CellManager.GetCells().Where(c =>
            {
                return !c.Equals(UnitReference.CurrentCell) && c.GetDistance(UnitReference.CurrentCell) <= _range &&
                       GridUtilities.GetLineOfCells(UnitReference.CurrentCell, c, gridController.CellManager, _gridType) is var line &&
                       line.Zip(line.Skip(1), (a, b) => (UnitReference as Unit).GetComponent<IChargeMovement>().IsCellChargeableToFrom(a, b)).All(result => result);
            });
        }

        public override void Display(IGridController gridController)
        {
            gridController.CellManager.MarkAsReachable(_cellsInRange);
        }

        public override void CleanUp(IGridController gridController)
        {
            gridController.CellManager.UnMark(_cellsInRange);
            gridController.CellManager.UnMark(_currentLine);
            _cellsInRange = Enumerable.Empty<ICell>();
            _currentLine = Enumerable.Empty<ICell>();
            _isConfirmed = false;
            _confirmedTarget = null;
        }

        public override void OnCellClicked(ICell cell, IGridController gridController)
        {
            if (!_cellsInRange.Contains(cell))
            {
                gridController.GridState = new GridStateAwaitInput();
                return;
            }

            if (_useTouchOptimizedControls)
            {
                gridController.CellManager.MarkAsReachable(_currentLine);
                _currentLine = GridUtilities.GetLineOfCells(UnitReference.CurrentCell, cell, gridController.CellManager, _gridType);
                gridController.CellManager.MarkAsPath(_currentLine, UnitReference.CurrentCell);
            }

            if (_withConfirmation && (!_isConfirmed || !_confirmedTarget.Equals(cell)))
            {
                _isConfirmed = true;
                _confirmedTarget = cell;
                return;
            }

            HumanExecuteAbility(new ChargeCommand(UnitReference.CurrentCell, cell, _currentLine, _damage), gridController);
        }

        public override void OnUnitClicked(IUnit unit, IGridController gridController)
        {
            OnCellClicked(unit.CurrentCell, gridController);
        }

        public override void OnCellHighlighted(ICell cell, IGridController gridController)
        {
            if (_useTouchOptimizedControls) return;
            if (_cellsInRange.Contains(cell))
            {
                _currentLine = GridUtilities.GetLineOfCells(UnitReference.CurrentCell, cell, gridController.CellManager, _gridType);
                gridController.CellManager.MarkAsPath(_currentLine, UnitReference.CurrentCell);
            }
        }

        public override void OnCellDehighlighted(ICell cell, IGridController gridController)
        {
            if (_useTouchOptimizedControls)
            {
                if (_currentLine.Contains(cell))
                {
                    gridController.CellManager.MarkAsPath(_currentLine, UnitReference.CurrentCell);
                }
                else if (_cellsInRange.Contains(cell))
                {
                    gridController.CellManager.MarkAsReachable(cell);
                }
            }
            else if (_currentLine != null)
            {
                gridController.CellManager.UnMark(_currentLine);
                gridController.CellManager.MarkAsReachable(_currentLine);
            }
        }
    }
}