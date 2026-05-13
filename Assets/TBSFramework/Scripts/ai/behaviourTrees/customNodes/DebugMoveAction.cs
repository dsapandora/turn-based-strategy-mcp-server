using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.AI.BehaviourTrees;
using TurnBasedStrategyFramework.Common.AI.Evaluators;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TurnBasedStrategyFramework.Unity.AI.BehaviourTrees
{
    /// <summary>
    /// A debug-only tree node that evaluates all movable cells for the AI unit,
    /// scores them using multiple evaluators, and visualizes the results with a color gradient.
    /// Allows manual inspection of scores per cell and pauses until user presses the Q key on the keyboard.
    /// </summary>
    public class DebugMoveAction : ITreeNode
    {
        private IUnit _unit;
        private IGridController _gridController;
        private readonly IEnumerable<IPositionEvaluator> _positionEvaluators;
        private Gradient _debugGradient;

        private Dictionary<Vector2Int, (float sum, List<(string evaluatorName, float score)> scores)> cellScores = new();

        private float _minValue;
        public float _maxValue;

        public DebugMoveAction(IUnit unit, IGridController gridController, IEnumerable<IPositionEvaluator> positionEvaluators)
        {
            _unit = unit;
            _positionEvaluators = positionEvaluators;
            _gridController = gridController;
            var colorKeys = new GradientColorKey[3];

            colorKeys[0] = new GradientColorKey(Color.red, 0f);
            colorKeys[1] = new GradientColorKey(Color.yellow, 0.5f);
            colorKeys[2] = new GradientColorKey(Color.green, 1f);

            _debugGradient = new Gradient();
            _debugGradient.SetKeys(colorKeys, new GradientAlphaKey[0]);
        }

        public async Task<bool> Execute(bool debugMode)
        {
            if (!debugMode)
            {
                return true;
            }

            foreach (var cell in _gridController.CellManager.GetCells())
            {
                cell.CellClicked += OnCellCicked;
                cell.CellDehighlighted += OnCellDehighlighted;
            }

            foreach (var unit in _gridController.UnitManager.GetUnits())
            {
                unit.UnitClicked += OnUnitClicked;
            }

            foreach (var positionEvaluator in _positionEvaluators)
            {
                positionEvaluator.Initialize(_unit, _gridController);
            }

            foreach (var cell in _gridController.CellManager.GetCells()
                .Where(c => _unit.IsCellMovableTo(c) || c.Equals(_unit.CurrentCell)))
            {
                var scores = _positionEvaluators
                    .Select(e => (
                        evaluatorName: e.GetType().Name,
                        score: e.EvaluatePosition(cell, _unit, _gridController) * e.Weight
                    ))
                    .ToList();

                var scoreSum = scores.Sum(s => s.score);
                cellScores[cell.GridCoordinates.ToVector2Int()] = (scoreSum, scores);
            }

            var min = cellScores.Values.OrderBy(c => c.sum).First();
            var max = cellScores.Values.OrderByDescending(c => c.sum).First();

            _minValue = min.sum;
            _maxValue = max.sum;

            Debug.Log($"Min Value: {_minValue}");
            Debug.Log($"Max Value: {_maxValue}");

            foreach (var cell in _gridController.CellManager.GetCells()
                .Where(c => _unit.IsCellMovableTo(c) || c.Equals(_unit.CurrentCell)))
            {
                var scoreSum = cellScores[cell.GridCoordinates.ToVector2Int()].sum;
                var t = (scoreSum - _minValue) / Math.Max(_maxValue - _minValue, float.Epsilon);
                var color = _debugGradient.Evaluate(t);
                _gridController.CellManager.SetColor(cell, color.r, color.g, color.b, color.a);

                if (scoreSum.Equals(_maxValue))
                {
                    _gridController.CellManager.SetColor(cell, 0.0f, 0.0f, 1.0f, 1.0f);
                }
            }

            Debug.Log($"Click on any cell to check its score. Press {Key.Q} to continue.");
            while (!Keyboard.current.qKey.wasPressedThisFrame)
            {
                await Awaitable.NextFrameAsync();
            }

            foreach (var cell in _gridController.CellManager.GetCells())
            {
                cell.CellClicked -= OnCellCicked;
                cell.CellDehighlighted -= OnCellDehighlighted;
            }

            foreach (var unit in _gridController.UnitManager.GetUnits())
            {
                unit.UnitClicked -= OnUnitClicked;
            }

            _ = _gridController.CellManager
                .UnMark(_gridController.CellManager.GetCells()
                .Where(c => _unit.IsCellMovableTo(c) || c.Equals(_unit.CurrentCell)));

            return true;
        }

        private void OnUnitClicked(IUnit unit)
        {
            OnCellCicked(unit.CurrentCell);
        }

        private void OnCellDehighlighted(ICell cell)
        {
            if (cellScores.TryGetValue(cell.GridCoordinates.ToVector2Int(), out (float sum, List<(string evaluatorName, float score)> scores) value))
            {
                var scoreSum = value.sum;
                var t = (scoreSum - _minValue) / Math.Max(_maxValue - _minValue, float.Epsilon);
                var color = _debugGradient.Evaluate(t);
                _gridController.CellManager.SetColor(cell, color.r, color.g, color.b, color.a);
            }
        }

        private void OnCellCicked(ICell obj)
        {
            if (!cellScores.ContainsKey(obj.GridCoordinates.ToVector2Int())) return;

            var (sum, scores) = cellScores[obj.GridCoordinates.ToVector2Int()];
            var logMessage = $"Scores for cell {obj.GridCoordinates}:\n";

            logMessage += string.Join("\n", scores.Select(s => $"Evaluator: {s.evaluatorName}, Score: {s.score}"));
            logMessage += $"\nTotal Score: {sum}";

            Debug.Log(logMessage);
        }

    }
}
