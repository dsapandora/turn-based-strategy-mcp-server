using TurnBasedStrategyFramework.Common.Utilities;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Utilities
{
    /// <summary>
    /// Extension methods for converting UnityEngine.Vector2Int to IVector2Int.
    /// </summary>
    public static class Vector2IntExtension
    {
        public static Vector2IntImpl ToIVector2Int(this Vector2Int vector)
        {
            return new Vector2IntImpl(vector.x, vector.y);
        }
    }
}
