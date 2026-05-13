using System.Threading.Tasks;
using TurnBasedStrategyFramework.Unity.Units;
using TurnBasedStrategyFramework.Unity.Utilities;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// A highlighter that repositions a transform to a unit's destination cell, defined by MoveHighlightParams.
    /// </summary>
    public class RelocateToDestinationHighlighter : Highlighter
    {
        [SerializeField] private Transform _transform;

        public override Task Apply(IHighlightParams @params)
        {
            var moveHighlightParams = (MoveHighlightParams)@params;
            _transform.position = moveHighlightParams.Destination.WorldPosition.ToVector3();
            return Task.CompletedTask;
        }
    }
}