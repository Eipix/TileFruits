using System;
using System.Globalization;
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
        public void Save<T>(string key, T value)
        {
            byte[] bytes = SerializationUtility.SerializeValue(value, DataFormat.JSON);
            string json = Encoding.UTF8.GetString(bytes);
            Bridge.storage.Set(key, json);
            Debug.Log($"Save {key}: {value}");
        }

        public async UniTask<T> Load<T>(string key, T defaultValue = default)
        {
            bool loaded = false;
            T result = defaultValue;
            
            Bridge.storage.Get(key, OnComplete);
            
            await UniTask.WaitUntil(() => loaded);
            
            return result;

            void OnComplete(bool success, string data)
            {
                if (success is false || data is null)
                {
                    Debug.Log($"Load Fail {key}");
                    loaded = true;
                    return;
                }
                
                Type targetType = typeof(T);
                
                if (FormatterUtilities.IsPrimitiveType(targetType))
                {
                    result = (T)Convert.ChangeType(data, targetType, CultureInfo.InvariantCulture);
                    loaded = true;
                    return;
                }

                byte[] bytes = Encoding.UTF8.GetBytes(data);
                result = SerializationUtility.DeserializeValue<T>(bytes, DataFormat.JSON);
                loaded = true;
            }
        }

        public void DeleteKey(string key) => Bridge.storage.Delete(key);
    }
}
