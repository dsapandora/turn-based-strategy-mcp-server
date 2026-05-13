using System;
using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.AI
{
    /// <summary>
    /// Defines a contract for selecting and ordering units for the AI to control based on a specific selection strategy.
    /// The implementation determines the strategy for how the units are ordered.
    /// </summary>
    public interface IUnitSelector
    {
        /// <summary>
        /// Selects and orders units for the AI to control, based on a specified selection strategy.
        /// </summary>
        /// <param name="getUnits">A function that provides the available units for selection.</param>
        /// <param name="gridController">The grid controller.</param>
        /// <returns>An enumerable collection of units ordered by their selection priority</returns>
        IEnumerable<IUnit> SelectNext(Func<IEnumerable<IUnit>> getUnits, GridController gridController);
    }
}