using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Controllers;

namespace TurnBasedStrategyFramework.Common.Units.Abilities
{
    /// <summary>
    /// Represents a command to execute an attack action by a unit on multiple target units.
    /// </summary>
    public readonly struct MultipleTargetAttackCommand : ICommand
    {
        /// <summary>
        /// Targets to be attacked.
        /// </summary>
        private readonly IEnumerable<IUnit> _targets;

        /// <summary>
        /// The amount of damage to be inflicted on the targets.
        /// </summary>
        private readonly float _damage;

        /// <summary>
        /// The cost in action points required to perform the attack.
        /// </summary>
        private readonly int _actionCost;

        /// <summary>
        /// A function that highlights the aggressor unit when performing the attack.
        /// </summary>
        private readonly Func<Task> _aggressorHighlighter;

        /// <summary>
        /// A function that highlights a defender unit when it is attacked.
        /// </summary>
        private readonly Func<IUnit, Task> _defenderHighlighter;

        public MultipleTargetAttackCommand(IEnumerable<IUnit> targets, float damage, int actionCost, Func<Task> aggressorHighlighter, Func<IUnit, Task> defenderHighlighter)
        {
            _targets = targets;
            _damage = damage;
            _actionCost = actionCost;

            _aggressorHighlighter = aggressorHighlighter;
            _defenderHighlighter = defenderHighlighter;
        }

        public async Task Execute(IUnit unit, IGridController controller)
        {
            _ = _aggressorHighlighter.Invoke();

            foreach (var target in _targets)
            {
                target.ModifyHealth(-_damage, unit);
                target.InvokeAttacked(new UnitAttackedEventArgs(target, unit, _damage));
                await _defenderHighlighter.Invoke(target);
            }

            unit.ActionPoints -= _actionCost;
            await Task.CompletedTask;
        }

        public Task Undo(IUnit unit, IGridController controller)
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<string, object> Serialize()
        {
            throw new NotImplementedException();
        }

        public ICommand Deserialize(Dictionary<string, object> actionParams, IGridController gridController)
        {
            throw new NotImplementedException();
        }
    }
}