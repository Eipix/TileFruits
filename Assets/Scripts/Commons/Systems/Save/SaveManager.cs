using System;
using System.Collections.Generic;

namespace Commons.Systems.SaveManager
{
    public class SaveManager : IRegistry<ISaveLoad>
    {
        private readonly List<ISaveLoad> _saveLoaders = new();

        public event Action Saved;
        public event Action Loaded;

        public void Register(ISaveLoad saveLoad) => _saveLoaders.Add(saveLoad);

        public void Unregister(ISaveLoad saveLoad) => _saveLoaders.Remove(saveLoad);

        public void Save()
        {
            _saveLoaders.ForEach(loadable => loadable.Save());
            Saved?.Invoke();
        }

        public void Load()
        {
            _saveLoaders.ForEach(loadable => loadable.Load());
            Loaded?.Invoke();
        }
    }
}
