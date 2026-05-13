using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.AI;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Controllers.GameResolvers;
using TurnBasedStrategyFramework.Common.Players;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.AI;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;

namespace TurnBasedStrategyFramework.Unity.Players
{
    /// <summary>
    /// A Unity-specific implementation of an AI-controlled player. 
    /// The AIPlayer selects and commands units during its turn using behavior trees for decision making.
    /// </summary>
    public class AIPlayer : Player
    {
        /// <summary>
        /// Gets or sets a flag indicating whether debug mode is enabled.
        /// When enabled, the AI pauses for user input between unit actions.
        /// </summary>
        [SerializeField] private bool _debugMode;

        /// <summary>
        /// The delay in milliseconds before the AI begins executing actions at the start of its turn.
        /// </summary>
        [SerializeField] private int _turnStartDelay = 0;

        /// <summary>
        /// The delay in milliseconds between actions of subsequent AI-controlled units, simulating human-like decision-making.
        /// </summary>
        [SerializeField] private int _unitDelay = 250;

        /// <summary>
        /// The unit selector used by the AI to determine which unit to command next.
        /// </summary>
        [SerializeField] private UnityUnitSelector _unitSelector;

        public override PlayerType PlayerType { get; set; } = PlayerType.AutomatedPlayer;

        /// <summary>
        /// A cancellation token source used to cancel AI actions if the game ends or the turn is over.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        public override void Initialize(GridController gridController)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            gridController.GameEnded += OnGameEnded;
            gridController.TurnEnded += OnTurnEnded;
        }

        private void OnTurnEnded(TurnTransitionParams turnTransitionParams)
        {
            CancelOngoingAction(); // Cancels any ongoing AI actions when the turn ends.
        }

        private void OnGameEnded(GameResult gameResult)
        {
            CancelOngoingAction(); // Cancels any ongoing AI actions when the game ends.
        }

        private void CancelOngoingAction()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Executes the AI player's turn by selecting and commanding units in sequence.
        /// </summary>
        /// <param name="gridController">The grid controller.</param>
        public override async void Play(GridController gridController)
        {
            await Awaitable.WaitForSecondsAsync(_turnStartDelay / 1000f);
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            var playableUnits = gridController.TurnContext.PlayableUnits().ToList();
            foreach (var playableUnit in _unitSelector.SelectNext(() => playableUnits, gridController))
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                await gridController.UnitManager.MarkAsSelected(playableUnit);
                playableUnit.InvokeUnitSelected();

                if (_debugMode)
                {
                    Debug.Log($"Current unit: {playableUnit}; Press {Key.N} to proceed to the next action");
                    await WaitForKeypress(Key.N);
                }

                await Awaitable.WaitForSecondsAsync(_unitDelay / 1000f, _cancellationTokenSource.Token);
                await playableUnit.BehaviourTree.Execute(_debugMode);

                await gridController.UnitManager.MarkAsFriendly(new IUnit[] { playableUnit });
                await gridController.UnitManager.MarkAsFinished(new IUnit[] { playableUnit });
                playableUnit.InvokeUnitDeselected();
            }

            gridController.EndTurn();
        }

        /// <summary>
        /// Waits for the user to press the specified key in debug mode before continuing.
        /// </summary>
        /// <param name="key">The key to wait for.</param>
        /// <returns>A task representing the wait operation.</returns>
        private async Task WaitForKeypress(Key key)
        {
            KeyControl keyControl = Keyboard.current[key];
            while (!keyControl.wasPressedThisFrame)
            {
                await Awaitable.NextFrameAsync();
            }
        }

        private void Reset()
        {
            if (GetComponent<IUnitSelector>() == null)
            {
                var unitSelector = gameObject.AddComponent<SubsequentUnitSelector>();
                _unitSelector = unitSelector;
            }
        }
    }
}
