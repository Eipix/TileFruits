using System;

namespace Generator.Provider
{
    public class TileMapProvider : ITileMapProvider
    {
        private ITileMap _tileMap;

        public ITileMap ActiveMap
        {
            get => _tileMap;
            set
            {
                _tileMap = value ?? throw new ArgumentNullException(nameof(value));
                MapChanged?.Invoke(value);
            }
        }
    
        public event Action<ITileMap> MapChanged;
    }
}
