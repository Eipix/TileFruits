using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Commons.ScriptableObjects
{
    public class GUIDScriptableObject : ScriptableObject
    {
        [field: SerializeField, HideInInspector]
        public string Id { get; private set; }

        private void OnValidate()
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(this);

            if (string.IsNullOrEmpty(path))
                return;

            string guid = AssetDatabase.AssetPathToGUID(path);

            if (Id != guid)
            {
                Id = guid;
                EditorUtility.SetDirty(this);
            }
#endif
            Validate();
        }

        protected virtual void Validate() { }
    }
}
