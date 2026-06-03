using System;
using System.Collections.Generic;
using Commons.Systems;
using Commons.Systems.Save;
using Commons.Systems.SaveManager;
using Constants;
using Cysharp.Threading.Tasks;
using Gameplay;
using Gameplay.Levels;
using Generator;
using Generator.Provider;
using UnityEngine;
using Zenject;

namespace View.Windows.Collection
{
    public class CollectionController : IInitializable, IDisposable, ISaveLoad
    {
        private readonly CollectionWindow _window;
        private readonly ISaveSystem _saveSystem;
        private readonly LevelManager _levelManager;
        private readonly TileList _tileDatabase;
        private readonly IRegistry<ISaveLoad> _saveLoadRegistry;

        private List<string> _tileIds;

        public CollectionController(CollectionWindow window,
            ISaveSystem saveSystem,
            LevelManager levelManager,
            TileList tileDatabase,
            IRegistry<ISaveLoad> registry)
        {
            _window = window;
            _saveSystem = saveSystem;
            _levelManager = levelManager;
            _tileDatabase = tileDatabase;
            _saveLoadRegistry = registry;

            _tileIds = new(_tileDatabase.Length);
        }

        public void Initialize()
        {
            foreach (var config in _tileDatabase)
                _window.Add(config);

            _levelManager.LevelStarted += ExploreTiles;
            _saveLoadRegistry.Register(this);
        }

        public void Dispose()
        {
            _levelManager.LevelStarted -= ExploreTiles;
            _saveLoadRegistry.Unregister(this);
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
                Save();
        }

        public void Save() =>
            _saveSystem.Save(SaveKeys.ExploredItemIDs_StringArray, _tileIds);

        public async UniTask Load()
        {
             _tileIds = await _saveSystem.Load(SaveKeys.ExploredItemIDs_StringArray, _tileIds);
            
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
