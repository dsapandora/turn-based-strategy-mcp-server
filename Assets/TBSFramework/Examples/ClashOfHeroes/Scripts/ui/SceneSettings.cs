using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.UI
{
    /// <summary>
    /// Manages scene settings such as resolution, frame rate, shadow distance, and camera configuration.
    /// Adjusts the camera settings based on the screen aspect ratio.
    /// </summary>
    public class SceneSettings : MonoBehaviour
    {
        [Tooltip("Scale factor applied to the screen resolution.")]
        [SerializeField] private float _renderScale = 1.0f;
        [Tooltip("Default window width when in windowed mode (so the window is not too small).")]
        [SerializeField] private int _windowedWidth = 800;
        [Tooltip("Default window height when in windowed mode.")]
        [SerializeField] private int _windowedHeight = 600;
        [Tooltip("Target frames per second for the application.")]
        [SerializeField] private int _targetFPS = 60;
        [Tooltip("Maximum shadow rendering distance.")]
        [SerializeField] private int _shadowDistance = 75;

        [Tooltip("The primary scene camera.")]
        [SerializeField] private Camera _mainCamera;
        [Tooltip("The UI camera.")]
        [SerializeField] private Camera _uiCamera;

        [Tooltip("Camera settings for 16:9 aspect ratio.")]
        [SerializeField] private CameraSettings _cameraSettings16_9;
        [Tooltip("Camera settings for 18:9 aspect ratio.")]
        [SerializeField] private CameraSettings _cameraSettings18_9;
        [Tooltip("Camera settings for 19.5:9 aspect ratio.")]
        [SerializeField] private CameraSettings _cameraSettings195_9;
        [Tooltip("Camera settings for 20:9 aspect ratio.")]
        [SerializeField] private CameraSettings _cameraSettings20_9;
        [Tooltip("Camera settings for 21:9 aspect ratio.")]
        [SerializeField] private CameraSettings _cameraSettings21_9;
        [Tooltip("Camera settings for 4:3 aspect ratio.")]
        [SerializeField] private CameraSettings _cameraSettings4_3;
        [Tooltip("Camera settings for 3:2 aspect ratio.")]
        [SerializeField] private CameraSettings _cameraSettings3_2;
        [Tooltip("Camera settings for 16:10 aspect ratio.")]
        [SerializeField] private CameraSettings _cameraSettings16_10;

        private static bool _initialized = false;
        private static int _nativeWidth;
        private static int _nativeHeight;

        void Awake()
        {
            if (!_initialized)
            {
                // Use a proper default size in windowed so the window is not tiny
                _nativeWidth = _windowedWidth;
                _nativeHeight = _windowedHeight;
                _initialized = true;
            }

            Application.targetFrameRate = _targetFPS;
            Time.fixedDeltaTime = 1.0f / _targetFPS;

            int targetWidth = (int)(_nativeWidth * _renderScale);
            int targetHeight = (int)(_nativeHeight * _renderScale);

            Screen.SetResolution(targetWidth, targetHeight, FullScreenMode.Windowed);
            QualitySettings.shadowDistance = _shadowDistance;

            float aspectRatio = (float)Screen.height / (float)Screen.width;

            if (Mathf.Approximately(aspectRatio, 16.0f / 9.0f))
            {
                ApplyCameraSettings(_cameraSettings16_9);
            }
            else if (Mathf.Approximately(aspectRatio, 18.0f / 9.0f))
            {
                ApplyCameraSettings(_cameraSettings18_9);
            }
            else if (Mathf.Approximately(aspectRatio, 19.5f / 9.0f))
            {
                ApplyCameraSettings(_cameraSettings195_9);
            }
            else if (Mathf.Approximately(aspectRatio, 20.0f / 9.0f))
            {
                ApplyCameraSettings(_cameraSettings20_9);
            }
            else if (Mathf.Approximately(aspectRatio, 21.0f / 9.0f))
            {
                ApplyCameraSettings(_cameraSettings21_9);
            }
            else if (Mathf.Approximately(aspectRatio, 4.0f / 3.0f))
            {
                ApplyCameraSettings(_cameraSettings4_3);
            }
            else if (Mathf.Approximately(aspectRatio, 3.0f / 2.0f))
            {
                ApplyCameraSettings(_cameraSettings3_2);
            }
            else if (Mathf.Approximately(aspectRatio, 16.0f / 10.0f))
            {
                ApplyCameraSettings(_cameraSettings16_10);
            }
        }

        void ApplyCameraSettings(CameraSettings cameraSettings)
        {
            _mainCamera.transform.position = cameraSettings.Position;
            _mainCamera.transform.eulerAngles = cameraSettings.Rotation;
            _mainCamera.fieldOfView = cameraSettings.FOV;

            _uiCamera.transform.position = cameraSettings.Position;
            _uiCamera.transform.eulerAngles = cameraSettings.Rotation;
            _uiCamera.fieldOfView = cameraSettings.FOV;

        }
    }
}