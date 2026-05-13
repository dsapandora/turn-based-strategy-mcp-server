using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Units.Abilities;

namespace TurnBasedStrategyFramework.Unity.Units.Abilities
{
    /// <summary>
    /// A Unity-specific implementation of an attack ability, forwarding method calls to common <see cref="AttackAbilityImpl"/>.
    /// </summary>
    public class AttackAbility : Ability
    {
        private AttackAbilityImpl _attackAbility;
        public override void Initialize(IGridController gridController)
        {
            base.Initialize(gridController);
            _attackAbility = new AttackAbilityImpl(UnitReference);
            _attackAbility.Initialize(gridController);
        }

        public override void Display(IGridController gridController)
        {
            _attackAbility.Display(gridController);
        }

        public override void CleanUp(IGridController gridController)
        {
            _attackAbility.CleanUp(gridController);
        }

        public override void OnAbilitySelected(IGridController gridController)
        {
            _attackAbility.OnAbilitySelected(gridController);
        }

        public override void OnAbilityDeselected(IGridController gridController)
        {
            _attackAbility.OnAbilityDeselected(gridController);
        }

        public override void OnTurnStart(IGridController gridController)
        {
            _attackAbility.OnTurnStart(gridController);
        }

        public override void OnTurnEnd(IGridController gridController)
        {
            _attackAbility.OnTurnEnd(gridController);
        }

        public override void OnCellClicked(ICell cell, IGridController gridController)
        {
            _attackAbility.OnCellClicked(cell, gridController);
        }

        public override void OnCellHighlighted(ICell cell, IGridController gridController)
        {
            _attackAbility.OnCellHighlighted(cell, gridController);
        }

        public override void OnCellDehighlighted(ICell cell, IGridController gridController)
        {
            _attackAbility.OnCellDehighlighted(cell, gridController);
        }
        public override void OnUnitClicked(IUnit unit, IGridController gridController)
        {
            _attackAbility.OnUnitClicked(unit, gridController);
        }

        public override void OnUnitHighlighted(IUnit unit, IGridController gridController)
        {
            _attackAbility.OnUnitHighlighted(unit, gridController);
        }

        public override void OnUnitDehighlighted(IUnit unit, IGridController gridController)
        {
            _attackAbility.OnUnitDehighlighted(unit, gridController);
        }

        public override void OnUnitDestroyed(IGridController gridController)
        {
            _attackAbility.OnUnitDestroyed(gridController);
        }

        public override bool CanPerform(IGridController gridController)
        {
            return _attackAbility.CanPerform(gridController);
        }
    }
}