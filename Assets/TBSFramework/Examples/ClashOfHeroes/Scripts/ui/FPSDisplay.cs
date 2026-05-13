using TMPro;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.UI
{
    /// <summary>
    /// Displays the current frames per second.
    /// </summary>
    public class FPSDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _fpsText;

        private int _frameCount;
        private float _deltaTime;
        private float _fps;

        void Update()
        {
            _frameCount++;
            _deltaTime += Time.unscaledDeltaTime;
            if (_deltaTime > 1.0f)
            {
                _fps = _frameCount / _deltaTime;
                _frameCount = 0;
                _deltaTime = 0;
            }

            if (_fpsText != null)
            {
                _fpsText.text = "FPS: " + _fps.ToString("F0");
            }
        }
    }
}