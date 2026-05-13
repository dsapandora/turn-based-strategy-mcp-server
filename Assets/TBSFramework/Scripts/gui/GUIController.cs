using TurnBasedStrategyFramework.Unity.Controllers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TurnBasedStrategyFramework.Unity.Gui
{
    /// <summary>
    /// Basic GUI Controller for managing turn transitions.
    /// </summary>
    public class GUIController : MonoBehaviour
    {
        [SerializeField] UnityGridController _gridController;

        private void Update()
        {
            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                EndTurn();
            }
        }

        public void EndTurn()
        {
            _gridController.EndTurn();
        }

        public void SetGridController(UnityGridController gridController) 
        {
            _gridController = gridController;
        }
    }
}