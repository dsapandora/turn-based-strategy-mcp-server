using System.Threading;
using System.Threading.Tasks;
using TMPro;
using TurnBasedStrategyFramework.Unity.Controllers;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.UI
{
    /// <summary>
    /// Handles UI transitions between turns by animating a panel with a message.
    /// Listens for turn start events and displays a movement animation for the transition message.
    /// </summary>
    public class TurnTransitionUI : MonoBehaviour
    {
        [SerializeField] private UnityGridController _gridController;
        [SerializeField] private GameObject _turnTransitionPanel;
        [SerializeField] private TMP_Text _turnTransitionText;

        [SerializeField] private float _animationTime;
        [SerializeField] private AnimationCurve _animationCurve;

        [SerializeField] private Vector2 _startPosition;
        [SerializeField] private Vector2 _finishPosition;

        private CancellationTokenSource _animationCancellationTokenSource;

        private void Awake()
        {
            _gridController.TurnStarted += OnTurnStarted;
            _gridController.GameEnded += (_) => { _gridController.TurnStarted -= OnTurnStarted; };
        }

        private async void OnTurnStarted(Common.Controllers.TurnTransitionParams obj)
        {
            _animationCancellationTokenSource?.Cancel();
            CancellationToken animationToken = new CancellationTokenSource().Token;
            _animationCancellationTokenSource = new CancellationTokenSource();

            _turnTransitionText.text = obj.TurnContext.CurrentPlayer.PlayerNumber == 0 ? "your turn" : "enemy turn";

            _turnTransitionPanel.SetActive(true);
            await AnimateTurnTransitionAsync(animationToken);

            if (!animationToken.IsCancellationRequested)
            {
                _turnTransitionPanel.SetActive(false);
            }
        }

        private async Task AnimateTurnTransitionAsync(CancellationToken cancellationToken)
        {
            RectTransform panelRect = _turnTransitionPanel.GetComponent<RectTransform>();
            float elapsedTime = 0f;

            while (elapsedTime < _animationTime && !cancellationToken.IsCancellationRequested)
            {
                float t = elapsedTime / _animationTime;
                float curveValue = _animationCurve.Evaluate(t);
                panelRect.anchoredPosition = Vector2.Lerp(_startPosition, _finishPosition, curveValue);

                await Awaitable.NextFrameAsync();
                elapsedTime += Time.deltaTime;
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                panelRect.anchoredPosition = _finishPosition;
            }
        }
    }
}