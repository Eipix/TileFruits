using Cysharp.Threading.Tasks;

namespace Commons.Systems.Save
{
    public interface ISaveSystem
    {
        void SetAndSave<T>(string key, T value)
        {
            Set(key, value);
            SaveAsync();
        }
        
        void Set<T>(string key, T value);
        T Get<T>(string key, T defaultValue = default);

        UniTask SaveAsync();
        UniTask LoadAsync();
        
        void DeleteKey(string key);
        void DeleteAll();
    }
}
