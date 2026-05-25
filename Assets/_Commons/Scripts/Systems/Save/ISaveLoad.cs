using Cysharp.Threading.Tasks;

namespace Commons.Systems.SaveManager
{
    public interface ISaveLoad
    {
        void Save();
        UniTask Load();
    }
}
