using System.Threading.Tasks;
using TurnBasedStrategyFramework.Unity.Units;
using TurnBasedStrategyFramework.Unity.Utilities;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// A highlighter that moves a target transform along a curved path from a start position to a destination.
    /// </summary>
    public class CurveMovementHighlighter : Highlighter
    {
        [SerializeField] private Transform _targetTransform;

        [SerializeField] private float _duration = 1f;
        [SerializeField] private AnimationCurve _horizontalCurve;
        [SerializeField] private AnimationCurve _verticalCurve;

        public override async Task Apply(IHighlightParams @params)
        {
            var moveHighlightParams = (MoveHighlightParams)@params;

            Vector3 startPos = moveHighlightParams.Source.WorldPosition.ToVector3();
            Vector3 endPos = moveHighlightParams.Destination.WorldPosition.ToVector3();

            float elapsedTime = 0f;

            while (elapsedTime < _duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);

                float horizontalT = _horizontalCurve.Evaluate(t);
                Vector3 horizontalPosition = Vector3.Lerp(startPos, endPos, horizontalT);

                float verticalOffset = _verticalCurve.Evaluate(t);
                float baselineY = Mathf.Lerp(startPos.y, endPos.y, horizontalT);
                horizontalPosition.y = baselineY + verticalOffset;

                _targetTransform.position = horizontalPosition;

                await Awaitable.NextFrameAsync();
            }

            _targetTransform.position = endPos;
        }
    }
}
