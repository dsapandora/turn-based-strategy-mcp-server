using System;
using System.Linq;
using TbsFramework.EditorUtils.GridGenerators;
using TurnBasedStrategyFramework.Common.Players;
using TurnBasedStrategyFramework.Unity.Cells;
using TurnBasedStrategyFramework.Unity.Controllers;
using TurnBasedStrategyFramework.Unity.Controllers.GameResolvers;
using TurnBasedStrategyFramework.Unity.Gui;
using TurnBasedStrategyFramework.Unity.Players;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Tilemaps;

namespace TbsFramework.EditorUtils
{
    class GridHelper : EditorWindow
    {
        public bool keepMainCamera = false;

        public int nHumanPlayer = 2;
        public int nComputerPlayer = 0;

        bool is2D = false;
        GameObject cellPrefab = null;
        int mapHeight = 0;
        int mapWidth = 0;

        GameObject gridController;
        GameObject cellManager;
        GameObject unitManager;
        GameObject playerManager;
        GameObject guiController;
        GameObject directionalLight;

        BoolWrapper tileEditModeOn;
        [SerializeField] Cell tilePrefab;
        int tilePaintingRadius = 1;
        int lastPaintedHash = -1;

        BoolWrapper unitEditModeOn;
        [SerializeField] Unit unitPrefab;
        int playerNumber;

        bool gridControllerGameObjectPresent;
        bool cellManagerGameObjectPresent;
        bool unitsGameObjectPresent;
        GameObject gridControllerGameObject;
        GameObject cellManagerGameObject;
        GameObject unitsGameObject;

        BoolWrapper toToggle = null;

        private Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Window/Grid Helper")]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(GridHelper));
            window.titleContent.text = "Grid Helper";
        }

        public void OnEnable()
        {
            var gridGameObject = GameObject.Find("GridController");
            var cellManagerGameObject = GameObject.Find("CellManager");
            var unitsGameObject = GameObject.Find("UnitManager");

            gridControllerGameObjectPresent = gridGameObject != null;
            cellManagerGameObjectPresent = cellManagerGameObject != null;
            unitsGameObjectPresent = unitsGameObject != null;

            tileEditModeOn = new BoolWrapper(false);
            unitEditModeOn = new BoolWrapper(false);

            Selection.selectionChanged += OnSelectionChanged;
            Undo.undoRedoPerformed += OnUndoPerformed;
        }


        public void OnDestroy()
        {
            DisableSceneViewInteraction();
            tileEditModeOn = new BoolWrapper(false);
            unitEditModeOn = new BoolWrapper(false);

            Selection.selectionChanged -= OnSelectionChanged;
            Undo.undoRedoPerformed -= OnUndoPerformed;
        }

        void OnHierarchyChange()
        {
            var gridGameObject = GameObject.Find("GridController");
            var cellManagerGameObject = GameObject.Find("CellManager");
            var unitsGameObject = GameObject.Find("UnitManager");

            gridControllerGameObjectPresent = gridGameObject != null;
            cellManagerGameObjectPresent = cellManagerGameObject != null;
            unitsGameObjectPresent = unitsGameObject != null;

            if (unitsGameObject != null)
            {
                this.unitsGameObject = null;
            }
            if (gridGameObject != null)
            {
                this.gridControllerGameObject = null;
            }

            Repaint();
        }

        void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUIStyle.none);
            MapGenerationGUI();
            TilePaintingGUI();
            UnitPaintingGUI();
            PrefabHelperGUI();

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.R)
            {
                ToggleEditMode();
            }
            GUILayout.EndScrollView();
        }

        private void ToggleEditMode()
        {
            if (toToggle == null)
            {
                return;
            }
            toToggle.value = !toToggle.value;
            if (toToggle.value)
            {
                EnableSceneViewInteraction();
            }
            Repaint();
        }

        private void PrefabHelperGUI()
        {
            GUILayout.Label("Prefab helper", EditorStyles.boldLabel);
            GUILayout.Label("Select multiple objects in hierarchy and click button below to create multiple prefabs at once. Please note that this may take a while", EditorStyles.wordWrappedLabel);

            if (GUILayout.Button("Selection to prefabs"))
            {
                string path = EditorUtility.SaveFolderPanel("Save prefabs", "", "");
                if (path.Length != 0)
                {
                    path = path.Replace(Application.dataPath, "Assets");

                    GameObject[] objectArray = Selection.gameObjects;
                    for (int i = 0; i < objectArray.Length; i++)
                    {
                        GameObject gameObject = objectArray[i];
                        string localPath = path + "/" + gameObject.name + ".prefab";
                        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                        PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction);
                    }
                    Debug.Log(string.Format("{0} prefabs saved to {1}", objectArray.Length, path));
                }
            }

            if (GUILayout.Button("Selection to prefabs (variants)"))
            {
                string path = EditorUtility.SaveFolderPanel("Save prefabs", "", "");
                if (path.Length != 0)
                {
                    path = path.Replace(Application.dataPath, "Assets");

                    Transform[] objectArray = Selection.transforms;
                    GameObject root = objectArray[0].gameObject;

                    string localPath = path + "/" + root.name + ".prefab";
                    localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

                    var rootPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(root, localPath, InteractionMode.UserAction);

                    for (int i = 1; i < objectArray.Length; i++)
                    {
                        GameObject gameObject = objectArray[i].gameObject;
                        var rootInstance = PrefabUtility.InstantiatePrefab(rootPrefab) as GameObject;

                        foreach (var component in gameObject.GetComponents<Component>())
                        {
                            var destComponent = rootInstance.GetComponent(component.GetType());
                            if (destComponent)
                            {
                                EditorUtility.CopySerialized(component, rootInstance.GetComponent(component.GetType()));
                            }
                        }
                        localPath = path + "/" + gameObject.name + ".prefab";
                        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                        PrefabUtility.SaveAsPrefabAssetAndConnect(rootInstance, localPath, InteractionMode.UserAction);

                        DestroyImmediate(rootInstance);
                    }
                    Debug.Log(string.Format("{0} prefabs saved to {1}", objectArray.Length, path));
                }
            }
        }

        private void UnitPaintingGUI()
        {
            GUILayout.Label("Unit painting", EditorStyles.boldLabel);
            if (!unitsGameObjectPresent)
            {
                if (unitsGameObject == null)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                    style.normal.textColor = Color.red;
                    GUILayout.Label("Unit parent GameObject missing", style);
                }
                unitsGameObject = (GameObject)EditorGUILayout.ObjectField("Units parent", unitsGameObject, typeof(GameObject), true, new GUILayoutOption[0]);
            }
            unitPrefab = (Unit)EditorGUILayout.ObjectField("Unit prefab", unitPrefab, typeof(Unit), false, new GUILayoutOption[0]);

            if (unitPrefab != null
                && unitPrefab.GetComponent<Collider>() == null
                && unitPrefab.GetComponentInChildren<Collider>() == null
                && unitPrefab.GetComponent<Collider2D>() == null
                && unitPrefab.GetComponentInChildren<Collider2D>() == null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.wordWrappedLabel);
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.red;
                GUILayout.Label("Please add a collider to your unit prefab. Without the collider the scene will not be playable", style);
            }

            playerNumber = EditorGUILayout.IntField(new GUIContent("Player number"), playerNumber);
            GUILayout.Label(string.Format("Unit Edit Mode is {0}", unitEditModeOn.value ? "on" : "off"));

            if (toToggle != null && toToggle == unitEditModeOn)
            {
                GUILayout.Label("Press Ctrl + R to toggle unit painting mode on / off");
            }

            if (unitEditModeOn.value)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Enter Unit Edit Mode"))
            {
                unitEditModeOn.value = true;
                tileEditModeOn.value = false;
                toToggle = unitEditModeOn;
                EnableSceneViewInteraction();

                GameObject UnitsParent = unitsGameObjectPresent ? GameObject.Find("UnitManager") : unitsGameObject;
                if (UnitsParent == null)
                {
                    Debug.LogError("Units parent gameobject is missing, assign it in GridHelper");
                }
            }
            GUI.enabled = true;
            if (!unitEditModeOn.value)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Exit Unit Edit Mode"))
            {
                unitEditModeOn.value = false;
                DisableSceneViewInteraction();
            }
            GUI.enabled = true;
        }

        private void TilePaintingGUI()
        {
            GUILayout.Label("Tile painting", EditorStyles.boldLabel);
            if (!gridControllerGameObjectPresent)
            {
                if (gridControllerGameObject == null)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                    style.normal.textColor = Color.red;
                    GUILayout.Label("CellGrid GameObject missing", style);
                }
                gridControllerGameObject = (GameObject)EditorGUILayout.ObjectField("GridController", gridControllerGameObject, typeof(GameObject), true, new GUILayoutOption[0]);
            }
            tilePaintingRadius = EditorGUILayout.IntSlider(new GUIContent("Brush radius"), tilePaintingRadius, 1, 4);
            EditorGUI.BeginChangeCheck();
            tilePrefab = (Cell)EditorGUILayout.ObjectField("Tile prefab", tilePrefab, typeof(Cell), true, new GUILayoutOption[0]);

            if (tilePrefab != null
                && tilePrefab.GetComponent<Collider>() == null
                && tilePrefab.GetComponentInChildren<Collider>() == null
                && tilePrefab.GetComponent<Collider2D>() == null
                && tilePrefab.GetComponentInChildren<Collider2D>() == null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.wordWrappedLabel);
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.red;
                GUILayout.Label("Please add a collider to your cell prefab. Without the collider the scene will not be playable", style);
            }

            if (EditorGUI.EndChangeCheck())
            {
                lastPaintedHash = -1;
            }
            GUILayout.Label(string.Format("Tile Edit Mode is {0}", tileEditModeOn.value ? "on" : "off"));

            if (toToggle != null && toToggle == tileEditModeOn)
            {
                GUILayout.Label("Press Ctrl + R to toggle tile painting mode on / off");
            }

            if (tileEditModeOn.value)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Enter Tile Edit Mode"))
            {
                tileEditModeOn.value = true;
                unitEditModeOn.value = false;
                toToggle = tileEditModeOn;
                EnableSceneViewInteraction();

                GameObject CellGrid = gridControllerGameObjectPresent ? GameObject.Find("GridController") : gridControllerGameObject;
                if (CellGrid == null)
                {
                    Debug.LogError("CellGrid gameobject is missing, assign it in GridHelper");
                }
            }
            GUI.enabled = true;
            if (!tileEditModeOn.value)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Exit Tile Edit Mode"))
            {
                tileEditModeOn.value = false;
                DisableSceneViewInteraction();
            }
            GUI.enabled = true;
        }

        private void MapGenerationGUI()
        {
            GUILayout.Label("Grid generation", EditorStyles.boldLabel);
            GUILayout.Label("Camera", EditorStyles.boldLabel);
            GUILayout.Label("PlayerManager", EditorStyles.boldLabel);
            nHumanPlayer = EditorGUILayout.IntField(new GUIContent("Human players No"), nHumanPlayer);
            nComputerPlayer = EditorGUILayout.IntField(new GUIContent("AI players No"), nComputerPlayer);

            GUILayout.Label("Grid", EditorStyles.boldLabel);
            is2D = (bool)EditorGUILayout.Toggle("Is 2D", is2D);
            cellPrefab = (GameObject)EditorGUILayout.ObjectField("Cell Prefab", cellPrefab, typeof(GameObject), false);
            mapHeight = (int)EditorGUILayout.IntField("Map Height", mapHeight);
            mapWidth = (int)EditorGUILayout.IntField("Map Width", mapWidth);

            keepMainCamera = EditorGUILayout.Toggle(new GUIContent("Keep main camera", "Determines whether to keep the current Main Camera or create a new one"), keepMainCamera, new GUILayoutOption[0]);

            if (GUILayout.Button("Generate scene"))
            {
                Undo.ClearAll();
                GenerateBaseStructure();
            }
            if (GUILayout.Button("Clear scene"))
            {
                string dialogTitle = "Confirm delete";
                string dialogMessage = "This will delete all objects on the scene. Do you wish to continue?";
                string dialogOK = "Ok";
                string dialogCancel = "Cancel";

                bool shouldDelete = EditorUtility.DisplayDialog(dialogTitle, dialogMessage, dialogOK, dialogCancel);
                if (shouldDelete)
                {
                    Undo.ClearAll();
                    GridHelperUtils.ClearScene(false);
                }
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.R)
            {
                ToggleEditMode();
            }

            if (tileEditModeOn.value || unitEditModeOn.value)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }

            if (tileEditModeOn.value && tilePrefab != null)
            {
                PaintTiles();
            }
            if (unitEditModeOn.value && unitPrefab != null)
            {
                PaintUnits();
            }
        }

        private void PaintUnits()
        {
            GameObject UnitsParent = unitsGameObjectPresent ? GameObject.Find("UnitManager") : unitsGameObject;
            if (UnitsParent == null)
            {
                return;
            }

            var selectedCell = GetSelectedCell();
            if (selectedCell == null)
            {
                return;
            }

            Handles.color = Color.red;
            Handles.DrawWireDisc(selectedCell.transform.position, Vector3.up, (is2D ? selectedCell.CellDimensions.y : selectedCell.CellDimensions.z) / 2);
            Handles.DrawWireDisc(selectedCell.transform.position, Vector3.forward, (is2D ? selectedCell.CellDimensions.y : selectedCell.CellDimensions.z) / 2);
            HandleUtility.Repaint();
            if (Event.current.button == 0 && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown))
            {
                if (unitEditModeOn.value && selectedCell.IsTaken)
                {
                    return;
                }

                Undo.SetCurrentGroupName("Unit painting");
                int group = Undo.GetCurrentGroup();

                Undo.RecordObject(selectedCell, "Unit painting");
                var newUnit = (PrefabUtility.InstantiatePrefab(unitPrefab.gameObject) as GameObject).GetComponent<Unit>();
                newUnit.PlayerNumber = playerNumber;
                newUnit.CurrentCell = selectedCell;

                selectedCell.IsTaken = true;
                selectedCell.CurrentUnits.Add(newUnit);
                EditorUtility.SetDirty(selectedCell);

                newUnit.transform.position = selectedCell.transform.position;
                newUnit.transform.parent = UnitsParent.transform;
                newUnit.transform.rotation = selectedCell.transform.rotation;

                Undo.RegisterCreatedObjectUndo(newUnit.gameObject, "Unit painting");
            }
        }

        private void PaintTiles()
        {
            GameObject cellManager = cellManagerGameObjectPresent ? GameObject.Find("CellManager") : cellManagerGameObject;

            Cell selectedCell = GetSelectedCell();
            if (selectedCell == null)
            {
                return;
            }

            Handles.color = Color.red;
            Handles.DrawWireDisc(selectedCell.transform.position, Vector3.up, (is2D ? selectedCell.CellDimensions.y : selectedCell.CellDimensions.z) * (tilePaintingRadius - 0.5f));
            Handles.DrawWireDisc(selectedCell.transform.position, Vector3.forward, (is2D ? selectedCell.CellDimensions.y : selectedCell.CellDimensions.z) * (tilePaintingRadius - 0.5f));
            HandleUtility.Repaint();
            int selectedCellHash = selectedCell.GetHashCode();
            if (lastPaintedHash != selectedCellHash)
            {
                if (Event.current.button == 0 && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown))
                {
                    lastPaintedHash = selectedCellHash;
                    Undo.SetCurrentGroupName("Tile painting");
                    int group = Undo.GetCurrentGroup();
                    var cells = cellManager.GetComponentsInChildren<Cell>();
                    var cellsInRange = cells.Where(c => c.GetDistance(selectedCell) <= tilePaintingRadius - 1).ToList();

                    foreach (var c in cellsInRange)
                    {
                        if (tilePrefab == PrefabUtility.GetCorrespondingObjectFromSource(c))
                        {
                            continue;
                        }
                        var newCell = (PrefabUtility.InstantiatePrefab(tilePrefab.gameObject, c.transform.parent) as GameObject).GetComponent<Cell>();
                        newCell.transform.position = c.transform.position;
                        newCell.transform.SetSiblingIndex(c.transform.GetSiblingIndex());
                        newCell.name = $"{newCell.name}_{c.GridCoordinates}";
                        c.CopyFields(newCell);

                        try
                        {
                            Undo.RegisterCreatedObjectUndo(newCell.gameObject, "Tile painting");
                            Undo.DestroyObjectImmediate(c.gameObject);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(string.Format("{0} - You are probably using wrong tile prefab", e.Message));
                            DestroyImmediate(newCell.gameObject);
                        }

                    }
                    Undo.CollapseUndoOperations(group);
                    Undo.IncrementCurrentGroup();
                }
            }
        }

        private Cell GetSelectedCell()
        {
            var raycastHit2D = Physics2D.GetRayIntersection(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), Mathf.Infinity);
            if (raycastHit2D.transform != null && raycastHit2D.transform.GetComponent<Cell>() != null)
            {
                return raycastHit2D.transform.GetComponent<Cell>();
            }

            RaycastHit raycastHit3D;
            bool isRaycast3D = Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out raycastHit3D);
            if (isRaycast3D && raycastHit3D.transform.GetComponent<Cell>() != null)
            {
                return raycastHit3D.transform.GetComponent<Cell>();
            }

            return null;
        }

        void GenerateBaseStructure()
        {
            GridHelperUtils.ClearScene(keepMainCamera);

            gridController = new GameObject("GridController");
            cellManager = new GameObject("CellManager");
            cellManagerGameObject = cellManager;
            var grid = new GameObject("Grid");
            var tilemap = new GameObject("Tilemap");
            playerManager = new GameObject("PlayerManager");
            unitManager = new GameObject("UnitManager");
            guiController = new GameObject("GUIController");
            var gameEndConditions = new GameObject("GameEndConditions");

            directionalLight = new GameObject("DirectionalLight");
            var light = directionalLight.AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.Rotate(45f, 0, 0);

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();

            var cellManagerScript = cellManager.AddComponent<RegularCellManager>();
            var playerManagerScript = playerManager.AddComponent<UnityPlayerManager>();
            for (int i = 0; i < nHumanPlayer; i++)
            {
                var player = new GameObject(string.Format("Player_{0}", playerManager.transform.childCount));
                player.AddComponent<TurnBasedStrategyFramework.Unity.Players.HumanPlayer>();
                player.GetComponent<IPlayer>().PlayerNumber = playerManager.transform.childCount;
                player.transform.parent = playerManager.transform;
            }

            for (int i = 0; i < nComputerPlayer; i++)
            {
                var aiPlayer = new GameObject(string.Format("AI_Player_{0}", playerManager.transform.childCount));
                var aiPlayerScript = aiPlayer.AddComponent<AIPlayer>();
                aiPlayerScript.PlayerNumber = playerManager.transform.childCount;
                aiPlayer.transform.parent = playerManager.transform;
            }

            var unitManagerScript = unitManager.AddComponent<UnityUnitManager>();

            var gridControllerScript = gridController.AddComponent<UnityGridController>();
            var turnResolverScript = gridController.AddComponent<SubsequentTurnResolver>();
            gridControllerScript.TurnResolver = turnResolverScript;
            gridControllerScript.PlayerManager = playerManagerScript;
            gridControllerScript.UnitManager = unitManagerScript;
            gridControllerScript.CellManager = cellManagerScript;

            gridController.GetComponent<UnityGridController>().PlayerManager = playerManager.GetComponent<UnityPlayerManager>();

            var cellScript = cellPrefab.GetComponent<Cell>();
            CellShape cellShape = cellScript.CellShape;

            grid.AddComponent<Grid>();
            grid.GetComponent<Grid>().cellLayout = cellShape == CellShape.Square ? GridLayout.CellLayout.Rectangle : GridLayout.CellLayout.Hexagon;
            grid.GetComponent<Grid>().cellSwizzle = cellShape == CellShape.Square
                ? (is2D ? GridLayout.CellSwizzle.XYZ : GridLayout.CellSwizzle.XZY)
                : (is2D ? GridLayout.CellSwizzle.YXZ : GridLayout.CellSwizzle.YZX);
            grid.GetComponent<Grid>().cellSize = cellScript.CellDimensions;
            tilemap.AddComponent<Tilemap>();
            tilemap.GetComponent<Tilemap>().orientation = Tilemap.Orientation.XZ;
            tilemap.transform.parent = grid.transform;

            var guiControllerScript = guiController.AddComponent<GUIController>();
            guiControllerScript.SetGridController(gridControllerScript);

            var dominationCondition = new GameObject("DominationCondition");
            var dominationConditionScript = dominationCondition.AddComponent<DominationVictoryCondition>();
            dominationConditionScript.SetUnitManager(unitManagerScript);
            dominationConditionScript.SetPlayerManager(playerManagerScript);
            dominationConditionScript.SetGridController(gridControllerScript);
            dominationCondition.transform.parent = gameEndConditions.transform;

            ICellGridGenerator generator = null;

            if(cellShape.Equals(CellShape.Square))
            {
                var squareGenerator = new RectangularSquareGridGenerator();
                squareGenerator.Width = mapWidth;
                squareGenerator.Height = mapHeight;
                squareGenerator.SquarePrefab = cellPrefab;

                generator = squareGenerator;
            }
            else
            {
                var hexGenerator = new RectangularHexGridGenerator();
                hexGenerator.Width = mapWidth;
                hexGenerator.Height = mapHeight;
                hexGenerator.HexagonPrefab = cellPrefab;

                generator = hexGenerator;
            }

            generator.CellsParent = cellManager.transform;
            generator.Is2D = is2D;

            GridInfo gridInfo = generator.GenerateGrid();

            var camera = Camera.main;
            if (camera == null || !keepMainCamera)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
                camera.transform.position = gridInfo.Center;

                float zoomOutFactor = 1.25f;
                if (is2D)
                {
                    float distance = (Mathf.Max(gridInfo.Dimensions.x, gridInfo.Dimensions.y) * Mathf.Sqrt(3) / 2) * zoomOutFactor;
                    camera.transform.position += new Vector3(0, 0, -distance);
                    cameraObject.AddComponent<Physics2DRaycaster>();
                }
                else
                {
                    float distance = (Mathf.Max(gridInfo.Dimensions.x, gridInfo.Dimensions.z) * Mathf.Sqrt(3) / 2) * zoomOutFactor;
                    camera.transform.position += new Vector3(0, distance, 0);
                    cameraObject.AddComponent<PhysicsRaycaster>();
                    camera.transform.Rotate(90, 0, 0);
                }
                camera.transform.SetAsFirstSibling();
            }
        }

        void EnableSceneViewInteraction()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        void DisableSceneViewInteraction()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSelectionChanged()
        {
            if (Selection.activeGameObject == null)
            {
                return;
            }

            if (PrefabUtility.GetPrefabAssetType(Selection.activeGameObject) != PrefabAssetType.NotAPrefab)
            {
                if (tileEditModeOn.value || toToggle == tileEditModeOn)
                {
                    if (Selection.activeGameObject.GetComponent<Cell>() != null)
                    {
                        lastPaintedHash = -1;
                        if (PrefabUtility.GetPrefabInstanceStatus(Selection.activeGameObject) == PrefabInstanceStatus.Connected)
                        {
                            tilePrefab = PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject).GetComponent<Cell>();
                        }
                        else
                        {
                            tilePrefab = Selection.activeGameObject.GetComponent<Cell>();
                        }
                        Repaint();
                    }
                }

                else if (unitEditModeOn.value || toToggle == unitEditModeOn)
                {
                    if (Selection.activeGameObject.GetComponent<Unit>() != null)
                    {
                        if (PrefabUtility.GetPrefabInstanceStatus(Selection.activeGameObject) == PrefabInstanceStatus.Connected)
                        {
                            unitPrefab = PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject).GetComponent<Unit>();
                        }
                        else
                        {
                            unitPrefab = Selection.activeGameObject.GetComponent<Unit>();
                        }
                        Repaint();
                    }
                }
            }
        }

        private void OnUndoPerformed()
        {
            lastPaintedHash = -1;
        }

        internal class BoolWrapper
        {
            public bool value;
            public BoolWrapper(bool value)
            {
                this.value = value;
            }
        }
    }
}