using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Common.AI.Evaluators
{
    /// <summary>
    /// Evaluates a position based on the potential damage that the unit would receive from enemy units.
    /// </summary>
    public struct DamageReceivedPositionEvaluator : IPositionEvaluator
    {
        public readonly float Weight { get; }
        private readonly float _decayRate;
        private readonly float _epsilon;

        private readonly Dictionary<ICell, float> _baseScores;
        private readonly Dictionary<ICell, float> _accumulatedScores;
        private float _maxAccumulatedScore;

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageReceivedPositionEvaluator"/> struct with the specified weight.
        /// </summary>
        /// <param name="weight">The weight of this evaluator in the scoring system.</param>
        public DamageReceivedPositionEvaluator(float weight, float decayRate = 0.5f)
        {
            Weight = weight;
            _decayRate = decayRate;
            _epsilon = 1e-6f;

            _baseScores = new Dictionary<ICell, float>();
            _accumulatedScores = new Dictionary<ICell, float>();
            _maxAccumulatedScore = 0;
        }


        public void Initialize(IUnit evaluatingUnit, IGridController gridController)
        {
            _baseScores.Clear();
            _accumulatedScores.Clear();

            var possibleCells = gridController.CellManager.GetCells()
                .Where(c => evaluatingUnit.IsCellMovableTo(c) || c.Equals(evaluatingUnit.CurrentCell))
                .ToList();

            foreach (var cell in possibleCells)
            {
                float baseScore = gridController.UnitManager.GetEnemyUnits(evaluatingUnit.PlayerNumber)
                    .Where(u => u.IsUnitAttackable(evaluatingUnit, cell, u.CurrentCell))
                    .Select(u =>
                    {
                        return u.CalculateTotalDamage(evaluatingUnit, cell, u.CurrentCell);
                    })
                    .DefaultIfEmpty(0f)
                    .Max();

                _baseScores[cell] = Math.Min(baseScore / evaluatingUnit.Health, 1f);
            }

            float maxScore = _baseScores.Values.Max();
            foreach (var cell in _baseScores.Keys.ToList())
            {
                _baseScores[cell] /= (maxScore + _epsilon);
            }

            foreach (var cell in possibleCells)
            {
                float localSum = _baseScores[cell];
                foreach (var otherCell in possibleCells)
                {
                    float distance = otherCell.GetDistance(cell);
                    localSum += _baseScores[otherCell] * (float)Math.Pow((1 - _decayRate), distance);
                }

                _accumulatedScores[cell] = localSum;
            }

            _maxAccumulatedScore = _accumulatedScores.Values.Max();
        }


        /// <summary>
        /// Evaluates a position based on the potential damage that would be received from enemy units if the evaluating unit were to move to the specified cell.
        /// </summary>
        /// <param name="evaluatedCell">The cell to evaluate.</param>
        /// <param name="evaluatingUnit">The unit performing the evaluation.</param>
        /// <param name="gridController">The grid controller managing the game state.</param>
        /// <returns>A float value representing the evaluation score, scaled by the potential damage received relative to the unit's health.</returns>
        public readonly float EvaluatePosition(ICell evaluatedCell, IUnit evaluatingUnit, IGridController gridController)
        {
            float final = _accumulatedScores[evaluatedCell];
            return final / (_maxAccumulatedScore + _epsilon);
        }

    }
}
