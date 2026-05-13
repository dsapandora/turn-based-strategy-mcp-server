using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Players;
using UnityEngine;


namespace TurnBasedStrategyFramework.Unity.Players
{
    /// <summary>
    /// Base class for Players, difining basic properties like player number and type.
    /// </summary>
    public abstract class Player : MonoBehaviour, IPlayer
    {
        [SerializeField] private int _playerNumber;
        public int PlayerNumber { get { return _playerNumber; } set { _playerNumber = value; } }

        public abstract PlayerType PlayerType { get; set; }

        public abstract void Initialize(GridController gridController);
        public abstract void Play(GridController gridController);
    }
}