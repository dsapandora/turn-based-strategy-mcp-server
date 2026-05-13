using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Units.Abilities;
using TurnBasedStrategyFramework.Unity.Units;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities
{
    public class ChargeCommand : ICommand
    {
        private readonly IEnumerable<ICell> _path;
        private readonly int _damage;
        private readonly ICell _source;
        private readonly ICell _destination;

        public ChargeCommand(ICell source, ICell destination, IEnumerable<ICell> path, int damage)
        {
            _path = path;
            _source = source;
            _destination = destination;
            _damage = damage;
        }

        public async Task Execute(IUnit unit, IGridController controller)
        {
            void handler(UnitChangedGridPositionEventArgs eventArgs) => OnUnitLeftCell(eventArgs, unit, controller.CellManager, _destination);
            unit.UnitLeftCell += handler;
            unit.CurrentCell.IsTaken = false;
            unit.CurrentCell.CurrentUnits.Remove(unit);

            await controller.UnitManager.MarkAsMoving(unit, _source, _destination, _path);
            await unit.MovementAnimation(_path, _destination);
            await controller.UnitManager.UnMarkAsMoving(unit, _source, _destination, _path);

            unit.InvokeUnitMoved(new UnitMovedEventArgs(unit, _source, _destination, _path));
            unit.UnitLeftCell -= handler;

            _destination.IsTaken = true;
            unit.CurrentCell = _destination;
            unit.CurrentCell.CurrentUnits.Add(unit);
        }

        private async void OnUnitLeftCell(UnitChangedGridPositionEventArgs obj, IUnit unit, ICellManager cellManager, ICell destination)
        {
            if (obj.EnteredCell.CurrentUnits.Any())
            {
                var collidedUnit = obj.EnteredCell.CurrentUnits.First();
                await (collidedUnit as Unit).GetComponent<IRammedHighlighter>().ApplyDamageEffect();

                var direction = destination.WorldPosition.Subtract(obj.EnteredCell.WorldPosition);
                var normalizedDirection = direction.Normalize();

                var nearestCell = cellManager.GetCells()
                    .Where(c => collidedUnit.IsCellMovableTo(c) && !_path.Contains(c))
                    .OrderBy(c => c.GetDistance(obj.EnteredCell))
                    .ThenBy(c =>
                    {
                        var toCell = c.WorldPosition.Subtract(obj.EnteredCell.WorldPosition);
                        var normalizedToCell = toCell.Normalize();
                        return normalizedDirection.Dot(normalizedToCell);
                    })
                    .FirstOrDefault();

                if (nearestCell != null)
                {
                    await (collidedUnit as Unit).GetComponent<IRammedHighlighter>().ApplyKnockbackEffect(new MoveHighlightParams(collidedUnit.CurrentCell, nearestCell, new ICell[1] { nearestCell }));

                    collidedUnit.CurrentCell.IsTaken = false;
                    collidedUnit.CurrentCell.CurrentUnits.Remove(collidedUnit);
                    collidedUnit.CurrentCell = nearestCell;
                    nearestCell.IsTaken = true;
                    nearestCell.CurrentUnits.Add(collidedUnit);
                }

                collidedUnit.ModifyHealth(-_damage, unit);
            }
        }

        public Task Undo(IUnit unit, IGridController controller)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, object> Serialize()
        {
            throw new NotImplementedException();
        }

        public ICommand Deserialize(Dictionary<string, object> actionParams, IGridController gridController)
        {
            throw new NotImplementedException();
        }

        class UnitLeftCellEventArgs : EventArgs
        {
            public UnitChangedGridPositionEventArgs GridPositionArgs { get; }
            public IUnit Unit { get; }
            public ICellManager CellManager { get; }
            public IUnitManager UnitManager { get; }
            public ICell Destination { get; }

            public UnitLeftCellEventArgs(UnitChangedGridPositionEventArgs gridPositionArgs, IUnit unit, ICellManager cellManager, IUnitManager unitManager, ICell destination)
            {
                GridPositionArgs = gridPositionArgs;
                Unit = unit;
                CellManager = cellManager;
                UnitManager = unitManager;
                Destination = destination;
            }
        }
    }
}