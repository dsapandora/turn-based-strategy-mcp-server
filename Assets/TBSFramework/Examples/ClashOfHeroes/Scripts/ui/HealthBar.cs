using System;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.UI
{
    /// <summary>
    /// Animates a health bar based on changes to a Unit's health.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Image _healthBar;
        [SerializeField] Unit _unitReference;
        [SerializeField] AnimationCurve _animationCurve;

        private float _currentScale = 1;
        [SerializeField] private float _animationDuration = 0.5f;
        private bool _isAnimating = false;

        private void Awake()
        {
            _unitReference.HealthChanged += OnHealthChanged;
        }

        private async void OnHealthChanged(TurnBasedStrategyFramework.Common.Units.HealthChangedEventArgs obj)
        {
            float targetScale = Math.Max(obj.AffectedUnit.Health, 0) / obj.AffectedUnit.MaxHealth;

            if (!_isAnimating)
            {
                await AnimateHealthBarAsync(targetScale);
            }
        }

        private async Task AnimateHealthBarAsync(float targetScale)
        {
            _isAnimating = true;
            float elapsedTime = 0f;
            float initialScale = _currentScale;

            while (elapsedTime < _animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / _animationDuration;
                float smoothedT = _animationCurve.Evaluate(t);

                _currentScale = Mathf.Lerp(initialScale, targetScale, smoothedT);
                _healthBar.transform.localScale = new Vector3(_currentScale, 1, 1);

                await Awaitable.NextFrameAsync();
            }

            _currentScale = targetScale;
            _healthBar.transform.localScale = new Vector3(_currentScale, 1, 1);

            _isAnimating = false;
        }
    }
}
