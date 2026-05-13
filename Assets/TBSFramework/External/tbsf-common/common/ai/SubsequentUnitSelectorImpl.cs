using System;
using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.AI
{
    /// <summary>
    /// Implements a basic unit selection strategy for selecting and ordering units for the AI to control.
    /// </summary>
    public class SubsequentUnitSelectorImpl : IUnitSelector
    {
        /// <summary>
        /// Selects and returns units sequentially based on the order provided by the given function.
        /// </summary>
        /// <param name="getUnits">A function that provides the available units for selection.</param>
        /// <param name="gridController">The grid controller.</param>
        /// <returns>An enumerable collection of units in the order they were provided.</returns>
        public IEnumerable<IUnit> SelectNext(Func<IEnumerable<IUnit>> getUnits, GridController gridController)
        {
            foreach (var unit in getUnits())
            {
                yield return unit;
            }
        }
    }
}
