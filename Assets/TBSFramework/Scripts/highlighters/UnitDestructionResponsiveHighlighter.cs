using System.Collections.Generic;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// A highlighter wrapper that applies a collection of highlighters to a unit, 
    /// and ensures that the highlighting stops when the associated unit is destroyed.
    /// </summary>
    public class UnitDestructionResponsiveHighlighter : Highlighter
    {
        [SerializeField] private List<Highlighter> _highlighters;
        [SerializeField] private Unit _unitReference;

        private bool _isDestroyed;

        public override async Task Apply(IHighlightParams @params)
        {
            foreach (var highlighter in _highlighters)
            {
                if (!_isDestroyed)
                {
                    await highlighter.Apply(@params);
                }
            }
        }

        private void Awake()
        {
            _unitReference.UnitDestroyed += (args) => { _isDestroyed = true; };
        }
    }
}