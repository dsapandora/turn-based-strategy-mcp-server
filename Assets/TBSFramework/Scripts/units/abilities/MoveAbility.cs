using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Units.Abilities;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Units.Abilities
{
    /// <summary>
    /// A Unity-specific implementation of an move ability, forwarding method calls to common <see cref="MoveAbilityImpl"/>.
    /// </summary>
    public class MoveAbility : Ability
    {
        /// <summary>
        /// Indicates if a move action requires confirmation (double-tap).
        /// </summary>
        [SerializeField] private bool _withConfirmation;
        /// <summary>
        /// Enables an optimized control scheme for touch devices, improving usability on mobile platforms. 
        /// Best used in combination with <see cref="_withConfirmation"/>.
        /// </summary>
        [SerializeField] private bool _useTouchOptimizedControls;
        private MoveAbilityImpl _moveAbilityImpl;

        public override void Initialize(IGridController gridController)
        {
            base.Initialize(gridController);
            _moveAbilityImpl = new MoveAbilityImpl(UnitReference, _withConfirmation, _useTouchOptimizedControls);
            _moveAbilityImpl.Initialize(gridController);
        }

        public override void Display(IGridController gridController)
        {
            _moveAbilityImpl.Display(gridController);
        }

        public override void CleanUp(IGridController gridController)
        {
            _moveAbilityImpl.CleanUp(gridController);
        }

        public override void OnAbilitySelected(IGridController gridController)
        {
            _moveAbilityImpl.OnAbilitySelected(gridController);
        }

        public override void OnAbilityDeselected(IGridController gridController)
        {
            _moveAbilityImpl.OnAbilityDeselected(gridController);
        }

        public override void OnCellClicked(ICell cell, IGridController gridController)
        {
            _moveAbilityImpl.OnCellClicked(cell, gridController);
        }

        public override void OnCellHighlighted(ICell cell, IGridController gridController)
        {
            _moveAbilityImpl.OnCellHighlighted(cell, gridController);
        }

        public override void OnCellDehighlighted(ICell cell, IGridController gridController)
        {
            _moveAbilityImpl.OnCellDehighlighted(cell, gridController);
        }

        public override void OnUnitClicked(IUnit unit, IGridController gridController)
        {
            _moveAbilityImpl.OnUnitClicked(unit, gridController);
        }

        public override void OnUnitHighlighted(IUnit unit, IGridController gridController)
        {
            _moveAbilityImpl.OnUnitHighlighted(unit, gridController);
        }

        public override void OnUnitDehighlighted(IUnit unit, IGridController gridController)
        {
            _moveAbilityImpl.OnUnitDehighlighted(unit, gridController);
        }

        public override void OnUnitDestroyed(IGridController gridController)
        {
            _moveAbilityImpl.OnUnitDestroyed(gridController);
        }

        public override bool CanPerform(IGridController gridController)
        {
            return _moveAbilityImpl.CanPerform(gridController);
        }
    }
}