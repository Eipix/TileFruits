using UnityEngine;

namespace Commons.Extensions
{
    public static class InstantiateExtensions
    {
        public static T Instantiate<T>(string name = null) where T : Component, new()
        {
            return Instantiate<T>(Vector3.zero, Quaternion.identity, null, name);
        }
        
        public static T Instantiate<T>(
            Transform parent,
            string name = null) where T : Component, new()
        {
            return Instantiate<T>(Vector3.zero, Quaternion.identity, parent, name);
        }
        
        public static T Instantiate<T>(
            Vector3 position,
            Transform parent,
            string name = null) where T : Component, new()
        {
            return Instantiate<T>(position, Quaternion.identity, parent, name);
        }
        
        public static T Instantiate<T>(
            Vector3 position,
            Quaternion rotation,
            Transform parent,
            string name = null) where T : Component, new()
        {
            var go = new GameObject();
            T instance = go.AddComponent<T>();
            instance.transform.SetParent(parent);
            instance.transform.localScale = Vector3.one;
            instance.transform.position = position;
            instance.transform.rotation = rotation;

            if (name != null)
                instance.name = name;
            
            return instance;
        }
    }
}
