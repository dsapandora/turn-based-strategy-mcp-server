using System.Threading.Tasks;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// A highlighter that activates or deactivates a specified GameObject based on the provided activation status.
    /// </summary>
    public class GameObjectActivatorHighlighter : Highlighter
    {
        [SerializeField] private bool _activationStatus;
        [SerializeField] private GameObject _target;

        /// <summary>
        /// Delay in milliseconds
        /// </summary>
        [SerializeField] private float _delay;

        public override async Task Apply(IHighlightParams @params)
        {
            _target.SetActive(_activationStatus);
            if(_delay > 0 ) 
            {
                await Awaitable.WaitForSecondsAsync(_delay / 1000f);
            }
            await Task.CompletedTask;
        }
    }
}