using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.UI
{
    /// <summary>
    /// Controls the main menu in Clash of Heroes demo, loading scenes based on button clicks.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private List<Button> _levelSelectButtons = new List<Button>();
        [SerializeField] private List<string> _levelNames = new List<string>();

        private void Awake()
        {
            Time.timeScale = 1.0f;

            for (int i = 0; i < _levelSelectButtons.Count; i++)
            {
                var button = _levelSelectButtons[i];
                var levelName = _levelNames[i];

                button.onClick.AddListener(() => { SceneManager.LoadScene(levelName); });

            }
        }
    }
}