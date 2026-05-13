using System.Threading.Tasks;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// A highlighter that smoothly moves a target transform from its current position to a new position using linear interpolation.
    /// The new position is determined by a position delta (relative offset) from the current position.
    /// </summary>
    public class LerpToPositionHighlighter : Highlighter
    {
        [SerializeField] private Vector3 _positionDelta;
        [SerializeField] private float _duration;
        [SerializeField] private Transform _transform;

        public override async Task Apply(IHighlightParams @params)
        {
            Vector3 startPosition = _transform.position;
            Vector3 targetPosition = startPosition + _positionDelta;

            float elapsedTime = 0f;

            while (elapsedTime < _duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);
                _transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                await Awaitable.NextFrameAsync();
            }

            _transform.position = targetPosition;
        }
    }
}
