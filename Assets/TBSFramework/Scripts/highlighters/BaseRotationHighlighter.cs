using System.Threading.Tasks;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// Base class for highlighters that rotate a Transform towards a specific direction.
    /// Provides shared logic for rotating over time with configurable duration, animation curve, and axis restriction.
    /// </summary>
    public abstract class BaseRotationHighlighter : Highlighter
    {
        [SerializeField] protected float _duration = 0.5f;
        [SerializeField] protected AnimationCurve _animationCurve;
        [SerializeField] protected Transform _transform;

        /// <summary
        /// Delay to apply in milliseconds.
        /// </summary>
        [SerializeField] protected int _delay;

        /// <summary>
        /// Enum for restricting the rotation to a specific axis or allowing full rotation.
        /// </summary>
        protected enum RotationAxis
        {
            All,        // Rotate on all axes
            YOnly,      // Rotate only on the Y-axis
            XOnly,      // Rotate only on the X-axis
            ZOnly       // Rotate only on the Z-axis
        }

        [SerializeField] protected RotationAxis _rotationAxis = RotationAxis.All;

        /// <summary>
        /// Rotates the Transform towards the specified direction over the configured duration and using the animation curve.
        /// </summary>
        /// <param name="direction">The direction to rotate towards.</param>
        protected async Task RotateTowards(Vector3 direction)
        {
            Quaternion initialRotation = _transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            targetRotation = RestrictRotationToAxis(initialRotation, targetRotation);

            float elapsedTime = 0f;

            while (elapsedTime < _duration)
            {
                float t = elapsedTime / _duration;
                float curveValue = _animationCurve.Evaluate(t);
                Quaternion intermediateRotation = Quaternion.Slerp(initialRotation, targetRotation, curveValue);

                _transform.rotation = RestrictRotationToAxis(initialRotation, intermediateRotation);

                elapsedTime += Time.deltaTime;
                await Awaitable.NextFrameAsync();
            }

            _transform.rotation = RestrictRotationToAxis(initialRotation, targetRotation);
        }

        /// <summary>
        /// Restricts a rotation to the specified axis based on the configured RotationAxis setting.
        /// </summary>
        /// <param name="initialRotation">The initial rotation.</param>
        /// <param name="targetRotation">The target rotation.</param>
        /// <returns>A Quaternion representing the restricted rotation.</returns>
        protected Quaternion RestrictRotationToAxis(Quaternion initialRotation, Quaternion targetRotation)
        {
            Vector3 initialEuler = initialRotation.eulerAngles;
            Vector3 targetEuler = targetRotation.eulerAngles;

            switch (_rotationAxis)
            {
                case RotationAxis.YOnly:
                    return Quaternion.Euler(initialEuler.x, targetEuler.y, initialEuler.z);
                case RotationAxis.XOnly:
                    return Quaternion.Euler(targetEuler.x, initialEuler.y, initialEuler.z);
                case RotationAxis.ZOnly:
                    return Quaternion.Euler(initialEuler.x, initialEuler.y, targetEuler.z);
                default:
                    return targetRotation;
            }
        }
    }
}
