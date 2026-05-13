using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Units
{
    /// <summary>
    /// Represents a unit that has an associated unit type, defined by a <see cref="ScriptableObject"/>.
    /// </summary>
    public interface ITypedUnit
    {
        /// <summary>
        /// Gets the resource representing the unit's type.
        /// </summary>
        public ScriptableObject UnitType { get; }
    }
}
