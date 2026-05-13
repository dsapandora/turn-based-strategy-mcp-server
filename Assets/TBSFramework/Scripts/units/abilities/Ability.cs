using System;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Units.Abilities;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Units.Abilities
{
    /// <summary>
    /// A Unity-specific abstract base class representing an ability that a unit can perform in the game.
    /// </summary>
    public abstract class Ability : MonoBehaviour, IAbility
    {
        public event Action<IAbility> AbilitySelected;
        public event Action<IAbility> AbilityDeselected;

        public IUnit UnitReference { get; set; }

        public virtual void Initialize(IGridController gridController) { }
        public virtual void Display(IGridController gridController) { }
        public virtual void CleanUp(IGridController gridController) { }

        public virtual void OnUnitClicked(IUnit unit, IGridController gridController) { }
        public virtual void OnUnitDehighlighted(IUnit unit, IGridController gridController) { }
        public virtual void OnUnitHighlighted(IUnit unit, IGridController gridController) { }
        public virtual void OnUnitDestroyed(IGridController gridController) { }

        public virtual void OnCellClicked(ICell cell, IGridController gridController) { }
        public virtual void OnCellDehighlighted(ICell cell, IGridController gridController) { }
        public virtual void OnCellHighlighted(ICell cell, IGridController gridController) { }

        public virtual void OnAbilityDeselected(IGridController gridController) { }
        public virtual void OnAbilitySelected(IGridController gridController) { }

        public virtual void OnTurnStart(IGridController gridController) { }
        public virtual void OnTurnEnd(IGridController gridController) { }

        public virtual bool CanPerform(IGridController gridController) { return true; }

        public virtual void InvokeAbilitySelected()
        {
            AbilitySelected?.Invoke(this);
        }

        public virtual void InvokeAbilityDeselected()
        {
            AbilityDeselected?.Invoke(this);
        }
    }
}