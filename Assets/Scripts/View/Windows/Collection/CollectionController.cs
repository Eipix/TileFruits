using System;
using System.Collections.Generic;
using Commons.Systems;
using Commons.Systems.Save;
using Commons.Systems.SaveManager;
using Constants;
using Cysharp.Threading.Tasks;
using Gameplay;
using Gameplay.Levels;
using Zenject;

namespace View.Windows.Collection
{
    public class CollectionController : IInitializable, IDisposable
    {
        private readonly CollectionWindow _window;
        private readonly ISaveSystem _saveSystem;
        private readonly LevelManager _levelManager;
        private readonly TileList _tileDatabase;

        private List<string> _tileIds;

        public CollectionController(CollectionWindow window,
            ISaveSystem saveSystem,
            LevelManager levelManager,
            TileList tileDatabase)
        {
            _window = window;
            _saveSystem = saveSystem;
            _levelManager = levelManager;
            _tileDatabase = tileDatabase;

            _tileIds = new(_tileDatabase.Length);
        }

        public void Initialize()
        {
            foreach (var config in _tileDatabase)
                _window.Add(config);

            _window.ForceRebuildLayoutImmediate();
            
            Load();
            
            _levelManager.LevelStarted += ExploreTiles;
        }

        public void Dispose()
        {
            _levelManager.LevelStarted -= ExploreTiles;
        }

        public void ExploreTiles()
        {
            var levelTiles = _levelManager.CurrentLevel.GeneratorConfig.TileList;
            bool hasNewTiles = false;

            foreach (var levelConfig in levelTiles)
            {
                if (_tileIds.Contains(levelConfig.Id) is false)
                {
                    _tileIds.Add(levelConfig.Id);
                    _window.Unlock(levelConfig);
                    hasNewTiles = true;
                }
            }

            if (hasNewTiles)
                _saveSystem.SetAndSave(SaveKeys.ExploredItemIDs_StringArray, _tileIds);
        }

        private void Load()
        {
             _tileIds = _saveSystem.Get(SaveKeys.ExploredItemIDs_StringArray, _tileIds);
            
            foreach (var tileId in _tileIds)
            {
                if (tileId == null)
                    continue;
                
                foreach (var tileConfig in _tileDatabase)
                {
                    if (tileId == tileConfig.Id)
                        _window.Unlock(tileConfig);
                }
            }
        }
    }
}
