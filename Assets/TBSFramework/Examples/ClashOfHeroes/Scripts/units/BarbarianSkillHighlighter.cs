using System.Collections.Generic;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Unity.Highlighters;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units
{
    /// <summary>
    /// Applies visual highlight effects for barbarian skills.
    /// </summary>
    public class BarbarianSkillHighlighter : MonoBehaviour, ISweepHighlighter
    {
        [SerializeField] private List<Highlighter> _sweepHighlighterFn;

        public async Task ApplySweepEffect(GameObject target, CombatHighlightParams @params)
        {
            foreach (var fn in _sweepHighlighterFn)
            {
                await fn.Apply(@params);
            }
        }
    }
}