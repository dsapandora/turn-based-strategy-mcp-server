using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Utilities;
using TurnBasedStrategyFramework.Unity.Cells;
using UnityEditor;
using UnityEngine;

namespace TbsFramework.EditorUtils.GridGenerators
{
    /// <summary>
    /// Generates rectangular shaped grid of hexagons.
    /// </summary>
    [ExecuteInEditMode()]
    class RectangularHexGridGenerator : ICellGridGenerator
    {
#pragma warning disable 0649
        public GameObject HexagonPrefab;
        public int Width;
        public int Height;
#pragma warning restore 0649

        public override GridInfo GenerateGrid()
        {
            HexGridType hexGridType = Width % 2 == 0 ? HexGridType.even_q : HexGridType.odd_q;
            List<Cell> hexagons = new List<Cell>();

            if (HexagonPrefab.GetComponent<Hexagon>() == null)
            {
                Debug.LogError("Invalid hexagon prefab provided");
                return null;
            }

            var hexSize = HexagonPrefab.GetComponent<Cell>().CellDimensions;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    var hexagon = PrefabUtility.InstantiatePrefab(HexagonPrefab) as GameObject;
                    var position = Is2D ? new Vector3((j * hexSize.x * 0.75f), (i * hexSize.y) + (j % 2 == 0 ? 0 : hexSize.y * 0.5f)) :
                        new Vector3((j * hexSize.x * 0.75f), 0, (i * hexSize.z) + (j % 2 == 0 ? 0 : hexSize.z * 0.5f));

                    hexagon.transform.position = position;
                    var gridCoordinates = new Vector2IntImpl(Width - j - 1, Height - i - 1);
                    hexagon.GetComponent<Hexagon>().GridCoordinates = gridCoordinates;
                    hexagon.name = $"{hexagon.name}_{gridCoordinates}";
                    hexagon.GetComponent<Hexagon>().GridType = hexGridType;
                    hexagon.GetComponent<Hexagon>().MovementCost = 1;
                    hexagons.Add(hexagon.GetComponent<Cell>());

                    hexagon.transform.parent = CellsParent;
                }
            }

            return GetGridInfo(hexagons);
        }
    }
}