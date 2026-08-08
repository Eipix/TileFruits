using System;
using System.Collections.Generic;
using System.IO;
using Commons.Systems.Save;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEngine;

namespace Commons.Systems
{
    public class SaveSystem : ISaveSystem
    {
        public const string SaveFile = "SaveData.json";

        private Dictionary<string, object> _data;
        
        public string Path => $"{Application.persistentDataPath}/{SaveFile}";

        public void Set<T>(string key, T value) => _data[key] = value;

        public T Get<T>(string key, T defaultValue = default)
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
            SerializeAndWrite(_data);
            return UniTask.CompletedTask;
        }

        public UniTask LoadAsync()
        {
            if (File.Exists(Path) is false)
            {
                _data = new Dictionary<string, object>();
                return UniTask.CompletedTask;
            }

            try
            {
                byte[] bytes = File.ReadAllBytes(Path);

                var data = SerializationUtility
                    .DeserializeValue<Dictionary<string, object>>(bytes, DataFormat.JSON);
                    
                _data = data ?? new Dictionary<string, object>();
                return UniTask.CompletedTask;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to deserialize save data: {ex.Message}");
                _data = new Dictionary<string, object>();
                return UniTask.CompletedTask;
            }
        }

        public bool HasKey(string key) => _data.ContainsKey(key);

        public void DeleteKey(string key) => _data.Remove(key);

        public void DeleteAll() => _data.Clear();

        private void SerializeAndWrite(Dictionary<string, object> data)
        {
            try
            {
                byte[] bytes = SerializationUtility.SerializeValue(data, DataFormat.JSON);
                File.WriteAllBytes(Path, bytes);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to write save data: {ex.Message}");
            }
        }
    }
}
