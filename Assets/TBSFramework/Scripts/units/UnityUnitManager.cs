using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Players;
using TurnBasedStrategyFramework.Common.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Units
{
    /// <summary>
    /// A Unity-specific implementation of <see cref="IUnitManager"/> responsible for managing units within the game.
    /// Loads units that are its children in the scene. 
    /// </summary>
    public class UnityUnitManager : MonoBehaviour, IUnitManager
    {
        private IList<IUnit> _units;
        private int _unitCount;

        public event Action<IUnit> UnitAdded;
        public event Action<IUnit> UnitRemoved;

        public void AddUnit(IUnit unit)
        {
            unit.UnitID = _unitCount++;
            _units.Add(unit);
            UnitAdded?.Invoke(unit);
        }
        public void RemoveUnit(IUnit unit)
        {
            _units.Remove(unit);
            UnitRemoved?.Invoke(unit);
        }

        public void Initialize(IGridController gridController)
        {
            _units = new List<IUnit>();
            foreach (var unit in GetComponentsInChildren<IUnit>()
                .OrderBy(u => u.CurrentCell == null)
                .ThenBy(u => u.CurrentCell?.GridCoordinates.x)
                .ThenBy(u => u.CurrentCell?.GridCoordinates.y))
            {
                AddUnit(unit);
            }
        }

        public IEnumerable<IUnit> GetUnits()
        {
            return _units;
        }
        public IEnumerable<IUnit> GetFriendlyUnits(IPlayer player)
        {
            return GetFriendlyUnits(player.PlayerNumber);
        }
        public IEnumerable<IUnit> GetFriendlyUnits(int playerNumber)
        {
            return _units.Where(u => u.PlayerNumber == playerNumber);
        }
        public IEnumerable<IUnit> GetEnemyUnits(IPlayer player)
        {
            return GetEnemyUnits(player.PlayerNumber);
        }
        public IEnumerable<IUnit> GetEnemyUnits(int playerNumber)
        {
            return _units.Where(u => u.PlayerNumber != playerNumber);
        }

        public async Task UnMark(IEnumerable<IUnit> units)
        {
            await Task.WhenAll(units.Select(u => (u as Unit).UnMark()));
        }
        public async Task MarkAsSelected(IUnit unit)
        {
            await (unit as Unit).MarkAsSelected();
        }
        public async Task MarkAsFriendly(IEnumerable<IUnit> units)
        {
            await Task.WhenAll(units.Select(u => (u as Unit).MarkAsFriendly()));
        }
        public async Task MarkAsFinished(IEnumerable<IUnit> units)
        {
            await Task.WhenAll(units.Select(u => (u as Unit).MarkAsFinished()));
        }
        public async Task MarkAsTargetable(IEnumerable<IUnit> units)
        {
            await Task.WhenAll(units.Select(u => (u as Unit).MarkAsTargetable()));
        }
        public async Task MarkAsAttacking(IUnit unit, IUnit target)
        {
            var targetUnit = target as Unit;
            await (unit as Unit).MarkAsAttacking(targetUnit);
        }
        public async Task MarkAsDefending(IUnit unit, IUnit aggressor)
        {
            var aggressorUnit = aggressor as Unit;
            await (unit as Unit).MarkAsDefending(aggressorUnit);
        }

        public async Task MarkAsMoving(IUnit unit, ICell source, ICell destination, IEnumerable<ICell> path)
        {
            await (unit as Unit).MarkAsMoving(source, destination, path);
        }

        public async Task UnMarkAsMoving(IUnit unit, ICell source, ICell destination, IEnumerable<ICell> path)
        {
            await (unit as Unit).UnMarkAsMoving(source, destination, path);
        }

        public async Task MarkAsDestroyed(IUnit unit)
        {
            await (unit as Unit).MarkAsDestroyed();
        }
    }
}