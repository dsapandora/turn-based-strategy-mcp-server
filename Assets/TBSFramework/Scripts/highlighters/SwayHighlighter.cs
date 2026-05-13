using System.Threading.Tasks;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// A highlighter that applies a sway animation to a target.
    /// The sway effect moves the target based on the direction towards another transform, with the magnitude and duration controlled by an animation curve.
    /// </summary>
    public class SwayHighlighter : Highlighter
    {
        [SerializeField] private float _magnitude = 1f;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private AnimationCurve _swayCurve;
        [SerializeField] private Transform _targetTransform;

        public override async Task Apply(IHighlightParams @params)
        {
            var combatHighlightParams = (CombatHighlightParams)@params;

            Vector3 startingPosition = _targetTransform.localPosition;
            Vector3 heading = combatHighlightParams.SecondaryUnit.transform.localPosition - combatHighlightParams.PrimaryUnit.transform.localPosition;
            Vector3 direction = heading.normalized * _magnitude;

            float elapsedTime = 0f;

            while (elapsedTime < _duration)
            {
                float t = elapsedTime / _duration;
                float swayFactor = _swayCurve.Evaluate(t);
                _targetTransform.localPosition = startingPosition + direction * swayFactor;
                elapsedTime += Time.deltaTime;
                await Awaitable.NextFrameAsync();
            }

            _targetTransform.localPosition = startingPosition;
        }
    }
}