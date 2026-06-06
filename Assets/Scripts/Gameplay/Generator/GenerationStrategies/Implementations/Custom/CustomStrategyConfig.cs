using System;
using System.Collections.Generic;
using System.Text;
using Commons.Utils;
using Constants;
using Cysharp.Threading.Tasks;
using Gameplay.Generator.GenerationStrategies.Base;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.Networking;
#endif

namespace Generator.GenerationStrategies.Implementations.Custom
{
    [CreateAssetMenu(menuName = "Generator/GenerationStrategies/Custom")]
    public class CustomStrategyConfig : GenerationStrategyConfig<CustomStrategy>
    {
        [ShowNativeProperty] private Vector2Int ExpectedSize => Size;
        
        [SerializeField] private List<Vector3Int> _positions;
        
        public IReadOnlyList<Vector3Int> Positions => _positions;

        protected override bool HideSize => true;

        private void OnValidate() => Size = FindSize();
        
        private Vector2Int FindSize()
        {
            Vector2Int size = Vector2Int.one;
            
            foreach (var position in _positions)
            {
                if (position.x > size.x)
                    size.x = position.x;
                
                if (position.y > size.y)
                    size.y = position.y;
            }
            
            return size;
        }

        [Button]
        private bool IsValid()
        {
            if (IsValid(out var error))
            {
                Debug.Log($"<color=green>{name} strategy is valid!</color>");
                return true;
            }
            
            Debug.LogError(error);
            return false;
        }
        
        public bool IsValid(out string errorMessage)
        {
            errorMessage = string.Empty;
            
            if (_positions.Count is 0)
            {
                errorMessage = "No positions have been generated";
                return false;
            }

            bool isSolvable = _positions.Count % MahjongConstants.TilesPerMatch is 0;

            if (isSolvable is false)
            {
                errorMessage = $"Invalid positions count (must be divided by {MahjongConstants.TilesPerMatch})";
                return false;
            }
                
            HashSet<Vector3Int> positionsLookup = new(_positions.Count);
            
            foreach (var position in _positions)
            {
                if (positionsLookup.Add(position) is false) 
                {
                    errorMessage = $"Duplicate positions are not allowed: {position}";
                    return false;
                }
                
                if(position.x < 0 || position.y < 0 || position.z < 0)
                {
                    errorMessage = $"Position cannot be negative {position}";
                    return false;
                }
            }
            
            foreach (var position in _positions)
            {
                if (HasPositionAround(position, positionsLookup))
                {
                    errorMessage = $"position offset must be 2 - {position}";
                    return false;
                }

                var lowerLayerPosition = position;
                lowerLayerPosition.z--;

                if (lowerLayerPosition.z < 0)
                    continue;

                if (positionsLookup.Contains(lowerLayerPosition) is false
                    && HasPositionAround(lowerLayerPosition, positionsLookup) is false)
                {
                    errorMessage = $"position {position} must have support from below";
                    return false;
                }
            }

            return true;
        }

        private bool HasPositionAround(Vector3Int center, HashSet<Vector3Int> lookup) =>
            HasPositionInDirections(center, TileMapUtils.DirectionsAround, lookup);
        
        private bool HasPositionInDirections(Vector3Int center, ReadOnlySpan<Vector3Int> directions, HashSet<Vector3Int> lookup)
        {
            foreach (var direction in directions)
            {
                var targetPosition = center + direction;
                
                if (lookup.Contains(targetPosition))
                    return true;
            }

            return false;
        }

        #if UNITY_EDITOR
        
        #region  AI
        
        private const string GeminiUrl = "https://generativelanguage.googleapis.com/v1beta/models/";

        private readonly string[] _models =
        {
            "gemini-3.1-pro-preview",
            "gemini-2.5-pro",
            "gemini-3.5-flash",
            "gemini-2.5-flash",
            "gemini-3-flash-preview",
            "gemini-3.1-flash-lite",
            "gemini-2.5-flash-lite"
        };
        
        [SerializeField, Dropdown(nameof(_models))]
        private string _model;

        [SerializeField] private string _prompt;
        
        [SerializeField, Min(0f), ValidateInput(nameof(ValidateCount))]
        private int _preferredTilesCountMin = 32;
        
        [SerializeField, Min(0f), ValidateInput(nameof(ValidateCount))]
        private int _preferredTilesCountMax = 100;

        private bool _isLoading;

        private bool ValidateCount(int count) =>
            count % MahjongConstants.TilesPerMatch is 0;

        [Button]
        private void RequestNewAILayout() => RequestNewLayoutFromAI().Forget();

        private async UniTask RequestNewLayoutFromAI()
        {
            if (ValidateCount(_preferredTilesCountMin) is false || ValidateCount(_preferredTilesCountMax) is false)
            {
                Debug.LogError("Invalid preferred tiles count");
                return;
            }
            
            
            _isLoading = true;
            string prompt = $@"{_prompt}. Avoid primitive form (as pyramid). Generate a list of approximately {_preferredTilesCountMin}-{_preferredTilesCountMax} objects in JSON format, each being an object with 'x', 'y', 'z' integer fields.  
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
7. Return ONLY a pure JSON array like [{{""x"":0, ""y"":0, ""z"":0}}, ...]. Do not add any text, markdown, or code blocks.";

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
                await request.SendWebRequest().ToUniTask();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    GeminiResponse fullResponse = JsonUtility.FromJson<GeminiResponse>(request.downloadHandler.text);
                    string jsonArray = fullResponse.candidates[0].content.parts[0].text;
                    jsonArray = ExtractJsonFromGeminiResponse(jsonArray);
    
                    string jsonToParse = "{\"list\":" + jsonArray + "}";
    
                    Vector3ListWrapper wrapper = JsonUtility.FromJson<Vector3ListWrapper>(jsonToParse);
    
                    if (wrapper != null && wrapper.list != null)
                    {
                        _positions = wrapper.list;
                        OnValidate();
                        IsValid();
                        Debug.Log($"Layout updated! Tiles count: {_positions.Count}");
                    }
                }
                else
                {
                    Debug.LogError($"AI Request failed: {request.error}");
                }

                _isLoading = false;
            }
            
            string ExtractJsonFromGeminiResponse(string response) =>
                response.Replace("```json", "").Replace("```", "").Trim();
        }
        
        public string GetApiKey() 
        {
            TextAsset asset = Resources.Load<TextAsset>("ApiKeyConfig");
            return asset.text.Trim();
        }
        
        [Serializable]
        public class GeminiRequest
        {
            public Content[] contents;
        }
        
        [Serializable]
        public class GeminiResponse { public Candidate[] candidates; }
        [Serializable]
        public class Candidate { public Content content; }
        [Serializable]
        public class Content { public Part[] parts; }
        [Serializable]
        public class Part { public string text; }
        
        [Serializable]
        public class Vector3ListWrapper
        {
            public List<Vector3Int> list;
        }
        
        #endregion
        #endif
    }
}
