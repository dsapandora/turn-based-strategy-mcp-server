using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// A highlighter that activates or deactivates specified GameObjects based on the provided activation status.
    /// </summary>
    public class GameObjectsActivatorHighlighter : Highlighter
    {
        [SerializeField] private bool _activationStatus;
        [SerializeField] private List<GameObject> _targets;

        public override Task Apply(IHighlightParams @params)
        {
            foreach (var target in _targets)
            {
                target.SetActive(_activationStatus);
            }
            return Task.CompletedTask;
        }
    }
}