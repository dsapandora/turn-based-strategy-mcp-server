using System.Collections.Generic;
using System.Linq;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Controllers.GridStates;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.Units.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities
{
    /// <summary>
    /// Represents an ability menu that displays a list of available abilities for a unit.
    /// </summary>
    public class AbilityMenu : Ability
    {
        [SerializeField] private List<Ability> _abilities;

        [SerializeField] private GameObject _abilityMenu;
        [SerializeField] private GameObject _abilityPanel;
        [SerializeField] private Button _abilityActivationButtonPrefab;

        private List<Button> _buttons = new List<Button>();
        public override void Display(IGridController gridController)
        {
            _buttons = new List<Button>();

            foreach (var ability in _abilities)
            {
                var button = Instantiate(_abilityActivationButtonPrefab, _abilityPanel.transform);
                button.onClick.AddListener(() => { gridController.GridState = new GridStateUnitSelected(UnitReference, ability); });
                button.transform.Find("AbilityImage").GetComponent<Image>().sprite = ability.GetComponent<IAbilityDetails>().AbilityImage;
                button.gameObject.SetActive(true);
                button.interactable = ability.CanPerform(gridController);

                _buttons.Add(button);
            }
            _abilityMenu.SetActive(true);
        }

        public override void CleanUp(IGridController gridController)
        {
            _abilityMenu.SetActive(false);
            foreach (var button in _buttons)
            {
                Destroy(button.gameObject);
            }
            _buttons.Clear();
        }

        public override void Initialize(IGridController gridController)
        {
            base.Initialize(gridController);
            foreach (var ability in _abilities)
            {
                UnitReference.RegisterAbility(ability, gridController);
            }
        }

        public override void OnCellClicked(ICell cell, IGridController gridController)
        {
        }

        public override void OnUnitClicked(IUnit unit, IGridController gridController)
        {
            if (gridController.UnitManager.GetFriendlyUnits(UnitReference.PlayerNumber).Contains(unit))
            {
                gridController.GridState = new GridStateUnitSelected(unit, unit.GetBaseAbilities());
            }
            else
            {
                gridController.GridState = new GridStateAwaitInput();
            }
        }

        public override bool CanPerform(IGridController gridController)
        {
            return _abilities.Select(a => a.CanPerform(gridController)).Any(r => r == true);
        }
    }
}