using System.Threading.Tasks;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// A highlighter that introduce a realtime delay.
    /// </summary>
    public class DelayHighlighter : Highlighter
    {
        /// <summary>
        /// Delay to apply in milliseconds.
        /// </summary>
        [SerializeField] private int _delay;
        public async override Task Apply(IHighlightParams @params)
        {
            await Awaitable.WaitForSecondsAsync(_delay / 1000f);
        }
    }
}