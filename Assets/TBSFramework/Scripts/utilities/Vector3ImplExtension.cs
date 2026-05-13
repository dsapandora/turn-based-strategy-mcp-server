using TurnBasedStrategyFramework.Common.Utilities;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Utilities
{
    /// <summary>
    /// Extension methods for converting IVector3 to UnityEngine.Vector3.
    /// </summary>
    public static class Vector3ImplExtension
    {
        public static Vector3 ToVector3(this Vector3Impl vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }
    }
}