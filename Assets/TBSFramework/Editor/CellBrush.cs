using System.Linq;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Utilities;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEditor;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Editor
{
    [ExecuteInEditMode]
    [CustomGridBrush(true, false, false, "Cell Brush")]
    public class CellBrush : GridBrushBase
    {
        [SerializeField] GameObject _cellPrefab;
        [SerializeField] Transform _cellsParent;
        [SerializeField] bool _is2D;
        [SerializeField] Vector3 _offset = new Vector3(0, 0, 0);

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            var gridPosition = new Vector2IntImpl(position.x, position.y);

            if (!_cellsParent)
            {
                Debug.LogWarning("No 'CellManager' found in the scene and no 'CellsParent' assigned. Cannot paint.");
                return;
            }

            var existingCell = GetObjectInCell(gridLayout, _cellsParent, position);
            var cellGO = PrefabUtility.InstantiatePrefab(_cellPrefab, _cellsParent) as GameObject;
            cellGO.name = $"{cellGO.name}_{gridPosition}";
            if (!cellGO)
            {
                Debug.LogError("Failed to instantiate cell prefab.");
                return;
            }

            Undo.RegisterCreatedObjectUndo(cellGO, "Cell Brush: Create Cell");
            if (existingCell)
            {
                var newCell = cellGO.GetComponent<ICell>();
                var oldCell = existingCell.GetComponent<ICell>();

                var siblingIndex = existingCell.transform.GetSiblingIndex();
                cellGO.transform.SetSiblingIndex(siblingIndex);

                if (oldCell.CurrentUnits.Any())
                {
                    newCell.IsTaken = oldCell.IsTaken;
                    foreach (var unit in oldCell.CurrentUnits)
                    {
                        newCell.CurrentUnits.Add(unit);
                        unit.CurrentCell = newCell;
                        EditorUtility.SetDirty((Unit)unit);
                    }
                }

                EditorUtility.SetDirty(cellGO);
                Undo.DestroyObjectImmediate(existingCell);
            }

            var worldPosition = gridLayout.CellToWorld(position);
            if (_is2D)
            {
                cellGO.transform.SetPositionAndRotation(
                    new Vector3(worldPosition.x + _offset.x, worldPosition.y + _offset.y, _cellPrefab.transform.position.z + _offset.z),
                    Quaternion.identity
                );
            }
            else
            {
                cellGO.transform.SetPositionAndRotation(
                    new Vector3(worldPosition.x + _offset.x, _cellPrefab.transform.position.y + _offset.y, worldPosition.z + _offset.z),
                    Quaternion.identity
                );
            }

            if (cellGO.TryGetComponent<ICell>(out var cellComponent))
            {
                cellComponent.GridCoordinates = gridPosition;
            }
            else
            {
                Debug.LogWarning("Instantiated cell prefab does not have an ICell component attached!");
            }
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (!_cellsParent)
                return;

            var existing = GetObjectInCell(gridLayout, _cellsParent, position);
            if (!existing)
                return;

            if (existing.TryGetComponent<ICell>(out var cell))
            {
                foreach (var unit in cell.CurrentUnits)
                {
                    Undo.DestroyObjectImmediate(((Unit)unit).gameObject);
                }

                cell.CurrentUnits.Clear();
            }

            Undo.DestroyObjectImmediate(existing);
        }

        GameObject GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                var childPos = grid.WorldToCell(new Vector3(child.position.x, child.position.y, child.position.z));
                if (childPos.x == position.x && childPos.y == position.y && childPos.z == position.z)
                {
                    return child.gameObject;
                }
            }
            return null;
        }

        [CustomEditor(typeof(CellBrush))]
        public class CellBrushEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                var cellPrefabProperty = serializedObject.FindProperty("_cellPrefab");
                var cellOffsetProperty = serializedObject.FindProperty("_offset");
                var cellsParentProperty = serializedObject.FindProperty("_cellsParent");
                var is2DMapProperty = serializedObject.FindProperty("_is2D");

                var cellManager = GameObject.Find("CellManager");
                bool noValidParent = !cellManager && !cellsParentProperty.objectReferenceValue;
                if (noValidParent)
                {
                    EditorGUILayout.HelpBox(
                        "Warning: No 'CellManager' found in the scene, and no 'Cells Parent' assigned. This brush cannot function without a valid parent!",
                        MessageType.Warning
                    );
                }

                if (!cellManager)
                {
                    EditorGUILayout.PropertyField(cellsParentProperty, new GUIContent("Cells Parent"));
                }
                else
                {
                    ((CellBrush)target)._cellsParent = cellManager.transform;
                }

                EditorGUILayout.PropertyField(cellOffsetProperty, new GUIContent("Offset"));
                EditorGUILayout.PropertyField(is2DMapProperty, new GUIContent("Is 2D Map"));
                EditorGUILayout.PropertyField(cellPrefabProperty, new GUIContent("Cell Prefab"));
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
