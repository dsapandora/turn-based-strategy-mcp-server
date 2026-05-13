using TurnBasedStrategyFramework.Common.Cells;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities
{
    /// <summary>
    /// Defines logic for determining whether a unit can charge from one cell to another.
    /// Used to constrain movement for charge ability.
    /// </summary>
    public interface IChargeMovement
    {
        public bool IsCellChargeableToFrom(ICell source, ICell destination);
    }
}