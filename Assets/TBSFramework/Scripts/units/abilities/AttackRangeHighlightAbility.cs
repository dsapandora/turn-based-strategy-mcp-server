using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units.Abilities;

namespace TurnBasedStrategyFramework.Unity.Units.Abilities
{
    /// <summary>
    /// A Unity-specific ability that highlights the attack range of a unit on the grid, forwarding method calls to common <see cref="AttackRangeHighlightAbilityImpl"/>
    /// </summary>
    public class AttackRangeHighlightAbility : Ability
    {
        private AttackRangeHighlightAbilityImpl _attackRangeHighlightAbility;

        public override void Initialize(IGridController gridController)
        {
            base.Initialize(gridController);
            _attackRangeHighlightAbility = new AttackRangeHighlightAbilityImpl(UnitReference);
            _attackRangeHighlightAbility.Initialize(gridController);
        }

        public override void OnAbilitySelected(IGridController gridController)
        {
            _attackRangeHighlightAbility.OnAbilitySelected(gridController);
        }

        public override void OnAbilityDeselected(IGridController gridController)
        {
            _attackRangeHighlightAbility.OnAbilityDeselected(gridController);
        }

        public override void OnCellHighlighted(ICell cell, IGridController gridController)
        {
            _attackRangeHighlightAbility.OnCellHighlighted(cell, gridController);
        }

        public override void OnCellDehighlighted(ICell cell, IGridController gridController)
        {
            _attackRangeHighlightAbility.OnCellDehighlighted(cell, gridController);
        }

        public override bool CanPerform(IGridController gridController)
        {
            return _attackRangeHighlightAbility.CanPerform(gridController);
        }
    }
}