using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.UI
{
    /// <summary>
    /// Stores camera settings, including position, rotation, and field of view.
    /// This class is used to configure camera properties based on different aspect ratios.
    /// </summary>
    public class CameraSettings : MonoBehaviour
    {
        [SerializeField] private Vector3 position;
        [SerializeField] private Vector3 rotation;
        [SerializeField] private int fov;

        public Vector3 Position { get => position; set => position = value; }
        public Vector3 Rotation { get => rotation; set => rotation = value; }
        public int FOV { get => fov; set => fov = value; }
    }
}