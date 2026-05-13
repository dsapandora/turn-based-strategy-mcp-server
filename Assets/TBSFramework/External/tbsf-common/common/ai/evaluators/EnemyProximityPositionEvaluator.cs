using System.Linq;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.AI.Evaluators
{
    public struct EnemyProximityPositionEvaluator : IPositionEvaluator
    {
        public readonly float Weight { get; }
        private float _maxScore;

        public EnemyProximityPositionEvaluator(float weight)
        {
            Weight = weight;
            _maxScore = 0f;
        }

        public void Initialize(IUnit evaluatingUnit, IGridController gridController)
        {
            var enemyCount = gridController.UnitManager.GetEnemyUnits(evaluatingUnit.PlayerNumber).Count();
            _maxScore = enemyCount * 0.5f;
        }

        public float EvaluatePosition(ICell evaluatedCell, IUnit evaluatingUnit, IGridController gridController)
        {
            var distances = gridController.UnitManager
                .GetEnemyUnits(evaluatingUnit.PlayerNumber)
                .Select(u => u.CurrentCell.GetDistance(evaluatedCell));

            float rawScore = distances.Sum(d => 1f / (d + 1));
            float normalizedScore = _maxScore > 0 ? rawScore / _maxScore : 0;

            return normalizedScore * Weight;
        }
    }
}