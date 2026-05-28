using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Commons.Systems.SaveManager
{
    public class SaveManager : IRegistry<ISaveLoad>
    {
        private readonly List<ISaveLoad> _saveLoaders = new();

        public void Register(ISaveLoad saveLoad) => _saveLoaders.Add(saveLoad);

        public void Unregister(ISaveLoad saveLoad) => _saveLoaders.Remove(saveLoad);

        public void Save()
        {
            _saveLoaders.ForEach(loadable => loadable.Save());
        }

        public async UniTask Load()
        {
            foreach (var saveLoader in _saveLoaders)
                await saveLoader.Load();
        }
    }
}
