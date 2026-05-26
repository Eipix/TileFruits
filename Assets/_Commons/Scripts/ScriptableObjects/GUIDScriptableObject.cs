using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Commons.ScriptableObjects
{
    public class GUIDScriptableObject : ScriptableObject
    {
        [field: SerializeField, ReadOnly] public string Id { get; private set; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            string path = AssetDatabase.GetAssetPath(this);

            if (string.IsNullOrEmpty(path))
                return;

            string guid = AssetDatabase.AssetPathToGUID(path);

            if (Id != guid)
            {
                Id = guid;
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}
