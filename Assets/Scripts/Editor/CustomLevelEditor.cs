using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Constants;
using Cysharp.Threading.Tasks;
using Gameplay;
using Gameplay.Levels;
using Generator;
using Generator.GenerationStrategies.Base;
using Generator.GenerationStrategies.Implementations;
using ModestTree;
using UnityEngine;
using UnityEngine.Networking;

namespace Editor
{
    using UnityEditor;
    
    [CustomEditor(typeof(Level))]
    public class CustomLevelEditor : Editor
    {
        public enum DrawMode
        {
            Draw,
            Erase
        }
        
        private const string GeneratorConfigName = "<GeneratorConfig>k__BackingField";
        private const string ShapePropertyName = "<ShapeStrategy>k__BackingField";
        private const string PositionsPropertyName = "_positions";
        private const float LayerOffset = 0.16f;
        private const float GridYOffset = 0.07f;
        private const int GridMargin = 1;

        private List<TileMock> _tiles = new();
        private readonly Color _blockedColor = Color.gray;
        private readonly Vector2 GridSize = new(0.6f, 0.6f);
        
        [SerializeField] private TileMock _tileMock;

        private Level _config;
        private Transform _rootTransform;
        private TileMap _tileMap;

        private CancellationTokenSource _cancellationTokenSource = new();
        
        private DrawMode _drawMode;
        private bool _isGenerationProcessing;
        private bool _isEditing;
        private bool _drawGrid = true;
        private string _solvableMessage;
        
        private CustomStrategy CustomShapeConfig => _config.GeneratorConfig.ShapeStrategy as CustomStrategy;
        private Vector2 Size => ShapeStrategy.Size;
        private string RootName => $"[CustomStrategyEditor] {_config.name}]";

        private GenerationStrategy ShapeStrategy => _config.GeneratorConfig.ShapeStrategy;
        private Vector3 Origin => - Size * GridSize / 2f;

        private void OnEnable()
        {
            _drawGrid = EditorPrefs.GetBool(nameof(_drawGrid), true);
            _prompt = EditorPrefs.GetString(nameof(_prompt));
            _model = EditorPrefs.GetString(nameof(_model));
            
            int rx = EditorPrefs.GetInt(nameof(_minMaxTilesCount) + "X", 0);
            int ry = EditorPrefs.GetInt(nameof(_minMaxTilesCount) + "Y", rx + 100);
            _minMaxTilesCount = new Vector2Int(rx, ry);
            
            _config = (Level)target;

            if (CustomShapeConfig == null)
                return;
            
            SceneView.duringSceneGui += OnSceneGUI;
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnDisable()
        {
            Clear();
            _isEditing = false;
            _isGenerationProcessing = false;
            _cancellationTokenSource.Cancel();
            SceneView.duringSceneGui -= OnSceneGUI;
            Undo.undoRedoPerformed -= OnUndoRedo;
        }
        
        private void OnUndoRedo()
        {
            if (_isEditing)
            {
                Clear();
                CreateRoot();
                LoadExistingTiles();
            }
        }

        private void CreateRoot()
        {
            _rootTransform = new GameObject(RootName).transform;
            _rootTransform.gameObject.hideFlags = HideFlags.DontSave;
        }

        private void Clear()
        {
            if(_rootTransform != null)
            {
                DestroyImmediate(_rootTransform.gameObject);
                _rootTransform = null;
            }
            _tiles.Clear();
            _tileMap?.Dispose();
            _tileMap = null;
        }

        public override void OnInspectorGUI()
        {
            if (CustomShapeConfig == null)
            {
                DrawDefaultInspector();
                return;
            }

            serializedObject.Update();
            DrawDefaultInspector();

            var listVectors = GetSerializedPositionsProperty();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(listVectors, true);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(15);

            bool isSolvable = CustomShapeConfig.IsSolvable(out _solvableMessage);
            EditorGUILayout.HelpBox(_solvableMessage, isSolvable ? MessageType.Info : MessageType.Error);
            
            GUI.backgroundColor = _isEditing ? Color.green : Color.white;
            string buttonText = _isEditing ? "Exit From Edit Mode" : "Enter Edit Mode";

            if (GUILayout.Button(buttonText, GUILayout.Height(40)))
            {
                _isEditing = !_isEditing;

                if (_isEditing)
                {
                    CreateRoot();
                    LoadExistingTiles();
                }
                else
                {
                    Clear();
                }

                SceneView.RepaintAll();
            }

            GUI.backgroundColor = Color.white;

            if (_isEditing)
            {
                EditorGUI.BeginChangeCheck();
                _drawGrid = GUILayout.Toggle(_drawGrid, "Show Grid", "Button");

                var names = Enum.GetNames(typeof(DrawMode));
                _drawMode = (DrawMode)GUILayout.Toolbar((int)_drawMode, names);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool(nameof(_drawGrid), _drawGrid);
                    SceneView.RepaintAll();
                }
            }

            if (_isEditing)
                EditorGUILayout.HelpBox("Edit mode is active!\nClick in " +
                                        "Scene window to place a tile", MessageType.Info);

            if (GUILayout.Button("Clear", GUILayout.Height(30)))
            {
                if (_rootTransform != null)
                    for (int i = _rootTransform.childCount - 1; i >= 0; i--)
                        DestroyImmediate(_rootTransform.GetChild(i).gameObject);

                _tiles.Clear();
                _tileMap?.Clear();
                listVectors.ClearArray();
                SceneView.RepaintAll();
            }

            GUILayout.Space(50);
            GUILayout.Label("AI");
            
            if (GUILayout.Button("RequestNewAILayout", GUILayout.Height(30)))
            {
                if (_isGenerationProcessing is false)
                    RequestNewLayoutFromAI().Forget();
            }

            EditorGUI.BeginChangeCheck();

            GUILayout.Label("Prompt");
            _prompt = GUILayout.TextArea(_prompt, GUILayout.MinHeight(60), GUILayout.ExpandHeight(true));

            int index = Models.IndexOf(_model);

            if (index == -1)
                index = 0;

            index = EditorGUILayout.Popup(index, Models);
            _model = Models[index];

            _minMaxTilesCount = EditorGUILayout.Vector2IntField("MinMaxTilesCount", _minMaxTilesCount);

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(nameof(_prompt), _prompt);
                EditorPrefs.SetString(nameof(_model), _model);
                EditorPrefs.SetInt(nameof(_minMaxTilesCount) + "X", _minMaxTilesCount.x);
                EditorPrefs.SetInt(nameof(_minMaxTilesCount) + "Y", _minMaxTilesCount.y);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private SerializedProperty GetSerializedPositionsProperty()
        {
            var serializedGeneratorConfig = serializedObject.FindProperty(GeneratorConfigName);
            var serializedShapeStrategy = serializedGeneratorConfig.FindPropertyRelative(ShapePropertyName);
            return serializedShapeStrategy.FindPropertyRelative(PositionsPropertyName);
        }
        
        private void LoadExistingTiles()
        {
            if (_rootTransform == null || _tileMock == null)
                return;

            serializedObject.Update();
            SerializedProperty listVectors = GetSerializedPositionsProperty();

            if (listVectors == null) 
                return;

            _tileMap = new TileMap(CustomShapeConfig.Size);
            
            for (int i = 0; i < listVectors.arraySize; i++)
            {
                Vector3Int cellIndex = listVectors.GetArrayElementAtIndex(i).vector3IntValue;
                _tileMap.Add(cellIndex);
                
                Vector3 savedPos = Origin + new Vector3(cellIndex.x * GridSize.x, cellIndex.y * GridSize.y, 0f);
                savedPos.y += LayerOffset * cellIndex.z;
                var tile = CreateMockTile(savedPos, cellIndex);
                tile.transform.parent = _rootTransform;
            }

            RepaintTiles();
        }
        
        private void RepaintTiles()
        {
            foreach (var tile in _tiles)
            {
                if(_tileMap.IsBlockedByAbove(tile.GridPosition))
                    tile.Color = _blockedColor;
                else
                    tile.Color = Color.white;
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (_isEditing is false)
                return;

            if (Event.current.type is EventType.MouseMove or EventType.MouseDrag)
                sceneView.Repaint();

            DrawGrid();
            HandleMouseInput();

            Handles.BeginGUI();

            var rect = new Rect(10, 10, 200, 50);
            GUILayout.BeginArea(rect, "Level Builder Active", GUI.skin.window);
            GUILayout.Label($"Selected Level: {_config.name}");
            GUILayout.EndArea();
            
            Handles.EndGUI();
        }

        private void DrawGrid()
        {
            Handles.color = Color.green;

            Vector3 origin = Origin;
            int minX = -GridMargin;
            int maxX = (int)Size.x + GridMargin;
            int minY = -GridMargin;
            int maxY = (int)Size.y + GridMargin;

            for (int x = minX; x <= maxX; x++)
            {
                float worldX = origin.x + x * GridSize.x;
                float worldYMin = origin.y + minY * GridSize.y + GridYOffset;
                float worldYMax = origin.y + maxY * GridSize.y + GridYOffset;

                if (_drawGrid is false && x > minX && x < maxX)
                    continue;
                
                Handles.DrawLine(
                    new Vector3(worldX, worldYMin, 0),
                    new Vector3(worldX, worldYMax, 0));
            }

            for (int y = minY; y <= maxY; y++)
            {
                float worldY = origin.y + y * GridSize.y + GridYOffset;
                float worldXMin = origin.x + minX * GridSize.x;
                float worldXMax = origin.x + maxX * GridSize.x;

                if (_drawGrid is false && y > minY && y < maxY)
                    continue;
                
                Handles.DrawLine(
                    new Vector3(worldXMin, worldY, 0),
                    new Vector3(worldXMax, worldY, 0));
            }
        }

        private void HandleMouseInput()
        {
            var e = Event.current;
            Vector3 origin = Origin;
            Plane groundPlane = new Plane(Vector3.back, origin);
            var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            int cellX;
            int cellY;

            bool autoFindLayer = false;
            int targetLayer = 0;

            if (TryRaycastTile(ray, out var hitGridPosition, out var original))
            {
                cellX = hitGridPosition.x;
                cellY = hitGridPosition.y;

                targetLayer = hitGridPosition.z + 1;
            }
            else
            {
                if (groundPlane.Raycast(ray, out float enter) is false)
                    return;

                var hitPoint = ray.GetPoint(enter);
                cellX = Mathf.RoundToInt((hitPoint.x - origin.x) / GridSize.x);
                cellY = Mathf.RoundToInt((hitPoint.y - origin.y) / GridSize.y);

                cellX = Mathf.Clamp(cellX, 0, (int)Size.x);
                cellY = Mathf.Clamp(cellY, 0, (int)Size.y);

                autoFindLayer = true;
            }

            float x = cellX * GridSize.x;
            float y = cellY * GridSize.y;

            var position = origin + new Vector3(x, y, 0f);
            Vector2Int gridPosition = new(cellX, cellY);

            DrawPreviewRect(position, gridPosition, autoFindLayer, targetLayer);

            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlID);

            if (e.alt || e.button != 0)
                return;

            if (e.type is EventType.MouseDown)
            {
                GUIUtility.hotControl = controlID;
                
                switch (_drawMode)
                {
                    case DrawMode.Draw:
                        PlaceTile(cellX, cellY, autoFindLayer, targetLayer);
                        break;
                    case DrawMode.Erase:
                        RemoveTile(original.x, original.y, original.z);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                e.Use();
                RepaintTiles();
            }
            else if (e.type is EventType.MouseDrag)
            {
                if (GUIUtility.hotControl != controlID)
                    return;
                
                switch (_drawMode)
                {
                    case DrawMode.Draw:
                        var listVectors = GetSerializedPositionsProperty();
                        int lastIndex = listVectors.arraySize - 1;
                        int lastPlacedLayer = lastIndex < 0
                            ? 0
                            : listVectors.GetArrayElementAtIndex(lastIndex).vector3IntValue.z;

                        PlaceTile(cellX, cellY, defaultLayer: lastPlacedLayer);
                        break;
                    case DrawMode.Erase:
                        RemoveTile(original.x, original.y, original.z);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                e.Use();
                RepaintTiles();
            }
            else if (e.type is EventType.MouseUp)
            {
                if (GUIUtility.hotControl == controlID)
                {
                    GUIUtility.hotControl = 0;
                    e.Use();
                }
            }
        }

        private bool RemoveTile(int cellX, int cellY, int targetLayer)
        {
            if (cellX < 0 || cellY < 0 || targetLayer < 0)
                return false;
            
            Vector3Int tileGridPosition = new(cellX, cellY, targetLayer);

            if (_tileMap.TryTakeTile(tileGridPosition) is false)
                return false;
            
            var listVectors = GetSerializedPositionsProperty();

            for (int i = listVectors.arraySize - 1; i >= 0; i--)
            {
                var vector3Int = listVectors.GetArrayElementAtIndex(i).vector3IntValue;
                
                if(vector3Int == tileGridPosition)
                {
                    var tile = _tiles[i];
                    
                    if(tile.GridPosition != tileGridPosition)
                        Debug.LogError($"Deleted wrong tile in {tileGridPosition}. Deleted {tile.GridPosition}");
                    
                    _tiles.RemoveAt(i);
                    DestroyImmediate(tile.gameObject);
                    
                    listVectors.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    break;
                }
            }
            return true;
        }

        private bool TryRaycastTile(Ray ray, out Vector3Int gridPosition, out Vector3Int originalPosition)
        {
            gridPosition = default;
            originalPosition = Vector3Int.one * -1;
            
            var hits = Physics2D.RaycastAll(ray.origin, ray.direction);

            if (hits.Length < 1)
                return false;
            
            int highestLayer = int.MinValue;
            
            foreach (var hit in hits)
            {
                if (hit.collider.TryGetComponent(out TileMock tileMock)
                    && tileMock.GridPosition.z > highestLayer)
                {
                    highestLayer = tileMock.GridPosition.z;
                    originalPosition = tileMock.GridPosition;
                    gridPosition = tileMock.GetGridSideByWorldPosition(hit.point);
                }
            }

            return highestLayer != int.MinValue;
        }

        private void DrawPreviewRect(Vector3 center, Vector2Int gridPosition, bool autoFindLayer, int targetLayer)
        {
            int layer = autoFindLayer
                ? _tileMap.GetLowestValidLayer(gridPosition)
                : targetLayer;
            
            layer = Mathf.Max(layer - 1, 0);
            
            float totalGridYOffset = GridYOffset + LayerOffset * layer;
            Vector3[] verts =
            {
                center + new Vector3(-GridSize.x, -GridSize.y + totalGridYOffset, 0),
                center + new Vector3(-GridSize.x, GridSize.y + totalGridYOffset, 0),
                center + new Vector3(GridSize.x, GridSize.y + totalGridYOffset, 0),
                center + new Vector3(GridSize.x, -GridSize.y + totalGridYOffset, 0)
            };
            
            Handles.DrawSolidRectangleWithOutline(
                verts, 
                new Color(0, 1, 0, 0.15f), 
                new Color(0, 1, 0, 0.6f)
            );
        }

        private void PlaceTile(int cellX, int cellY, bool autoFindLayer = false, int defaultLayer = 0)
        {
            int layer = autoFindLayer
                ? _tileMap.GetLowestValidLayer(new Vector2Int(cellX, cellY))
                : defaultLayer;
            
            Vector3 position = Origin + new Vector3(cellX * GridSize.x, cellY * GridSize.y, 0f);
            position.y += LayerOffset * layer;

            serializedObject.Update();
            SerializedProperty listVectors = GetSerializedPositionsProperty();

            Vector3Int cellIndex = new Vector3Int(cellX, cellY, layer);

            if (_tileMap.TryAdd(cellIndex) is false)
                return;

            Undo.RecordObject(_config, "Place Tile");

            int index = listVectors.arraySize;
            listVectors.InsertArrayElementAtIndex(index);
            SerializedProperty newElement = listVectors.GetArrayElementAtIndex(index);
            newElement.vector3IntValue = cellIndex;

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(_config);

            var tile = CreateMockTile(position, cellIndex);
            Undo.RegisterCreatedObjectUndo(tile.gameObject, "Place Tile Visual");
            Undo.SetTransformParent(tile.transform, _rootTransform, "Place Tile Visual");
        }

        private TileMock CreateMockTile(Vector3 position, Vector3Int gridPosition)
        {
            var tile = (TileMock)PrefabUtility.InstantiatePrefab(_tileMock);
            tile.transform.position = position;
            tile.GridPosition = gridPosition;
            var layer = (gridPosition.z * MapVisualizer.LayerPriority) - gridPosition.y;
            tile.SortingOrder = layer;
            tile.name = $"tile {gridPosition}";
            _tiles.Add(tile);
            return tile;
        }
        
        #region  AI
        
        private const string GeminiUrl = "https://generativelanguage.googleapis.com/v1beta/models/";

        private static readonly string[] Models =
        {
            "gemini-3.1-pro-preview",
            "gemini-2.5-pro",
            "gemini-3.5-flash",
            "gemini-3-flash-preview",
            "gemini-2.5-flash",
            "gemini-3.1-flash-lite",
            "gemini-2.5-flash-lite"
        };
        
        private string _model;
        private string _prompt;
        
        private Vector2Int _minMaxTilesCount = new(33, 99);
        
        private bool ValidateCount(int count) =>
            count % MahjongConstants.TilesPerMatch is 0;

        private async UniTask RequestNewLayoutFromAI()
        {
            if (ValidateCount(_minMaxTilesCount.x) is false || ValidateCount(_minMaxTilesCount.y) is false)
            {
                Debug.LogError("Invalid preferred tiles count");
                return;
            }

            _cancellationTokenSource = new();
            _isGenerationProcessing = true;
            string prompt = $@"{_prompt}. Generate a list of approximately {_minMaxTilesCount.x}-{_minMaxTilesCount.y} objects in JSON format, each being an object with 'x', 'y', 'z' integer fields.  
Rules:
1. Coordinates must be non-negative.
2. Total count must be a multiple of {MahjongConstants.TilesPerMatch}.
3. No duplicate coordinates.
4. Support Rule (STRICT): For every tile at position (x, y, z) where z > 0, there must be at least one 'support' tile at z-1.
   A 'support' tile exists if any of the following 9 positions contain a tile:
   (x, y, z-1), (x+1, y, z-1), (x-1, y, z-1), 
   (x, y+1, z-1), (x, y-1, z-1), 
   (x+1, y+1, z-1), (x+1, y-1, z-1), 
   (x-1, y+1, z-1), (x-1, y-1, z-1).
   Basically, the tile at (x, y, z) must be 'supported' by any tile in the 3x3 area directly below it on the z-1 layer.
5. Tile Size: Each tile occupies a 2x2 area in the (x, y) plane.
6. Spacing Rule: No two tiles on the same z-layer can have a distance of exactly 1 unit. 
   Mathematically, for any two tiles (x1, y1, z) and (x2, y2, z), the distance sqrt((x1-x2)^2 + (y1-y2)^2) must not be equal to 1. 
   This means they cannot be adjacent horizontally or vertically (e.g., (0,0) and (1,0) are forbidden, but (0,0) and (2,0) are allowed).
7. Return ONLY a pure JSON array like [{{""x"":0, ""y"":0, ""z"":0}}, ...]. Do not add any text, markdown, or code blocks.
8. Map size {Size.x}x{Size.y}";

            GeminiRequest requestBody = new GeminiRequest {
                contents = new[] { new Content { parts = new[] { new Part { text = prompt } } } }
            };
            
            var jsonRequest = JsonUtility.ToJson(requestBody);
            
            using (UnityWebRequest request = new UnityWebRequest($"{GeminiUrl}{_model}:generateContent?key={GetApiKey()}", "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                Debug.Log("Request sent");
                await request.SendWebRequest().ToUniTask(cancellationToken: _cancellationTokenSource.Token);

                if (request.result == UnityWebRequest.Result.Success)
                {
                    GeminiResponse fullResponse = JsonUtility.FromJson<GeminiResponse>(request.downloadHandler.text);
                    string jsonArray = fullResponse.candidates[0].content.parts[0].text;
                    jsonArray = ExtractJsonFromGeminiResponse(jsonArray);
    
                    string jsonToParse = "{\"list\":" + jsonArray + "}";
    
                    Vector3ListWrapper wrapper = JsonUtility.FromJson<Vector3ListWrapper>(jsonToParse);
    
                    if (wrapper != null && wrapper.list != null)
                    {
                        SerializedProperty listVectors = serializedObject.FindProperty(PositionsPropertyName);
                        int count = wrapper.list.Count;
                        listVectors.arraySize = count;

                        for (int i = 0; i < count; i++)
                        {
                            SerializedProperty element = listVectors.GetArrayElementAtIndex(i);
                            element.vector3IntValue = wrapper.list[i]; 
                        }
                        
                        serializedObject.ApplyModifiedProperties();
                        CustomShapeConfig.IsValidAll();
                        Debug.Log($"Layout updated! Tiles count: {count}");
                    }
                }
                else
                {
                    Debug.LogError($"AI Request failed: {request.error}");
                }
            }

            _isGenerationProcessing = false;
            string ExtractJsonFromGeminiResponse(string response) =>
                response.Replace("```json", "").Replace("```", "").Trim();
        }
        
        public string GetApiKey() 
        {
            TextAsset asset = Resources.Load<TextAsset>("ApiKeyConfig");
            return asset.text.Trim();
        }
        
        [Serializable]
        public class GeminiRequest { public Content[] contents; }
        
        [Serializable]
        public class GeminiResponse { public Candidate[] candidates; }
        [Serializable]
        public class Candidate { public Content content; }
        [Serializable]
        public class Content { public Part[] parts; }
        [Serializable]
        public class Part { public string text; }
        
        [Serializable]
        public class Vector3ListWrapper { public List<Vector3Int> list; }
        
        #endregion
    }
}