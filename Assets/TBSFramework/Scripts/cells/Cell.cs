using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Utilities;
using TurnBasedStrategyFramework.Unity.Highlighters;
using TurnBasedStrategyFramework.Unity.Units;
using TurnBasedStrategyFramework.Unity.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TurnBasedStrategyFramework.Unity.Cells
{
    /// <summary>
    /// A Game Object representing a cell in the scene. 
    /// It manages cell state, highlighting, click events, and visual indicators for grid interactions.
    /// </summary>
    public abstract class Cell : MonoBehaviour, ICell, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Vector2Int _gridCoordinates;
        [SerializeField] private bool _isTaken;

        private bool _positionCacheInitialized;
        private Vector2IntImpl _gridCoordinatesCache;

        public event Action<ICell> CellClicked;
        public event Action<ICell> CellHighlighted;
        public event Action<ICell> CellDehighlighted;

        [SerializeField] private List<Highlighter> _unMarkFn;
        [SerializeField] private List<Highlighter> _markAsHighlightedFn;
        [SerializeField] private List<Highlighter> _markAsReachableFn;
        [SerializeField] private List<Highlighter> _markAsPathFn;
        [SerializeField] private float _movementCost = 1;

        private int _hashCache = -1;

        public abstract CellShape CellShape { get; protected set; }

        public Vector2IntImpl GridCoordinates { get { if(!_positionCacheInitialized) { _gridCoordinatesCache = _gridCoordinates.ToIVector2Int(); _positionCacheInitialized = true; } return _gridCoordinatesCache; } set { _gridCoordinates = value.ToVector2Int(); _gridCoordinatesCache = value; } }
        public Vector3Impl WorldPosition { get { return transform.position.ToIVector3(); } set { transform.position = value.ToVector3(); } }
        public bool IsTaken { get { return _isTaken; } set { _isTaken = value; } }
        public float MovementCost { get { return _movementCost; } set { _movementCost = value; } }

        [SerializeField] private List<Unit> _currentUnits = new List<Unit>();
        private UnitListWrapper _unitListWrapper;
        public IList<IUnit> CurrentUnits
        {
            get
            {
                if (_unitListWrapper == null)
                {
                    _unitListWrapper = new UnitListWrapper(_currentUnits);
                }
                return _unitListWrapper;
            }
        }

        public abstract Vector3 CellDimensions { get; protected set; }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            CellClicked?.Invoke(this);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            CellHighlighted?.Invoke(this);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            CellDehighlighted?.Invoke(this);
        }

        public abstract IEnumerable<ICell> GetNeighbours(ICellManager cellManager);

        public abstract int GetDistance(ICell otherCell);

        public virtual async Task UnMark()
        {
            foreach (var fn in _unMarkFn)
            {
                await fn.Apply(NoParam.Instance);
            }
        }

        public virtual async Task MarkAsHighlighted()
        {
            foreach (var fn in _markAsHighlightedFn)
            {
                await fn.Apply(NoParam.Instance);
            }
        }

        public virtual async Task MarkAsReachable()
        {
            foreach (var fn in _markAsReachableFn)
            {
                await fn.Apply(NoParam.Instance);
            }
        }

        public virtual async Task MarkAsPath(IList<ICell> path, int cellIndex, ICell originCell)
        {
            foreach (var fn in _markAsPathFn)
            {
                await fn.Apply(new PathHighlightParams(path, cellIndex, originCell));
            }
        }

        /// <summary>
        /// Changes the color of the cell to given value. Used for AI debugging.
        /// </summary>
        public virtual void SetColor(float r, float g, float b, float a) { }

        public virtual bool Equals(ICell other)
        {
            return CellHelper.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return CellHelper.Equals(this, obj);
        }

        public override int GetHashCode()
        {
            if(_hashCache == -1)
            {
                _hashCache = CellHelper.GetHashCode(this);
            }
            return _hashCache;
        }
        public override string ToString()
        {
            return $"{GridCoordinates}";
        }

        public void InvokeCellHighlighted()
        {
            CellHighlighted?.Invoke(this);
        }

        public void InvokeCellDehighlighted()
        {
            CellDehighlighted?.Invoke(this);
        }

        public void InvokeCellClicked()
        {
            CellClicked?.Invoke(this);
        }

        /// <summary>
        /// Method for cloning field values into a new cell. Used in Tile Painter in Grid Helper
        /// </summary>
        /// <param name="newCell">Cell to copy field values to</param>
        public virtual void CopyFields(ICell newCell)
        {
            newCell.GridCoordinates = GridCoordinates;
        }
    }

    /// <summary>
    /// Stores parameters for highlighting a cell as part of a movement path.
    /// </summary>
    public readonly struct PathHighlightParams : IHighlightParams
    {
        /// <summary>
        /// The path that the cell is part of.
        /// </summary>
        public readonly IList<ICell> Path;

        /// <summary>
        /// The index of the cell within the path.
        /// </summary>
        public readonly int CellIndex;

        /// <summary>
        /// The origin cell from which the path begins.
        /// </summary>
        public readonly ICell OriginCell;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathHighlightParams"/> struct with the specified path, cell index, and origin cell.
        /// </summary>
        /// <param name="path">The path the cell is part of.</param>
        /// <param name="cellIndex">The index of the cell within the path.</param>
        /// <param name="originCell">The origin cell from which the path starts.</param>
        public PathHighlightParams(IList<ICell> path, int cellIndex, ICell originCell) : this()
        {
            Path = path;
            CellIndex = cellIndex;
            OriginCell = originCell;
        }
    }

    public enum CellShape
    {
        Square,
        Hexagon
    }

    /// <summary>
    /// Wraps a List<Unit> to expose it as IList<IUnit>, allowing Unity to serialize the list
    /// while adhering to the ICell interface.
    /// </summary>
    public class UnitListWrapper : IList<IUnit>
    {
        private readonly List<Unit> _units;

        public UnitListWrapper(List<Unit> units)
        {
            _units = units;
        }

        public IUnit this[int index]
        {
            get => _units[index];
            set => _units[index] = (Unit)value;
        }

        public int Count => _units.Count;

        public bool IsReadOnly => false;

        public void Add(IUnit item)
        {
            _units.Add((Unit)item);
        }

        public void Clear()
        {
            _units.Clear();
        }

        public bool Contains(IUnit item)
        {
            return _units.Contains((Unit)item);
        }

        public void CopyTo(IUnit[] array, int arrayIndex)
        {
            for (int i = 0; i < _units.Count; i++)
            {
                array[arrayIndex + i] = _units[i];
            }
        }

        public IEnumerator<IUnit> GetEnumerator()
        {
            return _units.Cast<IUnit>().GetEnumerator();
        }

        public int IndexOf(IUnit item)
        {
            return _units.IndexOf((Unit)item);
        }

        public void Insert(int index, IUnit item)
        {
            _units.Insert(index, (Unit)item);
        }

        public bool Remove(IUnit item)
        {
            return _units.Remove((Unit)item);
        }
        public void RemoveAt(int index)
        {
            _units.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _units.GetEnumerator();
        }
    }
}


