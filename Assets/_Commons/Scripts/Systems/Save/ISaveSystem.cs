using Cysharp.Threading.Tasks;

namespace Commons.Systems.Save
{
    public interface ISaveSystem
    {
        void Save<T>(string key, T value);

        UniTask<T> Load<T>(string key, T defaultValue = default);
        
        void DeleteKey(string key);
    }
}
