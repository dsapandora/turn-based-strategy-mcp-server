using System.Linq;
using TurnBasedStrategyFramework.Common.Controllers.GameResolvers;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.Players;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Controllers.GameResolvers
{
    /// <summary>
    /// A Unity-specific implementation that monitors the game's domination condition, 
    /// where the game ends when only one player has any units left alive.
    /// </summary>
    /// <remarks>
    /// This is the default game end condition in the Framework.
    /// </remarks>
    public class DominationVictoryCondition : MonoBehaviour
    {
        /// <summary>
        /// The manager responsible for handling units in the game.
        /// </summary>
        [SerializeField] private UnityUnitManager _unitManager;

        /// <summary>
        /// The manager responsible for handling players in the game.
        /// </summary>
        [SerializeField] private UnityPlayerManager _playerManager;

        /// <summary>
        /// The grid controller responsible for managing the game state.
        /// </summary>
        [SerializeField] private UnityGridController _gridController;

        /// <summary>
        /// Initializes the domination condition by subscribing to the unit removed event.
        /// </summary>
        public void Awake()
        {
            _unitManager.UnitRemoved += OnUnitRemoved;
        }

        /// <summary>
        /// Handles the unit removed event to check if only one player has any units alive. 
        /// If so, the game ends with that player as the winner.
        /// </summary>
        /// <param name="eventArgs">Event arguments containing details about the destroyed unit.</param>
        private void OnUnitRemoved(IUnit unit)
        {
            var playersWithUnitsAlive = _unitManager.GetUnits()
                                                    .Select(u => u.PlayerNumber)
                                                    .Distinct();
            if (playersWithUnitsAlive.Count() == 1)
            {
                var winner = _playerManager.GetPlayers()
                                           .First(p => p.PlayerNumber == playersWithUnitsAlive.First());
                var losers = _playerManager.GetPlayers()
                                           .Where(p => p != winner);

                _gridController.InvokeGameEnded(new GameResult(winner, losers));
            }
        }

        public void SetUnitManager(UnityUnitManager unitManager)
        {
            _unitManager = unitManager;
        }

        public void SetPlayerManager(UnityPlayerManager playerManager) 
        {
            _playerManager = playerManager;
        }

        public void SetGridController(UnityGridController gridController) 
        {
            _gridController = gridController;
        }
    }
}