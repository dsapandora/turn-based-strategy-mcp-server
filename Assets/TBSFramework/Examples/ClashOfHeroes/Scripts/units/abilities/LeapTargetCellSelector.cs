using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Cells;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities
{
    /// <summary>
    /// Target cell selector for leap abilities, restricted by range and height difference.
    /// </summary>
    public class LeapTargetCellSelector : TargetCellSelector
    {
        [SerializeField] private int _range;
        [SerializeField] private int _maxHeightDifference;

        public override bool IsValidTarget(ICell cell, IUnit unit)
        {
            return cell.GetDistance(unit.CurrentCell) <= _range
                    && unit.IsCellMovableTo(cell)
                    && Mathf.Abs((cell as IHeightComponent).Height - (unit.CurrentCell as IHeightComponent).Height) <= _maxHeightDifference;
        }
    }
}