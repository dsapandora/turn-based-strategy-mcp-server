using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units.Abilities;
using TurnBasedStrategyFramework.Unity.Units.Abilities;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units.Abilities
{
    /// <summary>
    /// A base ability in the Clash of Heroes demo.
    /// </summary>
    public class ClashOfHeroesAbility : Ability, IAbilityDetails
    {
        [SerializeField] private string _abilityDescription;
        [SerializeField] private string _abilityName;
        [SerializeField] private Sprite _abilityImage;
        [SerializeField] private int _maxCharges;
        [SerializeField] private bool _hasLimitedCharges;

        public string AbilityDescription { get => _abilityDescription; set => _abilityDescription = value; }
        public string AbilityName { get => _abilityName; set => _abilityName = value; }
        public Sprite AbilityImage { get => _abilityImage; set => _abilityImage = value; }

        public int Charges { get; set; }
        public int MaxCharges { get => _maxCharges; set => _maxCharges = value; }
        public bool HasLimitedCharges { get => _hasLimitedCharges; set => _hasLimitedCharges = value; }

        public override void Initialize(IGridController gridController)
        {
            base.Initialize(gridController);
            Charges = MaxCharges;
        }

        protected void HumanExecuteAbility(ICommand command, IGridController gridController, bool isNetworkInvoked = false)
        {
            UnitReference.HumanExecuteAbility(command, gridController, (_) => Task.CompletedTask, (_) => { Charges--; (UnitReference as ITurnAbilityLimit).AbilityUsePoints--; return Task.CompletedTask; }, isNetworkInvoked);
        }

        public override bool CanPerform(IGridController gridController)
        {
            return (!HasLimitedCharges || Charges > 0) && (UnitReference as ITurnAbilityLimit).AbilityUsePoints > 0;
        }
    }
}