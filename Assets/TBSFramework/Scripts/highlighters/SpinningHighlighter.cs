using System.Threading.Tasks;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// Highlighter that animates the rotation of a Transform with rotation speed controlled by an animation curve.
    /// </summary>
    public class SpinningHighlighter : Highlighter
    {
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private AnimationCurve _rotationSpeedCurve;
        [SerializeField] private Vector3 _rotationAxis = Vector3.up;
        [SerializeField] private float _rotationSpeed = 1f;

        /// <summary>
        /// Applies the rotation animation to the target Transform based on the animation curve.
        /// </summary>
        public override async Task Apply(IHighlightParams @params)
        {
            float elapsedTime = 0f;
            while (elapsedTime < _duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);
                float _speedMultiplier = _rotationSpeedCurve.Evaluate(t);

                _targetTransform.Rotate(_rotationAxis, _speedMultiplier * _rotationSpeed * Time.deltaTime, Space.Self);

                await Awaitable.NextFrameAsync();
            }
        }
    }
}
