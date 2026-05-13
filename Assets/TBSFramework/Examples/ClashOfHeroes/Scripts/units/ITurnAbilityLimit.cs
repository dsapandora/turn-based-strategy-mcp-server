namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes
{
    /// <summary>
    /// Interface for units that have a limited number of ability uses per turn.
    /// </summary>
    public interface ITurnAbilityLimit
    {
        public int GetMaxAbilityUsesPerTurn();
        public int AbilityUsePoints { get; set; }
    }
}