using System.Collections.Generic;
using System.Linq;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Players;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Players
{
    /// <summary>
    /// A Unity-specific player manager responsible for loading and managing players in the game.
    /// It loads player instances from its children in the scene.
    /// </summary>
    public class UnityPlayerManager : MonoBehaviour, IPlayerManager
    {
        private IList<IPlayer> _players;

        public void Initialize(GridController gridController)
        {
            _players = GetComponentsInChildren<IPlayer>().ToList();
        }

        public IEnumerable<IPlayer> GetPlayers()
        {
            return _players;
        }
        public IPlayer GetPlayerByNumber(int playerNumber)
        {
            return GetComponentsInChildren<IPlayer>().FirstOrDefault(p => p.PlayerNumber == playerNumber);
        }
    }
}