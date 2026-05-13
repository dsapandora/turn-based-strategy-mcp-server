using System.Threading.Tasks;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units
{
    /// <summary>
    /// Interface for applying sweep ability visual effects.
    /// </summary>
    public interface ISweepHighlighter
    {
        Task ApplySweepEffect(GameObject target, CombatHighlightParams @params);
    }
}