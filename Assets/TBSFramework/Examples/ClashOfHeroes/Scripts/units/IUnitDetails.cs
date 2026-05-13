using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units
{
    /// <summary>
    /// Interface defining display-related metadata for a unit,
    /// including name and portrait for use in UI panels.
    /// </summary>
    public interface IUnitDetails
    {
        public string UnitName { get; set; }
        public Sprite UnitPortrait { get; set; }
    }
}