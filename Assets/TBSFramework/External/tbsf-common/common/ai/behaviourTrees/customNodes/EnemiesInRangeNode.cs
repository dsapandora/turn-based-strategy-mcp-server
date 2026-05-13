using System.Linq;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.AI.BehaviourTrees
{
    /// <summary>
    /// Represents a node in a behavior tree that checks if there are any enemy units in range of the specified unit.
    /// </summary>
    public class EnemiesInRangeNode : ITreeNode
    {
        /// <summary>
        /// The unit for which the enemies in range will be checked.
        /// </summary>
        private readonly IUnit _unit;

        /// <summary>
        /// The grid controller responsible for managing the game state.
        /// </summary>
        private readonly IGridController _gridController;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemiesInRangeNode"/> class with the specified unit and grid controller.
        /// </summary>
        /// <param name="unit">The unit for which to check the enemies in range.</param>
        /// <param name="gridController">The grid controller to manage interactions.</param>
        public EnemiesInRangeNode(IUnit unit, IGridController gridController)
        {
            _unit = unit;
            _gridController = gridController;
        }

        /// <summary>
        /// Executes the enemies in range node by checking if there are any attackable enemy units in range of the specified unit.
        /// </summary>
        /// <returns>A task representing the execution, with a boolean result indicating whether any enemy units are in range.</returns>
        public Task<bool> Execute(bool debugMode)
        {
            var enemyUnits = _gridController.UnitManager.GetEnemyUnits(_unit.PlayerNumber);
            return Task.FromResult(enemyUnits.Any(u => _unit.IsUnitAttackable(u, u.CurrentCell, _unit.CurrentCell)));
        }
    }
}
