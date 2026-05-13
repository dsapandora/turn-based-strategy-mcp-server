//MIT License

//Copyright (c) 2021 FunnyCode

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Utils.Editor.Gizmos
{
    public static class GridCoordinatesGizmo
    {
        [DrawGizmo(GizmoType.Selected)]
        public static void DrawGizmo(Grid grid, GizmoType gizmoType)
        {
            Draw(SceneView.currentDrawingSceneView, grid);
        }

        [DrawGizmo(GizmoType.Selected)]
        public static void DrawGizmo(Tilemap tilemap, GizmoType gizmoType)
        {
            Draw(SceneView.currentDrawingSceneView, tilemap.layoutGrid);
        }

        private static void Draw(SceneView sceneView, Grid grid)
        {
            var camera = sceneView.camera;
            if (camera == null) return;

            float cellSizePixels = GetCellSizeInScreenPixels(grid, camera);
            const float minCellSizeForLabels = 180f;
            if (cellSizePixels < minCellSizeForLabels)
                return;

            Vector3 bottomLeftWorld = camera.ViewportToWorldPoint(Vector3.zero);
            Vector3 topRightWorld = camera.ViewportToWorldPoint(Vector3.one);

            var labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            var tmpTextSize = labelStyle.CalcSize(new GUIContent("0"));
            float textHeightWorld = GetVerticalScreenToWorldSize(camera, tmpTextSize.y);
            float offset = textHeightWorld * 2;

            GetGridBounds(grid, bottomLeftWorld, topRightWorld, out Vector3Int minCell, out Vector3Int maxCell);

            IEnumerable<Vector3Int> cellsToDraw = grid.cellLayout switch
            {
                GridLayout.CellLayout.Hexagon => GetHexCellsInBounds(minCell, maxCell),
                _ => GetRectCellsInBounds(minCell, maxCell),
            };

            foreach (var cell in cellsToDraw)
            {
                Vector3 cellCenter = grid.GetCellCenterWorld(cell);
                Vector3 labelPos = cellCenter + new Vector3(0, offset * 0.5f, 0);

                string label = $"{cell.x} ; {cell.y}";
                Handles.Label(labelPos, label, labelStyle);
            }
        }

        private static void GetGridBounds(Grid grid, Vector3 bottomLeftWorld, Vector3 topRightWorld,
            out Vector3Int minCell, out Vector3Int maxCell)
        {
            Vector3Int cellA = grid.WorldToCell(bottomLeftWorld);
            Vector3Int cellB = grid.WorldToCell(topRightWorld);

            minCell = new Vector3Int(
                Mathf.Min(cellA.x, cellB.x),
                Mathf.Min(cellA.y, cellB.y),
                0);

            maxCell = new Vector3Int(
                Mathf.Max(cellA.x, cellB.x),
                Mathf.Max(cellA.y, cellB.y),
                0);
        }

        private static IEnumerable<Vector3Int> GetRectCellsInBounds(Vector3Int min, Vector3Int max)
        {
            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    yield return new Vector3Int(x, y, 0);
                }
            }
        }

        private static IEnumerable<Vector3Int> GetHexCellsInBounds(Vector3Int min, Vector3Int max)
        {
            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    yield return new Vector3Int(x, y, 0);
                }
            }
        }

        private static float GetVerticalScreenToWorldSize(Camera camera, float size)
        {
            float yZeroWorld = camera.ScreenToWorldPoint(Vector3.zero).y;
            float yUpWorld = camera.ScreenToWorldPoint(new Vector3(0, size, 0)).y;
            return yUpWorld - yZeroWorld;
        }

        private static float GetCellSizeInScreenPixels(Grid grid, Camera camera)
        {
            Vector3 cellWorldSize = grid.cellSize;

            Vector3 cellOriginScreen = camera.WorldToScreenPoint(Vector3.zero);
            Vector3 cellOffsetScreen = camera.WorldToScreenPoint(cellWorldSize);

            float cellSizeInPixels = Vector2.Distance(
                new Vector2(cellOriginScreen.x, cellOriginScreen.y),
                new Vector2(cellOffsetScreen.x, cellOffsetScreen.y));

            return cellSizeInPixels;
        }
    }
}
