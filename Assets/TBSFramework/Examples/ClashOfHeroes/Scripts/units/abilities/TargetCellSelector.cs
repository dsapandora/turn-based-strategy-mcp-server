using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities
{
    /// <summary>
    /// Abstract base class for selecting valid target cells for an ability.
    /// </summary>
    public abstract class TargetCellSelector : MonoBehaviour
    {
        /// <summary>
        /// Determines if the specified cell is valid for the given unit's ability. 
        /// </summary>
        /// <param name="cell">The cell to evaluate.</param>
        /// <param name="unit">The evaluating unit.</param>
        /// <returns>True if the cell meets the criteria for validity, otherwise false.</returns>
        public abstract bool IsValidTarget(ICell cell, IUnit unit);
    }
}