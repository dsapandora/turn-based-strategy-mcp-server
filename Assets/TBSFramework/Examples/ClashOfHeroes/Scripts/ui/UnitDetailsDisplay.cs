using System;
using System.Text;
using TMPro;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.Cells;
using TurnBasedStrategyFramework.Unity.Controllers;
using TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.UI
{
    /// <summary>
    /// Displays the selected unit's details, including portrait, stats, and animated health changes.
    /// Reacts to unit click events and updates UI elements accordingly.
    /// </summary>
    public class UnitDetailsDisplay : MonoBehaviour
    {
        [SerializeField] private UnityGridController _gridController;
        [SerializeField] private UnityUnitManager _unitManager;
        [SerializeField] private UnityCellManager _cellManager;
        [SerializeField] private GameObject _unitDataPanel;

        [SerializeField] private Image _unitPortrait;
        [SerializeField] private TMP_Text _unitName;
        [SerializeField] private TMP_Text _unitStats;
        [SerializeField] private TMP_Text _unitHealth;

        private IUnit _selectedUnit;
        [SerializeField] private AnimationCurve _healthAnimationCurve;
        [SerializeField] private float _healthChangeAnimationTime;

        private bool _gameEnded = false;

        private void Awake()
        {
            _unitManager.UnitAdded += OnUnitAdded;
            _unitManager.UnitRemoved += OnUnitRemoved;

            _cellManager.CellAdded += OnCellAdded;
            _gridController.TurnEnded += (_) => { _selectedUnit = null; _unitDataPanel.SetActive(false); };
            _gridController.GameEnded += (_) => { _gameEnded = true; };
        }

        private void OnCellAdded(ICell cell)
        {
            cell.CellClicked += (_) => { _selectedUnit = null; _unitDataPanel.SetActive(false); };
        }

        private void OnUnitAdded(IUnit unit)
        {
            unit.UnitClicked += OnUnitClicked;
        }

        private void OnUnitRemoved(IUnit unit)
        {
            unit.UnitClicked -= OnUnitClicked;
        }

        private void OnUnitClicked(IUnit unit)
        {
            if (_gameEnded)
            {
                return;
            }
            if (_selectedUnit != null)
            {
                _selectedUnit.HealthChanged -= OnHealthChanged;
            }
            _selectedUnit = unit;
            _selectedUnit.HealthChanged += OnHealthChanged;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"attack: {unit.AttackFactor} <size=20><color=purple>*deal double damage attaking from high ground.</size></color>");
            if (unit.AttackRange > 1) sb.AppendLine($"range:  {unit.AttackRange}");
            if ((unit as ITurnAbilityLimit).GetMaxAbilityUsesPerTurn() > 0) sb.Append($"skill points: {(unit as ITurnAbilityLimit).AbilityUsePoints} / {(unit as ITurnAbilityLimit).GetMaxAbilityUsesPerTurn()} <size=20><color=purple>*abilities cost skill points, replenished each turn.</size></color>");

            _unitPortrait.sprite = (unit as IUnitDetails).UnitPortrait;
            _unitName.text = (unit as IUnitDetails).UnitName;
            _unitHealth.text = $"health: {unit.Health} / {unit.MaxHealth}";
            _unitStats.text = sb.ToString();
            _unitDataPanel.SetActive(true);
        }

        private async void OnHealthChanged(HealthChangedEventArgs obj)
        {
            float elapsedTime = 0f;
            float initialHealth = obj.AffectedUnit.Health - obj.HealthChangeAmount;
            float targetHealth = Math.Max(0, obj.AffectedUnit.Health);
            float effectiveHealthChange = targetHealth - initialHealth;

            while (elapsedTime < _healthChangeAnimationTime)
            {
                elapsedTime = Math.Min(elapsedTime + Time.deltaTime, _healthChangeAnimationTime);
                float t = _healthAnimationCurve.Evaluate(elapsedTime / _healthChangeAnimationTime);
                float currentHealth = initialHealth + (float)Math.Ceiling(effectiveHealthChange * t);

                _unitHealth.text = $"health: {currentHealth} / {obj.AffectedUnit.MaxHealth}";
                await Awaitable.NextFrameAsync();
            }

            _unitHealth.text = $"health: {targetHealth} / {obj.AffectedUnit.MaxHealth}";

            if (obj.AffectedUnit.Health <= 0)
            {
                _unitDataPanel.SetActive(false);
                _selectedUnit = null;
            }
        }
    }
}