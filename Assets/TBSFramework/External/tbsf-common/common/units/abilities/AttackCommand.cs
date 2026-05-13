using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Controllers;

namespace TurnBasedStrategyFramework.Common.Units.Abilities
{
    /// <summary>
    /// Represents a command to execute an attack action by a unit on a target unit.
    /// </summary>
    public readonly struct AttackCommand : ICommand
    {
        /// <summary>
        /// The target unit to be attacked.
        /// </summary>
        private readonly IUnit _target;

        /// <summary>
        /// The amount of damage to be inflicted on the target.
        /// </summary>
        private readonly float _damage;

        /// <summary>
        /// The cost in action points required to perform the attack.
        /// </summary>
        private readonly int _actionCost;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackCommand"/> struct.
        /// </summary>
        /// <param name="target">The unit to be attacked.</param>
        /// <param name="damage">The damage value to be inflicted on the target.</param>
        /// <param name="actionCost">The number of action points consumed by the attack.</param>
        public AttackCommand(IUnit target, float damage, int actionCost = 1)
        {
            _target = target;
            _damage = damage;
            _actionCost = actionCost;
        }

        /// <summary>
        /// Executes the attack command, reducing the target's health and consuming the attacker's action points.
        /// </summary>
        /// <param name="unit">The unit performing the attack.</param>
        /// <param name="controller">The grid controller responsible for managing the game state.</param>
        /// <returns>A task representing the asynchronous execution of the attack.</returns>
        public async Task Execute(IUnit unit, IGridController controller)
        {
            _target.ModifyHealth(-_damage, unit);
            _target.InvokeAttacked(new UnitAttackedEventArgs(_target, unit, _damage));
            unit.ActionPoints -= _actionCost;

            await Task.WhenAll(
                controller.UnitManager.MarkAsAttacking(unit, _target),
                controller.UnitManager.MarkAsDefending(_target, unit)
            );
        }

        /// <summary>
        /// Undoes the attack command, restoring the target's health and returning the action points to the attacker.
        /// </summary>
        /// <param name="unit">The unit that performed the attack.</param>
        /// <param name="controller">The grid controller responsible for managing the game state.</param>
        /// <returns>A task representing the asynchronous undo operation.</returns>
        public Task Undo(IUnit unit, IGridController controller)
        {
            _target?.ModifyHealth(+_damage, unit);
            unit.ActionPoints += _actionCost;

            return Task.CompletedTask;
        }

        private static class SerializationKeys
        {
            public const string TargetID = "target_id";
            public const string Damage = "damage";
            public const string ActionCost = "action_cost";
        }

        public Dictionary<string, object> Serialize()
        {
            return new Dictionary<string, object>
            {
                { SerializationKeys.TargetID, _target.UnitID },
                { SerializationKeys.Damage, _damage },
                { SerializationKeys.ActionCost, _actionCost }
            };
        }

        public ICommand Deserialize(Dictionary<string, object> actionParams, IGridController gridController)
        {
            var targetId = Convert.ToInt32(actionParams[SerializationKeys.TargetID]);
            var damage = Convert.ToSingle(actionParams[SerializationKeys.Damage]);
            var actionCost = Convert.ToInt32(actionParams[SerializationKeys.ActionCost]);

            var target = gridController.UnitManager.GetUnits()
                .First(u => u.UnitID == targetId);

            return new AttackCommand(target, damage, actionCost);
        }
    }
}
