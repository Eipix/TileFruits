using System;
using System.Collections.Generic;
using System.Text;
using Commons.Systems.Save;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using Playgama;
using UnityEngine;

namespace WebGLCommons.Scripts
{
    public class SDKSaveSystem : ISaveSystem
    {
        private const string GlobalSaveKey = "Data";
        
        private Dictionary<string, object> _data;
        
        public void Set<T>(string key, T value)
        {
            _data[key] = value;
            Debug.Log($"Set {key}");
        }

        public T Get<T>(string key, T defaultValue)
        {
            if (_data.TryGetValue(key, out object value) is false)
                return defaultValue;

            if (value is T direct)
                return direct;

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get '{key}' as {typeof(T)}: {ex.Message}. Using default value.");
                return defaultValue;
            }
        }

        public UniTask SaveAsync()
        {
            byte[] bytes = SerializationUtility.SerializeValue(_data, DataFormat.JSON);
            string json = Encoding.UTF8.GetString(bytes);
            Bridge.storage.Set(GlobalSaveKey, json);
            Debug.Log("Save");
            return UniTask.CompletedTask;
        }

        public async UniTask LoadAsync()
        {
            bool loaded = false;
            Bridge.storage.Get(GlobalSaveKey, OnComplete);
            
            await UniTask.WaitUntil(() => loaded);

            void OnComplete(bool success, string data)
            {
                if (success is false || data is null)
                {
                    _data = new();
                }
                else
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    _data = SerializationUtility
                        .DeserializeValue<Dictionary<string, object>>(bytes, DataFormat.JSON);
                }

                loaded = true;
            }
        }

        public void DeleteKey(string key) => Bridge.storage.Delete(key);
        public void DeleteAll() => Bridge.storage.Delete(GlobalSaveKey);
    }
}
