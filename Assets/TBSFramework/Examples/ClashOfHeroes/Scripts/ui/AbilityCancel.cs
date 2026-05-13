using TurnBasedStrategyFramework.Common.Controllers.GridStates;
using TurnBasedStrategyFramework.Common.Units.Abilities;
using TurnBasedStrategyFramework.Unity.Controllers;
using TurnBasedStrategyFramework.Unity.Units;
using TurnBasedStrategyFramework.Unity.Units.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.UI
{
    /// <summary>
    /// Allows to cancel selected ability
    /// </summary>
    public class AbilityCancel : MonoBehaviour
    {
        [SerializeField] private Ability _ability;
        [SerializeField] private Unit _unitReference;

        [SerializeField] private UnityGridController _gridController;
        [SerializeField] private Button _cancelButton;

        private void Awake()
        {
            _ability.AbilitySelected += OnAbilitySelected;
            _ability.AbilityDeselected += OnAbilityDeselected;
            _cancelButton.onClick.AddListener(() => { _gridController.GridState = new GridStateUnitSelected(_unitReference, _unitReference.GetBaseAbilities()); });
        }

        private void OnAbilityDeselected(IAbility ability)
        {
            _cancelButton.gameObject.SetActive(false);
        }

        private void OnAbilitySelected(IAbility ability)
        {
            _cancelButton.gameObject.SetActive(true);
        }
    }
}