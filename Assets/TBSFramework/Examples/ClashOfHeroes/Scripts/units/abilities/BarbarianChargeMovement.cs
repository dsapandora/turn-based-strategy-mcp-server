using System;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Unity.Cells;
using TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Cells;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities
{
    /// <summary>
    /// Implements <see cref="IChargeMovement"/> for the Barbarian unit.
    /// Disallows charging onto water tiles and enforces a max 1-height difference restriction.
    /// </summary>
    public class BarbarianChargeMovement : MonoBehaviour, IChargeMovement
    {
        [SerializeField] private ScriptableObject _waterCellType;

        public bool IsCellChargeableToFrom(ICell source, ICell destination)
        {
            var destCell = destination as Cell;
            if (destCell.GetComponent<ITypedCell>().CellType.Equals(_waterCellType))
            {
                return false;
            }

            int sourceHeight = (source as Cell).GetComponent<IHeightComponent>().Height;
            int destinationHeight = destCell.GetComponent<IHeightComponent>().Height;

            return Math.Abs(sourceHeight - destinationHeight) <= 1;
        }
    }
}