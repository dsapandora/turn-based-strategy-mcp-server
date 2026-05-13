using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.Cells;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEditor;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Editor
{
    [CustomGridBrush(true, false, false, "Unit Brush")]
    public class UnitBrush : GridBrushBase
    {
        [SerializeField] GameObject _unitPrefab;
        [SerializeField] Transform _unitsParent;
        [SerializeField] int _playerNumber;
        [SerializeField] Vector3 _offset;

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (!_unitsParent)
            {
                Debug.LogWarning("No 'UnitManager' found in the scene and no 'UnitsParent' assigned. Cannot paint.");
                return;
            }

            var selCell = GetSelectedCell();
            if (!selCell)
            {
                Debug.LogWarning("No cell under cursor. Unit will not be painted.");
                return;
            }

            var worldPos = gridLayout.CellToWorld(position);
            var unitGO = PrefabUtility.InstantiatePrefab(_unitPrefab, _unitsParent) as GameObject;
            if (!unitGO)
            {
                Debug.LogError("Failed to instantiate unit prefab.");
                return;
            }

            Undo.RegisterCreatedObjectUndo(unitGO, "Unit Brush: Create Unit");
            unitGO.transform.SetPositionAndRotation(worldPos + _offset, Quaternion.identity);

            selCell.IsTaken = true;
            selCell.CurrentUnits.Add(unitGO.GetComponent<Unit>());
            EditorUtility.SetDirty(selCell);

            unitGO.GetComponent<Unit>().CurrentCell = selCell;
            unitGO.GetComponent<IUnit>().PlayerNumber = _playerNumber;
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (!_unitsParent)
                return;

            var selUnit = GetSelectedUnit();
            if (!selUnit)
                return;

            selUnit.CurrentCell.CurrentUnits.Remove(selUnit);
            selUnit.CurrentCell.IsTaken = false;

            EditorUtility.SetDirty((Cell)selUnit.CurrentCell);
            Undo.DestroyObjectImmediate(selUnit.gameObject);
        }

        Unit GetSelectedUnit()
        {
            var hit2D = Physics2D.GetRayIntersection(
                HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), Mathf.Infinity
            );
            if (hit2D.transform && hit2D.transform.GetComponent<Unit>())
            {
                return hit2D.transform.GetComponent<Unit>();
            }

            if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out var hit3D))
            {
                Debug.Log(hit3D.transform.gameObject.ToString());
                var unit3D = hit3D.transform.GetComponentInChildren<Unit>();
                if (unit3D) return unit3D;
            }
            return null;
        }

        Cell GetSelectedCell()
        {
            var hit2D = Physics2D.GetRayIntersection(
                HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), Mathf.Infinity
            );
            if (hit2D.transform && hit2D.transform.GetComponent<Cell>())
            {
                return hit2D.transform.GetComponent<Cell>();
            }

            if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out var hit3D))
            {
                var cell3D = hit3D.transform.GetComponentInChildren<Cell>();
                if (cell3D) return cell3D;
            }
            return null;
        }

        [CustomEditor(typeof(UnitBrush))]
        public class UnitBrushEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                var unitPrefabProp = serializedObject.FindProperty("_unitPrefab");
                var unitsParentProp = serializedObject.FindProperty("_unitsParent");
                var playerNumProp = serializedObject.FindProperty("_playerNumber");
                var cellOffsetProperty = serializedObject.FindProperty("_offset");

                var unitManager = GameObject.Find("UnitManager");
                bool noValidParent = !unitManager && !unitsParentProp.objectReferenceValue;
                if (noValidParent)
                {
                    EditorGUILayout.HelpBox(
                        "Warning: No 'UnitManager' found in the scene, and no 'Units Parent' assigned. This brush cannot function without a valid parent!",
                        MessageType.Warning
                    );
                }

                if (!unitManager)
                {
                    EditorGUILayout.PropertyField(unitsParentProp, new GUIContent("Units Parent"));
                }
                else
                {
                    ((UnitBrush)target)._unitsParent = unitManager.transform;
                }

                EditorGUILayout.PropertyField(unitPrefabProp, new GUIContent("Unit Prefab"));
                EditorGUILayout.PropertyField(playerNumProp, new GUIContent("Player Number"));
                EditorGUILayout.PropertyField(cellOffsetProperty, new GUIContent("Offset"));

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
