using TurnBasedStrategyFramework.Common.Controllers.GridStates;
using TurnBasedStrategyFramework.Unity.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Utilities
{
    /// <summary>
    /// Deselects currently selected unit when clicked on. 
    /// </summary>
    public class DeselectUnitOnClick : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private UnityGridController _gridController;
        public void OnPointerClick(PointerEventData eventData)
        {
            _gridController.GridState = new GridStateAwaitInput();
        }
    }
}