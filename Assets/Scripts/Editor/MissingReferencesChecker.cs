#if UNITY_EDITOR

using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Editor
{
    public static class MissingReferencesChecker
    {
        private static bool CheckIfSceneIsDirty()
        {
            if (SceneManager.GetActiveScene().isDirty)
            {
                Debug.LogError("Please save the current scene before checking for missing references");
                return true;
            }

            return false;
        }

        [MenuItem("Tools/Find Missing References/Check All Scenes")]
        private static void CheckAllScenesUI()
        {
            if (CheckIfSceneIsDirty())
            {
                return;
            }

            int countMissing = CheckAllScenes();

            if (countMissing == 0)
            {
                Debug.Log("All scenes checked: No missing references found!");
            }
            else
            {
                Debug.LogError($"Found {countMissing} missing references across all scenes");
            }
        }

        [MenuItem("Tools/Find Missing References/Check All Prefabs")]
        private static void CheckAllPrefabsUI()
        {
            if (CheckIfSceneIsDirty())
            {
                return;
            }

            int countMissing = CheckAllPrefabs();

            if (countMissing == 0)
            {
                Debug.Log("All prefabs checked: No missing references found!");
            }
            else
            {
                Debug.LogError($"Found {countMissing} missing references across all prefabs");
            }
        }

        public static int CheckAllScenes()
        {
            int countMissing = 0;
            string[] scenePaths = AssetDatabase.FindAssets("t:Scene")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.StartsWith("Assets/"))
                .ToArray();

            Scene originalScene = SceneManager.GetActiveScene();

            foreach (string scenePath in scenePaths)
            {
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                GameObject[] rootObjects = scene.GetRootGameObjects();
                foreach (GameObject rootObject in rootObjects)
                {
                    int countMissingGameObject = CheckGameObjectForMissingReferences(rootObject, scenePath);
                    countMissing += countMissingGameObject;
                }
            }

            if (!string.IsNullOrEmpty(originalScene.path))
            {
                EditorSceneManager.OpenScene(originalScene.path, OpenSceneMode.Single);
            }

            return countMissing;
        }

        public static int CheckAllPrefabs()
        {
            int countMissing = 0;
            string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.StartsWith("Assets/"))
                .ToArray();

            foreach (string prefabPath in prefabPaths)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (!prefab)
                {
                    continue;
                }

                int countMissingGameObject = CheckGameObjectForMissingReferences(prefab, prefabPath);
                countMissing += countMissingGameObject;
            }

            return countMissing;
        }


        private static int CheckGameObjectForMissingReferences(GameObject obj, string assetPath)
        {
            int countMissing = 0;
            Component[] components = obj.GetComponentsInChildren<Component>(true);

            foreach (Component component in components)
            {
                if (!component)
                {
                    Debug.LogWarning($"Missing component in {assetPath} on GameObject: {GetGameObjectPath(obj)}");
                    countMissing++;
                    continue;
                }

                SerializedObject so = new SerializedObject(component);
                SerializedProperty sp = so.GetIterator();

                while (sp.NextVisible(true))
                {
                    if (!IsMissingReference(sp))
                    {
                        continue;
                    }

                    Debug.LogWarning(
                        $"Missing reference in {assetPath} on {component.GetType().Name}.{sp.propertyPath} in GameObject: {GetGameObjectPath(component.gameObject)}");
                    countMissing++;
                }
            }

            return countMissing;
        }

        private static bool IsMissingReference(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                return false;
            }

            if (property.objectReferenceValue || property.objectReferenceInstanceIDValue == 0)
            {
                return false;
            }

            return true;
        }

        private static string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;
            Transform current = obj.transform.parent;

            while (current)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            return path;
        }
    }
}

#endif
