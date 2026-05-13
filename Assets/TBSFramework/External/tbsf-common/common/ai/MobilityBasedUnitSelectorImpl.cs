using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.AI
{
    /// <summary>
    /// A unit selector that prioritizes units based on the number of available neighboring cells for movement.
    /// Units with the highest mobility options (more free adjacent cells) are selected first.
    /// </summary>
    public class MobilityBasedUnitSelectorImpl : IUnitSelector
    {
        public IEnumerable<IUnit> SelectNext(Func<IEnumerable<IUnit>> getUnits, GridController gridController)
        {
            return getUnits().OrderByDescending(u => u.CurrentCell
                .GetNeighbours(gridController.CellManager)
                .Count(c => !c.IsTaken));
        }
    }
}