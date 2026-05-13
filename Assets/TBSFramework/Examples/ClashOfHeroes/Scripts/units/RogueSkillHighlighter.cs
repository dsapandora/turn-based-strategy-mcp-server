using System.Collections.Generic;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Unity.Highlighters;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units
{
    /// <summary>
    /// Applies visual highlight effects for rogue.
    /// </summary>
    public class RogueSkillHighlighter : MonoBehaviour, IRelocateHighlighter
    {
        [SerializeField] private List<Highlighter> _relocateHighlighterFn;

        public async Task ApplyRelocateEffect(MoveHighlightParams @params)
        {
            foreach (var fn in _relocateHighlighterFn)
            {
                await fn.Apply(@params);
            }
        }
    }
}