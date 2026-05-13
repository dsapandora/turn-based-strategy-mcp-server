using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Units.Abilities;

namespace TurnBasedStrategyFramework.Unity.MobileDemo
{
    /// <summary>
    /// Command to relocate a unit to a specified position on the grid, bypassing pathfinding. 
    /// This is suitable for abilities like teleportation, leaping, or other direct placement mechanics.
    /// </summary>
    public readonly struct RelocateCommand : ICommand
    {
        private readonly ICell _destination;
        private readonly int _actionCost;

        private readonly Func<Task> _relocateHighlighter;


        public RelocateCommand(ICell destination, int actionCost, Func<Task> relocateHighlighter)
        {
            _destination = destination;
            _actionCost = actionCost;

            _relocateHighlighter = relocateHighlighter;
        }

        public async Task Execute(IUnit unit, IGridController controller)
        {
            var source = unit.CurrentCell;
            var sourceWorldPosition = unit.WorldPosition;
            await _relocateHighlighter();

            unit.CurrentCell.CurrentUnits.Remove(unit);
            unit.CurrentCell.IsTaken = false;

            unit.CurrentCell = _destination;
            _destination.CurrentUnits.Add(unit);
            _destination.IsTaken = true;

            unit.ActionPoints -= _actionCost;

            unit.WorldPosition = _destination.WorldPosition;
            unit.InvokeUnitLeftCell(new UnitChangedGridPositionEventArgs(unit, source, _destination));
            unit.InvokeUnitEnteredCell(new UnitChangedGridPositionEventArgs(unit, source, _destination));
            unit.InvokeUnitPositionChanged(new UnitPositionChangedEventArgs(unit, sourceWorldPosition, unit.WorldPosition));
            unit.InvokeUnitMoved(new UnitMovedEventArgs(unit, source, _destination, new List<ICell>() { source, _destination }));
            await Task.CompletedTask;
        }

        public Task Undo(IUnit unit, IGridController controller)
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<string, object> Serialize()
        {
            throw new System.NotImplementedException();
        }

        public ICommand Deserialize(Dictionary<string, object> actionParams, IGridController gridController)
        {
            throw new System.NotImplementedException();
        }
    }
}