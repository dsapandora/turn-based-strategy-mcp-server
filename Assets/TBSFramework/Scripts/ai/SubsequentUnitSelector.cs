using System;
using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.AI;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Unity.AI
{
    /// <summary>
    /// A concrete implementation of <see cref="UnityUnitSelector"/> that uses the <see cref="SubsequentUnitSelectorImpl"/>
    /// to select units in sequence.
    /// </summary>
    public class SubsequentUnitSelector : UnityUnitSelector
    {
        private readonly SubsequentUnitSelectorImpl _unitSelector = new SubsequentUnitSelectorImpl();

        public override IEnumerable<IUnit> SelectNext(Func<IEnumerable<IUnit>> getUnits, GridController gridController)
        {
            return _unitSelector.SelectNext(getUnits, gridController);
        }
    }
}