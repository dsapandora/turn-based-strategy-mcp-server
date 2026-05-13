using System;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Controllers.GameResolvers;
using TurnBasedStrategyFramework.Common.Controllers.GridStates;
using TurnBasedStrategyFramework.Common.Controllers.TurnResolvers;
using TurnBasedStrategyFramework.Common.Players;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.Cells;
using TurnBasedStrategyFramework.Unity.Players;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Controllers
{
    /// <summary>
    /// A Unity-specific controller for managing the grid, units, players, and turns in the game.
    /// It integrates with the underlying <see cref="GridController"/> to handle game logic, 
    /// and manages events such as game start, turn transitions, and ability usage.
    /// </summary>
    public class UnityGridController : MonoBehaviour, IGridController
    {
        private readonly GridController _controller = new GridController();

        /// <summary>
        /// A flag indicating if the game should start immediatelly when the scene loads.
        /// </summary>
        [SerializeField] private bool _startImmediatelly = true;
        [SerializeField] private UnityCellManager _cellManager;
        [SerializeField] private UnityUnitManager _unitManager;
        [SerializeField] private UnityPlayerManager _playerManager;

        [SerializeField] private UnityTurnResolver _turnResolver;

        public ICellManager CellManager { get { return _controller.CellManager; } set { _controller.CellManager = value; _cellManager = value as UnityCellManager; } }
        public IUnitManager UnitManager { get { return _controller.UnitManager; } set { _controller.UnitManager = value; _unitManager = value as UnityUnitManager; } }
        public IPlayerManager PlayerManager { get { return _controller.PlayerManager; } set { _controller.PlayerManager = value; _playerManager = value as UnityPlayerManager; } }

        public ITurnResolver TurnResolver { get { return _controller.TurnResolver; } set { _controller.TurnResolver = value; _turnResolver = value as UnityTurnResolver; } }

        public TurnContext TurnContext { get { return _controller.TurnContext; } }
        public GridState GridState { get { return _controller.GridState; } set { _controller.GridState = value; } }

        /// <summary>
        /// Triggered when the game starts.
        /// </summary>
        public event Action GameStarted;

        /// <summary>
        /// Triggered when the game is initialized, meaning all relevant data is set up and ready.
        /// </summary>
        public event Action GameInitialized;

        /// <summary>
        /// Triggered when the game ends, providing the result of the game.
        /// </summary>
        public event Action<GameResult> GameEnded;

        /// <summary>
        /// Triggered when a new turn starts, providing the turn context.
        /// </summary>
        public event Action<TurnTransitionParams> TurnStarted;

        /// <summary>
        /// Triggered when the current turn ends, providing the turn context.
        /// </summary>
        public event Action<TurnTransitionParams> TurnEnded;

        private void Awake()
        {
            _controller.CellManager = _cellManager;
            _controller.UnitManager = _unitManager;
            _controller.PlayerManager = _playerManager;
            _controller.TurnResolver = _turnResolver;

            _controller.GameInitialized += OnGameInitialized;
            _controller.GameEnded += OnGameEnded;
            _controller.GameStarted += OnGameStared;
            _controller.TurnStarted += OnTurnStarted;
            _controller.TurnEnded += OnTurnEnded;
        }

        public virtual void InitializeGame(bool isNetworkInvoked = false)
        {
            _controller.InitializeGame(isNetworkInvoked);
        }

        public virtual void StartGame(bool isNetworkInvoked = false)
        {
            _controller.StartGame(isNetworkInvoked);
        }

        public virtual void InitializeAndStart(bool isNetworkInvoked = false)
        {
            _controller.InitializeAndStart(isNetworkInvoked);
        }

        public virtual void EndTurn(bool isNetworkInvoked = false)
        {
            _controller.EndTurn(isNetworkInvoked);
        }

        public virtual void MakeTurnTransition(bool isNetworkInvoked = false)
        {
            _controller.MakeTurnTransition(isNetworkInvoked);
        }

        public void InvokeGameEnded(GameResult gameResult)
        {
            _controller.InvokeGameEnded(gameResult);
        }

        /// <summary>
        /// Handles the game initialize event, invoking the <see cref="GameInitialized"/> event.
        /// </summary>
        private void OnGameInitialized()
        {
            GameInitialized?.Invoke();
        }

        /// <summary>
        /// Handles the game start event, invoking the <see cref="GameStarted"/> event.
        /// </summary>
        private void OnGameStared()
        {
            GameStarted?.Invoke();
        }

        /// <summary>
        /// Handles the game end event, invoking the <see cref="GameEnded"/> event with the result.
        /// </summary>
        private void OnGameEnded(GameResult result)
        {
            GameEnded?.Invoke(result);
        }

        /// <summary>
        /// Handles the turn start event, invoking the <see cref="TurnStarted"/> event with the current turn context.
        /// </summary>
        private void OnTurnStarted(TurnTransitionParams turnTransitionParams)
        {
            TurnStarted?.Invoke(turnTransitionParams);
        }

        /// <summary>
        /// Handles the turn end event, invoking the <see cref="TurnEnded"/> event with the current turn context.
        /// </summary>
        private void OnTurnEnded(TurnTransitionParams turnTransitionParams)
        {
            TurnEnded?.Invoke(turnTransitionParams);
        }

        private void Start()
        {
            if (_startImmediatelly)
            {
                _controller.InitializeGame();
                _controller.StartGame();
            }
        }
    }
}