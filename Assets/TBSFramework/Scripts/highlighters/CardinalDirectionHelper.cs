using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// Helper class for resolving cardinal directions (e.g., Up, Down, Left, Right) to Unity's Vector3 directions.
    /// </summary>
    public static class CardinalDirectionHelper
    {
        public enum CardinalDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        public static Vector3 GetDirectionVector(CardinalDirection direction)
        {
            return direction switch
            {
                CardinalDirection.Up => Vector3.forward,
                CardinalDirection.Down => Vector3.back,
                CardinalDirection.Left => Vector3.left,
                CardinalDirection.Right => Vector3.right,
                _ => Vector3.forward,// Default to "up"
            };
        }
    }
}
