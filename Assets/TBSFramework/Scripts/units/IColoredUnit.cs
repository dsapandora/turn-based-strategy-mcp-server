using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Units
{
    /// <summary>
    /// Represents a unit with a color assigned to it.
    /// </summary>
    public interface IColoredUnit
    {
        public Color Color { get; set; }
    }
}
