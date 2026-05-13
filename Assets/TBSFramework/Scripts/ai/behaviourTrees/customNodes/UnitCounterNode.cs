using System;
using System.Linq;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.AI.BehaviourTrees;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.AI.BehaviourTrees
{
    /// <summary>
    /// A behavior tree node that counts units of a specific type owned by a player.
    /// It checks if the count meets or exceeds a specified threshold, which can be defined as a static value or a function.
    /// </summary>
    public readonly struct UnitCounterNode : ITreeNode
    {
        private readonly ScriptableObject _unitType;
        private readonly Func<int> _thresholdFn;
        private readonly IUnitManager _unitManager;

        private readonly int _playerNumber;

        public UnitCounterNode(ScriptableObject unitType, Func<int> thresholdFn, IUnitManager unitManager, int playerNumber)
        {
            _unitType = unitType;
            _thresholdFn = thresholdFn;
            _unitManager = unitManager;
            _playerNumber = playerNumber;
        }

        public UnitCounterNode(ScriptableObject unitType, int threshold, IUnitManager unitManager, int playerNumber) : this(unitType, () => threshold, unitManager, playerNumber)
        {
        }

        public Task<bool> Execute(bool debugMode)
        {
            var unitType = _unitType;
            var unitCount = _unitManager.GetFriendlyUnits(_playerNumber).Count(u => (u as ITypedUnit).UnitType.Equals(unitType));
            return Task.FromResult(unitCount >= _thresholdFn());
        }
    }
}