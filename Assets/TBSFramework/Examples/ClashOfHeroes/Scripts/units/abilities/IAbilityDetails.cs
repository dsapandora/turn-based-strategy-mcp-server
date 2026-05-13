using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities
{
    /// <summary>
    /// Interface defining details for a unit ability, including name, description, image, and charges.
    /// </summary>
    public interface IAbilityDetails
    {
        public string AbilityName { get; set; }
        public string AbilityDescription { get; set; }
        public Sprite AbilityImage { get; set; }

        public bool HasLimitedCharges { get; set; }
        public int MaxCharges { get; set; }
        public int Charges { get; set; }
    }
}