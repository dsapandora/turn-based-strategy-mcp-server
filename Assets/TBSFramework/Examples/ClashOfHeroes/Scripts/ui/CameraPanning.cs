using UnityEngine;
using UnityEngine.InputSystem;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.UI
{
    /// <summary>
    /// Handles camera panning via mouse drag, with configurable limits and sensitivity.
    /// </summary>
    public class CameraPanning : MonoBehaviour
    {
        [Header("Pan Settings")]
        [Tooltip("How far the camera can move from its original position.")]
        public float maxOffset = 2.0f;

        [Tooltip("Controls how quickly the camera resists the drag.")]
        public float scrollResistance = 0.5f;

        [Tooltip("How quickly the camera returns to center after mouse release.")]
        public float returnSpeed = 2.0f;

        [Header("Sensitivity Settings")]
        [Tooltip("Minimum drag distance (in screen pixels) before starting to pan.")]
        public float dragThreshold = 10f;

        [Tooltip("A multiplier to scale the mouse/touch delta. <1 means less sensitive.")]
        public float sensitivity = 0.5f;

        private Vector3 _originalPosition;
        private Vector3 _currentOffset;
        private bool _isDragging = false;
        private Vector3 _mouseDownPos;
        private bool _dragConfirmed = false;

        void Start()
        {
            _originalPosition = transform.position;
        }

        void Update()
        {
            HandleInput();

            if (!_isDragging)
            {
                _currentOffset = Vector3.Lerp(_currentOffset, Vector3.zero, Time.deltaTime * returnSpeed);
                transform.position = _originalPosition + _currentOffset;
            }
        }

        void HandleInput()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _isDragging = true;
                _dragConfirmed = false;
                _mouseDownPos = Mouse.current.position.ReadValue();
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                _isDragging = false;
                _dragConfirmed = false;
            }

            if (_isDragging)
            {
                Vector3 currentMousePos = Mouse.current.position.ReadValue();
                Vector3 mouseDelta = currentMousePos - _mouseDownPos;
                float distance = mouseDelta.magnitude;

                if (!_dragConfirmed && distance > dragThreshold)
                {
                    _dragConfirmed = true;
                }

                if (_dragConfirmed)
                {
                    mouseDelta *= sensitivity;

                    float effectiveStrength = 1f / (1f + distance * scrollResistance);
                    Vector3 desiredOffset = mouseDelta.normalized * Mathf.Min(distance * effectiveStrength, maxOffset);

                    _currentOffset = Vector3.Lerp(_currentOffset, desiredOffset, Time.deltaTime * 10f);
                    transform.position = _originalPosition + _currentOffset;
                }
            }
        }
    }
}