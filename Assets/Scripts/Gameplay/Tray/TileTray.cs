using System;
using System.Collections.Generic;
using Constants;

namespace Gameplay.Tray
{
    public class TileTray
    {
        private readonly List<TileConfig> _tiles;
        
        public event Action<TileConfig, int> Added;
        public event Action<TileConfig> MatchCleared;
        public event Action Filled;
        public event Action Cleared;
        
        private TileTraySettings _settings;
        
        public int Capacity => _settings.Capacity;
        public bool HasSpace => _tiles.Count < Capacity;
        
        public TileTray(TileTraySettings settings)
        {
            _settings = settings;
            _tiles = new(Capacity);
        }
        
        public void Add(TileConfig tile)
        {
            if (HasSpace is false)
                return;

            int targetIndex = FindInsertIndex(tile);
            _tiles.Insert(targetIndex, tile);
        
            Added?.Invoke(tile, targetIndex);

            TryMatch();

            if (_tiles.Count == Capacity)
                Filled?.Invoke();
        }

        private int FindInsertIndex(TileConfig tile)
        {
            for (int i = 0; i < _tiles.Count; i++)
            {
                if (_tiles[i] == tile)
                    return i;
            }
            return _tiles.Count;
        }

        private bool TryMatch()
        {
            if(CanMatch(out var match))
            {
                _tiles.RemoveAll(tile => tile == match);

                MatchCleared?.Invoke(match);
                return true;
            }
            
            return false;
        }

        private bool CanMatch(out TileConfig match)
        {
            match = null;
            int count = _tiles.Count;
            int required = MahjongConstants.TilesPerMatch;

            if (count < required)
                return false;
        
            int consecutiveCount = 1;
            for (int i = 1; i < count; i++)
            {
                if (_tiles[i] == _tiles[i - 1])
                {
                    consecutiveCount++;
                    if (consecutiveCount >= required)
                    {
                        match = _tiles[i];
                        return true;
                    }
                }
                else
                {
                    consecutiveCount = 1;
                }
            }
            
            return false;
        }

        public void Clear()
        {
            _tiles.Clear();
            Cleared?.Invoke();
        }
    }
}
