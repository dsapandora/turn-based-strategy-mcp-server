using System.Collections.Generic;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Unity.Highlighters;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units
{
    /// <summary>
    /// Applies visual highlight effects for mage skills.
    /// </summary>
    public class MageSkillHighlighter : MonoBehaviour, IHealHighlighter, IRelocateHighlighter
    {
        [SerializeField] private List<Highlighter> _healHighlighterFn;
        [SerializeField] private List<Highlighter> _teleportHighlighterFn;

        public async Task ApplyHealEffect()
        {
            foreach (var fn in _healHighlighterFn)
            {
                await fn.Apply(NoParam.Instance);
            }
        }

        public async Task ApplyRelocateEffect(MoveHighlightParams @params)
        {
            foreach (var fn in _teleportHighlighterFn)
            {
                await fn.Apply(@params);
            }
        }
    }
}