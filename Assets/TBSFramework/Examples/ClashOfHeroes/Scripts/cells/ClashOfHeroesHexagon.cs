using TurnBasedStrategyFramework.Unity.Cells;
using TurnBasedStrategyFramework.Unity.Highlighters;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Cells
{
    /// <summary>
    /// Represents a hexagonal cell in the Clash of Heroes demo.
    /// </summary>
    public class ClashOfHeroesHexagon : Hexagon, IHeightComponent, ITypedCell
    {
        [SerializeField] private int _height;
        [SerializeField] private ScriptableObject _cellType;

        [SerializeField] private SpriteRendererHighlighter _highlighter; // Highlighter for AI debugging

        public int Height 
        { 
            get { return _height; } 
            set 
            { 
                _height = value; 
            } 
        }

        public ScriptableObject CellType
        {
            get { return _cellType; }
            set
            {
                _cellType = value;
            }
        }

        public override void SetColor(float r, float g, float b, float a)
        {
            _highlighter.SetColor(new Color(r, g, b, a));
            _highlighter.Apply(NoParam.Instance);
        }
    }
}