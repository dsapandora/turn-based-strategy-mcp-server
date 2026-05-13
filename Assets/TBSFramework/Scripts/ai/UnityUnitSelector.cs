using System;
using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.AI;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.AI
{
    public abstract class UnityUnitSelector : MonoBehaviour, IUnitSelector
    {
        public abstract IEnumerable<IUnit> SelectNext(Func<IEnumerable<IUnit>> getUnits, GridController gridController);
    }
}