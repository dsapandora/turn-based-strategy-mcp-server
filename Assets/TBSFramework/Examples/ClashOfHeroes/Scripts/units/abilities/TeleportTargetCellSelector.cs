using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities
{
    /// <summary>
    /// Target cell selector for teleportation abilities, restricted by range.
    /// </summary>
    public class TeleportTargetCellSelector : TargetCellSelector
    {
        [SerializeField] private int _range;

        public override bool IsValidTarget(ICell cell, IUnit unit)
        {
            return cell.GetDistance(unit.CurrentCell) <= _range && unit.IsCellMovableTo(cell);
        }
    }
}