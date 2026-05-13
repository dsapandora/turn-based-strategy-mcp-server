using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Utilities
{
    /// <summary>
    /// This script rotates any GameObject around a specified axis at a configurable speed.
    /// Useful for objects like fans, rotors, wheels, or other spinning elements.
    /// </summary>
    public class Rotator : MonoBehaviour
    {
        /// <summary>
        /// The axis of rotation in local space. Example: (0, 0, 1) rotates around the Z-axis.
        /// </summary>
        [SerializeField] private Vector3 rotationAxis = new Vector3(0f, 0f, 1f);

        /// <summary>
        /// Rotation speed in degrees per second.
        /// </summary>
        [SerializeField] private float rotationSpeed = 100f;

        /// <summary>
        /// Whether the rotation is currently enabled. Allows you to pause rotation.
        /// </summary>
        [SerializeField] private bool isRotationEnabled = true;

        /// <summary>
        /// Rotates the GameObject each frame if rotation is enabled.
        /// </summary>
        void Update()
        {
            if (isRotationEnabled)
            {
                transform.Rotate(rotationSpeed * Time.deltaTime * rotationAxis);
            }
        }

        /// <summary>
        /// Sets the rotation speed dynamically.
        /// </summary>
        /// <param name="newSpeed">The new rotation speed in degrees per second.</param>
        public void SetRotationSpeed(float newSpeed)
        {
            rotationSpeed = newSpeed;
        }

        /// <summary>
        /// Enables or disables the rotation.
        /// </summary>
        /// <param name="enable">True to enable rotation, false to disable.</param>
        public void EnableRotation(bool enable)
        {
            isRotationEnabled = enable;
        }

        /// <summary>
        /// Changes the axis of rotation dynamically.
        /// </summary>
        /// <param name="newAxis">The new axis of rotation as a Vector3.</param>
        public void SetRotationAxis(Vector3 newAxis)
        {
            rotationAxis = newAxis;
        }
    }
}