using System.Threading.Tasks;
using TurnBasedStrategyFramework.Unity.Units;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units
{
    /// <summary>
    /// Interface for applying charge ability visual effects.
    /// </summary>
    public interface IRammedHighlighter
    {
        Task ApplyDamageEffect();
        Task ApplyKnockbackEffect(MoveHighlightParams @params);
    }
}