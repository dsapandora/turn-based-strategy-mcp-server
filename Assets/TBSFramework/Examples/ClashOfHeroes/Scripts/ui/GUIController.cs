using System.Linq;
using TMPro;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Controllers.GameResolvers;
using TurnBasedStrategyFramework.Common.Players;
using TurnBasedStrategyFramework.Unity.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.UI
{
    /// <summary>
    /// GUI Controller for the Clash of Heroes demo, manages the game's user interface, 
    /// including turn control, game over display, and pause functionality.
    /// </summary>
    public class GUIController : MonoBehaviour
    {
        [SerializeField] private Button _endTurnButton;
        [SerializeField] private UnityGridController _gridController;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private TMP_Text _resultText;

        private void Awake()
        {
            Time.timeScale = 1;
            _gridController.TurnStarted += OnTurnStarted;
            _gridController.GameEnded += OnGameEnded;
        }

        private void OnTurnStarted(TurnTransitionParams turnTransitionParams)
        {
            _endTurnButton.interactable = turnTransitionParams.TurnContext.CurrentPlayer.PlayerType == PlayerType.HumanPlayer;
        }

        private async void OnGameEnded(GameResult gameResult)
        {
            await Awaitable.WaitForSecondsAsync(1f);

            _endTurnButton.interactable = false;
            _gameOverPanel.SetActive(true);
            _resultText.text = gameResult.Winners.Any(p => p.PlayerNumber == 0) ? "Victory!" : "Defeat!";
        }

        public void LoadLevel(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }

        public void TogglePauseMenu()
        {
            Time.timeScale = _pausePanel.activeInHierarchy ? 1 : 0;
            _pausePanel.SetActive(!_pausePanel.activeInHierarchy);
        }

        public void EndTurn()
        {
            _gridController.EndTurn();
        }
    }
}