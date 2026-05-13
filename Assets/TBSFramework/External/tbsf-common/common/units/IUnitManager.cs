using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Players;

namespace TurnBasedStrategyFramework.Common.Units
{
    /// <summary>
    /// Interface for managing units in the game. 
    /// Provides methods for retrieving, adding, and managing units, 
    /// as well as handling their visual states like selection, friendliness, and attack status.
    /// </summary>
    public interface IUnitManager
    {
        /// <summary>
        /// Event triggered when a unit is added to the manager, typically when the game starts or when a unit is spawned on runtime.
        /// </summary>
        event Action<IUnit> UnitAdded;

        /// <summary>
        /// Event triggered when a unit is removed from the manager, typically when the unit is destroyed during the game, or removed by calling <see cref="TurnBasedStrategyFramework.Common.Units.IUnit.RemoveFromGame"/>
        /// </summary>
        event Action<IUnit> UnitRemoved;

        /// <summary>
        /// Initializes the UnitManager when the game start.
        /// </summary>
        void Initialize(IGridController gridController);

        /// <summary>
        /// Retrieves all units currently managed by the unit manager.
        /// </summary>
        /// <returns>An enumerable collection of all units.</returns>
        IEnumerable<IUnit> GetUnits();

        /// <summary>
        /// Retrieves the friendly units for the specified player.
        /// </summary>
        /// <param name="player">The player whose friendly units are to be retrieved.</param>
        /// <returns>An enumerable collection of the player's friendly units.</returns>
        IEnumerable<IUnit> GetFriendlyUnits(IPlayer player);

        /// <summary>
        /// Retrieves the friendly units for the specified player number.
        /// </summary>
        /// <param name="playerNumber">The player number whose friendly units are to be retrieved.</param>
        /// <returns>An enumerable collection of the player's friendly units.</returns>
        IEnumerable<IUnit> GetFriendlyUnits(int playerNumber);

        /// <summary>
        /// Retrieves the enemy units for the specified player.
        /// </summary>
        /// <param name="player">The player whose enemy units are to be retrieved.</param>
        /// <returns>An enumerable collection of the enemy units.</returns>
        IEnumerable<IUnit> GetEnemyUnits(IPlayer player);

        /// <summary>
        /// Retrieves the enemy units for the specified player number.
        /// </summary>
        /// <param name="playerNumber">The player number whose enemy units are to be retrieved.</param>
        /// <returns>An enumerable collection of the enemy units.</returns>
        IEnumerable<IUnit> GetEnemyUnits(int playerNumber);

        /// <summary>
        /// Adds a unit to the game.
        /// </summary>
        /// <param name="unit">The unit to be added.</param>
        void AddUnit(IUnit unit);

        /// <summary>
        /// Removes a unit from the game.
        /// </summary>
        /// <param name="unit">The unit to be removed</param>
        void RemoveUnit(IUnit unit);

        /// <summary>
        /// Removes the visual highlight from the specified units.
        /// </summary>
        /// <param name="units">The units to unmark.</param>
        /// <returns>A task representing the asynchronous unmarking operation.</returns>
        Task UnMark(IEnumerable<IUnit> units);

        /// <summary>
        /// Marks the specified unit as selected.
        /// </summary>
        /// <param name="units">The unit to be marked as selected.</param>
        /// <returns>A task representing the asynchronous marking operation.</returns>
        Task MarkAsSelected(IUnit unit);

        /// <summary>
        /// Marks the specified units as friendly.
        /// </summary>
        /// <param name="units">The units to be marked as friendly.</param>
        /// <returns>A task representing the asynchronous marking operation.</returns>
        Task MarkAsFriendly(IEnumerable<IUnit> units);

        /// <summary>
        /// Marks the specified units as finished, indicating that they have completed their actions for the turn.
        /// </summary>
        /// <param name="units">The units to be marked as finished.</param>
        /// <returns>A task representing the asynchronous marking operation.</returns>
        Task MarkAsFinished(IEnumerable<IUnit> units);

        /// <summary>
        /// Marks the specified units as targetable, indicating that they can be targeted for actions such as attacks.
        /// </summary>
        /// <param name="units">The units to be marked as targetable.</param>
        /// <returns>A task representing the asynchronous marking operation.</returns>
        Task MarkAsTargetable(IEnumerable<IUnit> units);

        /// <summary>
        /// Marks the specified unit as attacking.
        /// </summary>
        /// <param name="units">The unit initiating the attack.</param>
        /// <param name="target">The unit being attacked.</param>
        /// <returns>A task representing the asynchronous marking operation.</returns>
        Task MarkAsAttacking(IUnit unit, IUnit target);

        /// <summary>
        /// Marks the specified units as defending.
        /// </summary>
        /// <param name="units">The unit being attacked</param>
        /// <param name="target">The unit initiating the attack</param>
        /// <returns>A task representing the asynchronous marking operation.</returns>
        Task MarkAsDefending(IUnit unit, IUnit aggressor);

        /// <summary>
        /// Marks the specified units as moving.
        /// </summary>
        /// <param name="unit">The unit that is moving.</param>
        /// <param name="source">The starting cell of the movement.</param>
        /// <param name="destination">The destination cell of the movement.</param>
        /// <param name="path">The sequence of cells representing the movement path.</param>
        /// <returns>A task representing the asynchronous marking operation.</returns>
        Task MarkAsMoving(IUnit unit, ICell source, ICell destination, IEnumerable<ICell> path);

        /// <summary>
        /// Removes the movement highlight from specific units.
        /// </summary>
        /// <param name="unit">The unit that is moving.</param>
        /// <param name="source">The starting cell of the movement.</param>
        /// <param name="destination">The destination cell of the movement.</param>
        /// <param name="path">The sequence of cells representing the movement path.</param>
        /// <returns>A task representing the asynchronous marking operation.</returns>
        Task UnMarkAsMoving(IUnit unit, ICell source, ICell destination, IEnumerable<ICell> path);

        /// <summary>
        /// Marks the specified units as destroyed.
        /// </summary>
        /// <param name="units">The unit being destroyed</param>
        /// <returns>A task representing the asynchronous marking operation.</returns>
        Task MarkAsDestroyed(IUnit unit);
    }
}
