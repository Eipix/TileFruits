using System;
using JetBrains.Annotations;

namespace Generator.Provider
{
    public interface ITileMapProvider
    {
        [CanBeNull] public ITileMap ActiveMap { get; }
    
        public event Action<ITileMap> MapChanged;
    }
}
