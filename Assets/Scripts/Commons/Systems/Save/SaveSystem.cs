using System;
using System.Collections.Generic;
using System.IO;
using Commons.Systems.Save;
using OdinSerializer;
using UnityEngine;

namespace Commons.Systems
{
    public class SaveSystem : ISaveSystem
    {
        public const string SaveFile = "SaveData.json";

        public string JsonData => File.Exists(Path)
            ? File.ReadAllText(Path)
            : string.Empty;
        
        public string Path => $"{Application.persistentDataPath}/{SaveFile}";
        
        public Dictionary<string, object> Data
        {
            get
            {
                if (File.Exists(Path) is false)
                    return new Dictionary<string, object>();

                try
                {
                    byte[] bytes = File.ReadAllBytes(Path);

                    var data = SerializationUtility
                        .DeserializeValue<Dictionary<string, object>>(bytes, DataFormat.JSON);
                    
                    return data ?? new Dictionary<string, object>();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to deserialize save data: {ex.Message}");
                    return new Dictionary<string, object>();
                }
            }
        }

        public void Save(string key, object value)
        {
            var data = Data;
            data[key] = value;
            SerializeAndWrite(data);
        }

        public T Load<T>(string key, T defaultValue = default)
        {
            if (File.Exists(Path) is false)
                return defaultValue;

            var data = Data;

            if (data.TryGetValue(key, out object value) is false)
                return defaultValue;

            if (value is T direct)
                return direct;

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load '{key}' as {typeof(T)}: {ex.Message}. Using default value.");
                return defaultValue;
            }
        }

        public bool TryRemoveKey(string key)
        {
            var data = Data;

            if (data.Remove(key))
            {
                SerializeAndWrite(data);
                return true;
            }

            return false;
        }

        public void Reset()
        {
            if (File.Exists(Path))
                File.Delete(Path);
        }

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
