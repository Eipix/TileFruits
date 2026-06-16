using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Constants;
using Cysharp.Threading.Tasks;
using Gameplay;
using Generator;
using Generator.GenerationStrategies.Implementations.Custom;
using ModestTree;
using UnityEngine;
using UnityEngine.Networking;

namespace Editor
{
    using UnityEditor;
    
    [CustomEditor(typeof(CustomStrategyConfig))]
    public class CustomStrategyEditor : Editor
    {
        private const string PositionsPropertyName = "_positions";
        private const float LayerOffset = 0.16f;
        private const float GridSize = 0.7f;
        
        [SerializeField] private TileMock _tileMock;
        
        private CustomStrategyConfig _config;
        private Transform _rootTransform;
        private TileMap _tileMap;

        private CancellationTokenSource _cancellationTokenSource = new();
        private UniTask _aiGenerating;
        private bool _isGenerationProcessing;
        
        private Vector2 Size => _config.Size;
        private string RootName => $"[CustomStrategyEditor] {_config.name}]";

        private Vector3 Origin
        {
            get
            {
                float width = Size.x * GridSize;
                float height = Size.y * GridSize;
                return -new Vector3(width / 2f, height / 2f, 0f);
            }
        }

        private bool _isEditing;

        private void OnEnable()
        {
            _prompt = EditorPrefs.GetString(nameof(_prompt));
            _model = EditorPrefs.GetString(nameof(_model));
            
            int rx = EditorPrefs.GetInt(nameof(_range) + "X", 0);
            int ry = EditorPrefs.GetInt(nameof(_range) + "Y", rx + 100);
            _range = new Vector2Int(rx, ry);
            
            _config = (CustomStrategyConfig)target;
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
            if (_isEditing && _config != null)
            {
                Clear();
                _rootTransform = new GameObject(RootName).transform;
                LoadExistingTiles();
            }
        }

        private void Clear()
        {
            if(_rootTransform != null)
            {
                DestroyImmediate(_rootTransform.gameObject);
                _rootTransform = null;
            }
            _tileMap?.Dispose();
            _tileMap = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawPropertiesExcluding(serializedObject, PositionsPropertyName);
            SerializedProperty listVectors = serializedObject.FindProperty(PositionsPropertyName);
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(listVectors, true);
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.Space(15);
            GUI.backgroundColor = _isEditing ? Color.green : Color.white;
            
            string buttonText = _isEditing ? "Exit From Edit Mode" : "Enter Edit Mode";
            
            if (GUILayout.Button(buttonText, GUILayout.Height(40)))
            {
                _isEditing = !_isEditing;
                
                if (_isEditing)
                {
                    _rootTransform = new GameObject(RootName).transform;
                    LoadExistingTiles();
                }
                else
                {
                    Clear();
                }
                
                SceneView.RepaintAll();
            }
            
            GUI.backgroundColor = Color.white;

            if (GUILayout.Button("IsValid", GUILayout.Height(30)))
                _config.IsValid();
            
            if (GUILayout.Button("Clear", GUILayout.Height(30)))
            {
                if(_rootTransform != null)
                    for (int i = _rootTransform.childCount - 1; i >= 0; i--)
                        DestroyImmediate(_rootTransform.GetChild(i).gameObject);
                
                _tileMap?.Clear();
                var positions = serializedObject.FindProperty(PositionsPropertyName);
                positions.ClearArray();
                SceneView.RepaintAll();
            }
            
            if (GUILayout.Button("RequestNewLayoutFromAI", GUILayout.Height(30)))
            {
                if (_isGenerationProcessing is false)
                    _aiGenerating = RequestNewLayoutFromAI();
            }
            
            EditorGUI.BeginChangeCheck();
            _prompt = GUILayout.TextArea(_prompt, GUILayout.MinHeight(60), GUILayout.ExpandHeight(true));

            int index = Models.IndexOf(_model);

            if (index == -1)
                index = 0;
            
            index = EditorGUILayout.Popup(index, Models);
            _model = Models[index];

            _range = EditorGUILayout.Vector2IntField(nameof(_range), _range);

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(nameof(_prompt), _prompt);
                EditorPrefs.SetString(nameof(_model), _model);
                EditorPrefs.SetInt(nameof(_range) + "X", _range.x);
                EditorPrefs.SetInt(nameof(_range) + "Y", _range.y);
            }
            
            if (_isEditing)
                EditorGUILayout.HelpBox("Edit mode is active!\nHold down Shift + Click in " +
                                        "Scene window to place a 2x2 prefab", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
        
        private void LoadExistingTiles()
        {
            if (_rootTransform == null || _tileMock == null)
                return;

            serializedObject.Update();
            SerializedProperty listVectors = serializedObject.FindProperty(PositionsPropertyName);

            if (listVectors == null) 
                return;

            _tileMap = new TileMap(_config.Size);
            
            for (int i = 0; i < listVectors.arraySize; i++)
            {
                Vector3Int cellIndex = listVectors.GetArrayElementAtIndex(i).vector3IntValue;
                _tileMap.Add(cellIndex);
                
                Vector3 savedPos = Origin + new Vector3(cellIndex.x * GridSize, cellIndex.y * GridSize, 0f);
                savedPos.y += LayerOffset * cellIndex.z;
                var tile = CreateMockTile(savedPos, cellIndex.z);
                tile.transform.parent = _rootTransform;
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (_isEditing is false || _config == null)
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
            float width = Size.x * GridSize;
            float height = Size.y * GridSize;

            for (int x = 0; x <= Size.x; x++)
            {
                Vector3 start = origin + new Vector3(x * GridSize, 0);
                Vector3 end = start + new Vector3(0, height);
                
                Handles.DrawLine(start, end);
            }
            
            for (int y = 0; y <= Size.y; y++)
            {
                Vector3 start = origin + new Vector3(0, y * GridSize);
                Vector3 end = start + new Vector3(width, 0);
                
                Handles.DrawLine(start, end);
            }
        }

        private void HandleMouseInput()
        {
            var e = Event.current;
            Vector3 origin = Origin;
            Plane groundPlane = new Plane(Vector3.back, origin);
            var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            if (groundPlane.Raycast(ray, out float enter) is false)
                return;

            var hitPoint = ray.GetPoint(enter);

            int cellX = Mathf.RoundToInt((hitPoint.x - origin.x) / GridSize);
            int cellY = Mathf.RoundToInt((hitPoint.y - origin.y) / GridSize);

            cellX = Mathf.Clamp(cellX, 1, (int)Size.x - 1);
            cellY = Mathf.Clamp(cellY, 1, (int)Size.y - 1);

            float x = cellX * GridSize;
            float y = cellY * GridSize;

            var gridPosition = origin + new Vector3(x, y, 0f);

            DrawPreviewRect(gridPosition);

            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            if (e.modifiers is EventModifiers.Shift)
            {
                HandleUtility.AddDefaultControl(controlID);

                if (e is { button: 0, type: EventType.MouseDown })
                {
                    PlaceTile(cellX, cellY, true);
                    e.Use();
                }
                else if (e is { button: 0, type: EventType.MouseDrag })
                {
                    var listVectors = serializedObject.FindProperty(PositionsPropertyName);
                    int lastIndex = listVectors.arraySize - 1;
                    int lastPlacedLayer = lastIndex < 0
                        ? 0
                        : listVectors.GetArrayElementAtIndex(lastIndex).vector3IntValue.z;
                    
                    PlaceTile(cellX, cellY, defaultLayer: lastPlacedLayer);
                    e.Use();
                }
            }
        }

        private void DrawPreviewRect(Vector3 center)
        {
            Vector3[] verts =
            {
                center + new Vector3(-GridSize, -GridSize, 0),
                center + new Vector3(-GridSize, GridSize, 0),
                center + new Vector3(GridSize, GridSize, 0),
                center + new Vector3(GridSize, -GridSize, 0)
            };

            Handles.DrawSolidRectangleWithOutline(
                verts, 
                new Color(0, 1, 0, 0.15f), 
                new Color(0, 1, 0, 0.6f)
            );
        }

        private void PlaceTile(int cellX, int cellY, bool allowAutoJumpLayer = false, int defaultLayer = 0)
        {
            int highestLayer = -1;
            int supportRadius = 1; 

            foreach (Vector3Int pos in _tileMap.Positions)
            {
                if (Mathf.Abs(pos.x - cellX) <= supportRadius
                    && Mathf.Abs(pos.y - cellY) <= supportRadius)
                {
                    if (pos.z > highestLayer)
                        highestLayer = pos.z;
                }
            }
            
            int layer = defaultLayer;
            
            Vector3 position = Origin + new Vector3(cellX * GridSize, cellY * GridSize, 0f);

            if (highestLayer >= 0)
            {
                if (allowAutoJumpLayer)
                {
                    layer = highestLayer + 1;
                }
            }

            position.y += LayerOffset * layer;

            serializedObject.Update();
            SerializedProperty listVectors = serializedObject.FindProperty(PositionsPropertyName);

            if (listVectors != null)
            {
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
            }

            if (_tileMock != null)
            {
                var tile = CreateMockTile(position, layer);
                Undo.RegisterCreatedObjectUndo(tile.gameObject, "Place Tile Visual");
                Undo.SetTransformParent(tile.transform, _rootTransform, "Place Tile Visual");
            }
        }

        private TileMock CreateMockTile(Vector3 position, int sortingOrder)
        {
            var tile = (TileMock)PrefabUtility.InstantiatePrefab(_tileMock);
            tile.transform.position = position;
            tile.SortingOrder = sortingOrder;
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
        
        private Vector2Int _range = new(33, 99);
        
        private bool ValidateCount(int count) =>
            count % MahjongConstants.TilesPerMatch is 0;

        private async UniTask RequestNewLayoutFromAI()
        {
            if (ValidateCount(_range.x) is false || ValidateCount(_range.y) is false)
            {
                Debug.LogError("Invalid preferred tiles count");
                return;
            }

            _cancellationTokenSource = new();
            _isGenerationProcessing = true;
            string prompt = $@"{_prompt}. Generate a list of approximately {_range.x}-{_range.y} objects in JSON format, each being an object with 'x', 'y', 'z' integer fields.  
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
                        _config.IsValid();
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