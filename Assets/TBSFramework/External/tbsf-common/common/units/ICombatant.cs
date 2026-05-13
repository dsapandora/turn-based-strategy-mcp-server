using System;
using TurnBasedStrategyFramework.Common.Cells;

namespace TurnBasedStrategyFramework.Common.Units
{
    /// <summary>
    /// Defines combat-related behavior for a unit.
    /// </summary>
    /// <remarks>
    /// This interface is used to separate concerns related to combat, keeping the
    /// <see cref="IUnit"/> interface more readable.
    public interface ICombatant
    {
        /// <summary>
        /// Triggered when the unit is attacked
        /// </summary>
        event Action<UnitAttackedEventArgs> UnitAttacked;

        /// <summary>
        /// Triggered when the unit is destroyed.
        /// </summary>
        event Action<UnitDestroyedEventArgs> UnitDestroyed;

        /// <summary>
        /// Triggered when the unit's health changes.
        /// </summary>
        event Action<HealthChangedEventArgs> HealthChanged;

        /// <summary>
        /// Invokes the event to indicate that the unit has been attacked.
        /// </summary>
        /// <param name="eventArgs">The event arguments containing attack details.</param>
        void InvokeAttacked(UnitAttackedEventArgs eventArgs);

        /// <summary>
        /// Invokes the event to indicate that the unit has been destroyed.
        /// </summary>
        /// <param name="eventArgs">The event arguments containing destruction details.</param>
        void InvokeDestroyed(UnitDestroyedEventArgs eventArgs);

        /// <summary>
        /// Invokes the event to indicate that the unit's health has changed.
        /// </summary>
        /// <param name="eventArgs">The event arguments containing health change details.</param>
        void InvokeHealthChanged(HealthChangedEventArgs eventArgs);

        /// <summary>
        /// The current health of the unit.
        /// </summary>
        float Health { get; set; }

        /// <summary>
        /// The maximum health of the unit.
        /// </summary>
        float MaxHealth { get; }

        /// <summary>
        /// The attack range of the unit, indicating how far it can attack.
        /// </summary>
        int AttackRange { get; set; }

        /// <summary>
        /// The attack factor of the unit, used to determine damage dealt to opponents.
        /// </summary>
        int AttackFactor { get; set; }

        /// <summary>
        /// The defense factor of the unit, used to reduce incoming damage.
        /// </summary>
        int DefenceFactor { get; set; }

        /// <summary>
        /// Modifies the health of the unit by the specified amount.
        /// </summary>
        /// <param name="healthChangeAmount">The amount to change the unit's health by.</param>
        /// <param name="sourceUnit">The unit responsible for the health change.</param>
        void ModifyHealth(float healthChangeAmount, IUnit sourceUnit);

        /// <summary>
        /// Determines if the specified unit is attackable from the given cell.
        /// </summary>
        /// <param name="otherUnit">The unit to check if attackable.</param>
        /// <param name="otherUnitCell">The cell that the target unit occupies.</param>
        /// <param name="attackSourceCell">The cell from which the attack would originate.</param>
        /// <returns>True if the unit is attackable, otherwise false.</returns>
        bool IsUnitAttackable(IUnit otherUnit, ICell otherUnitCell, ICell attackSourceCell);

        /// <summary>
        /// Calculates the base damage this unit would deal to a specific defender, 
        /// applying any offensive modifiers.
        /// </summary>
        /// <param name="defender">The unit being targeted by the attack.</param>
        /// <param name="defenderCell">The cell occupied by the defender.</param>
        /// <param name="aggressorCell">The cell from which the attack is made.</param>
        /// <returns>The raw amount of damage before any defensive modifiers are applied.</returns>
        float CalculateDamageDealt(IUnit defender, ICell defenderCell, ICell aggressorCell);

        /// <summary>
        /// Calculates the base damage this unit would deal to a specific defender,
        /// applying any offensive modifiers, using the current positions of both units.
        /// </summary>
        /// <param name="defender">The unit being targeted by the attack.</param>
        /// <returns>The raw amount of damage before any defensive modifiers are applied.</returns>
        float CalculateDamageDealt(IUnit defender);

        /// <summary>
        /// Modifies the incoming damage based on this unit's defensive attributes.
        /// </summary>
        /// <param name="aggressor">The attacking unit.</param>
        /// <param name="damageDealt">The raw damage dealt by the attacker before mitigation.</param>
        /// <param name="aggressorCell">The cell from which the attack originates.</param>
        /// <param name="defenderCell">The cell this unit is occupying.</param>
        /// <returns>The final damage this unit will take after defensive modifiers are applied.</returns>
        float CalculateDamageTaken(IUnit aggressor, float damageDealt, ICell aggressorCell, ICell defenderCell);

        /// <summary>
        /// Modifies the incoming damage based on this unit's defensive attributes, 
        /// using the current positions of both the aggressor and the defender.
        /// </summary>
        /// <param name="aggressor">The unit performing the attack.</param>
        /// <param name="damageDealt">The raw damage dealt by the attacker before mitigation.</param>
        /// <returns>The final damage this unit will take after defensive modifiers are applied.</returns>
        float CalculateDamageTaken(IUnit aggressor, float damageDealt);

        /// <summary>
        /// Calculates the final damage dealt to the target unit, combining both 
        /// the attacker's offensive modifiers and the defender's mitigation effects.
        /// </summary>
        /// <param name="defender">The unit receiving the attack.</param>
        /// <param name="defenderCell">The cell occupied by the defender.</param>
        /// <param name="aggressorCell">The cell from which the attacker strikes.</param>
        /// <returns>The total effective damage dealt to the defender after all modifiers.</returns>
        float CalculateTotalDamage(IUnit defender, ICell defenderCell, ICell aggressorCell);

        /// <summary>
        /// Calculates the final damage dealt to the target unit, combining both 
        /// the attacker's offensive modifiers and the defender's mitigation effects,
        /// using the current positions of both units.
        /// </summary>
        /// <param name="defender">The unit receiving the attack.</param>
        /// <returns>The total effective damage dealt to the defender after all modifiers.</returns>
        float CalculateTotalDamage(IUnit defender);
    }

    /// <summary>
    /// Event arguments for when a unit is attacked.
    /// </summary>
    public readonly struct UnitAttackedEventArgs
    {
        public readonly IUnit AffectedUnit;
        public readonly IUnit AttackingUnit;
        public readonly float DamageDealt;

        public UnitAttackedEventArgs(IUnit affectedUnit, IUnit attackingUnit, float damageDealt)
        {
            AffectedUnit = affectedUnit;
            AttackingUnit = attackingUnit;
            DamageDealt = damageDealt;
        }
    }

    /// <summary>
    /// Event arguments for when a unit's health changes.
    /// </summary>
    public readonly struct HealthChangedEventArgs
    {
        public readonly IUnit AffectedUnit;
        public readonly IUnit SourceUnit;
        public readonly float HealthChangeAmount;

        public HealthChangedEventArgs(IUnit affectedUnit, IUnit sourceUnit, float healthChangeAmount)
        {
            AffectedUnit = affectedUnit;
            SourceUnit = sourceUnit;
            HealthChangeAmount = healthChangeAmount;
        }
    }

    /// <summary>
    /// Event arguments for when a unit is destroyed.
    /// </summary>
    public readonly struct UnitDestroyedEventArgs
    {
        public readonly IUnit AffectedUnit;
        public readonly IUnit AttackingUnit;

        public UnitDestroyedEventArgs(IUnit affectedUnit, IUnit attackingUnit)
        {
            AffectedUnit = affectedUnit;
            AttackingUnit = attackingUnit;
        }
    }
}