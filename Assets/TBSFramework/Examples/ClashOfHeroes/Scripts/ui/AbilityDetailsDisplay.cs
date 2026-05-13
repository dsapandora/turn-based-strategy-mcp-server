using TMPro;
using TurnBasedStrategyFramework.Common.Units.Abilities;
using TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities;
using TurnBasedStrategyFramework.Unity.Units.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.UI
{
    /// <summary>
    /// Handles the display of ability details when an ability is selected or deselected.
    /// </summary>
    public class AbilityDetailsDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject _detailsCanvas;
        [SerializeField] private Image _abilityImage;
        [SerializeField] private TMP_Text _abilityNameText;
        [SerializeField] private TMP_Text _abilityDescriptionText;

        [SerializeField] private Ability _ability;

        private void Awake()
        {
            _ability.AbilitySelected += OnAbilitySelected;
            _ability.AbilityDeselected += OnAbilityDeselected;
        }

        private void OnAbilityDeselected(IAbility ability)
        {
            _detailsCanvas.SetActive(false);
        }

        private void OnAbilitySelected(IAbility ability)
        {
            var abilityDetails = _ability.GetComponent<IAbilityDetails>();
            _abilityImage.sprite = abilityDetails.AbilityImage;
            _abilityNameText.text = abilityDetails.AbilityName;
            _abilityDescriptionText.text = $"charges: {abilityDetails.Charges} / {abilityDetails.MaxCharges} <size=20><color=purple>*total uses per game</size></color>\n{abilityDetails.AbilityDescription}";

            _detailsCanvas.SetActive(true);
        }
    }
}