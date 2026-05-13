using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// A highlighter that combines multiple highlighters into a single effect.
    /// Executes all included highlighters concurrently when applied.
    /// </summary>
    public class CompoundHighlighter : Highlighter
    {
        /// <summary>
        /// A list of highlighters to execute as part of the compound effect.
        /// Each highlighter in this list will run concurrently.
        /// </summary>
        [SerializeField] private List<Highlighter> _highlighters;
        public override Task Apply(IHighlightParams @params)
        {
            return Task.WhenAll(_highlighters.Select(h => h.Apply(@params)));
        }
    }
}