using System.Threading.Tasks;
using TurnBasedStrategyFramework.Unity.Units;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units
{
    /// <summary>
    /// Interface for applying relocate ability visual effects.
    /// </summary>
    public interface IRelocateHighlighter
    {
        Task ApplyRelocateEffect(MoveHighlightParams @params);
    }
}