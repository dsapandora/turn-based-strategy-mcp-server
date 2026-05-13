using System.Linq;
using TurnBasedStrategyFramework.Common.AI.Evaluators;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.Cells;
using TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Cells;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.AI.Evaluators
{
    /// <summary>
    /// Evaluates a grid cell’s desirability based on its elevation relative to the highest cell on the map.
    /// Higher cells yield higher scores, normalized by the maximum height.
    /// </summary>
    public struct HeightPositionEvaluator : IPositionEvaluator
    {
        public readonly float Weight { get; }
        private int _maxHeight;

        public HeightPositionEvaluator(float weight)
        {
            Weight = weight;
            _maxHeight = 0;
        }

        public readonly float EvaluatePosition(ICell evaluatedCell, IUnit evaluatingUnit, IGridController gridController)
        {
            return (evaluatedCell as Cell).GetComponent<IHeightComponent>().Height / _maxHeight;
        }

        public void Initialize(IUnit evaluatingUnit, IGridController gridController)
        {
            _maxHeight = gridController.CellManager.GetCells().Max(c => (c as Cell).GetComponent<IHeightComponent>().Height);
        }
    }
}