using System.Linq;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Unity.Cells;
using TurnBasedStrategyFramework.Unity.Highlighters;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Highlighters
{
    /// <summary>
    /// Displays a confirmation marker on a specific cell if it is the final destination in a given path.
    /// Used to indicate a player's selected move target.
    /// </summary>
    public class PathConfirmationHighlighter : Highlighter
    {
        [SerializeField] private GameObject _confirmationMarker;
        [SerializeField] private Cell _cellReference;

        public override Task Apply(IHighlightParams @params)
        {
            var pathHighlightParams = (PathHighlightParams)@params;
            if (pathHighlightParams.Path.Last().Equals(_cellReference))
            {
                _confirmationMarker.SetActive(true);
            }
            return Task.CompletedTask;
        }
    }
}