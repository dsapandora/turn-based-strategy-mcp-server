using System.Threading.Tasks;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// A highlighter that smoothly scales a transform.
    /// </summary>
    public class ScalingHighlighter : Highlighter
    {
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private AnimationCurve _scaleCurve;

        public override async Task Apply(IHighlightParams @params)
        {
            Vector3 originalScale = Vector3.one;

            float elapsedTime = 0f;
            while (elapsedTime < _duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);
                float scaleMultiplier = _scaleCurve.Evaluate(t);
                _targetTransform.localScale = originalScale * scaleMultiplier;

                await Awaitable.NextFrameAsync();
            }

            _targetTransform.localScale = originalScale * _scaleCurve.Evaluate(1f);
        }
    }
}
