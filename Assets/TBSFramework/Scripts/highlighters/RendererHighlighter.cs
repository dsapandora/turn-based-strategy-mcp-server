using System.Threading.Tasks;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// A highlighter that changes the color of the material for given renderer.
    /// </summary>
    public class RendererHighlighter : Highlighter
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _color;
        [SerializeField] private string _propertyName = "_Color"; // The default value for the Standard shader in the Built-in Renderer Pipeline. For the default Standart-Lit shader in the Universal Renderer Pipeline, the value is `_BaseColor`. 
        [SerializeField] private int _materialIndex = 0;

        private MaterialPropertyBlock _mpb;

        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();
            _mpb.SetColor(_propertyName, _color);
        }

        public override Task Apply(IHighlightParams @params)
        {
            _renderer.SetPropertyBlock(_mpb, _materialIndex);
            return Task.CompletedTask;
        }

        public void SetColor(Color color)
        {
            _color = color;
            _mpb.SetColor(_propertyName, _color);
        }
    }
}