using System;
using Commons.Systems;
using Commons.Systems.Save;
using Commons.Systems.SaveManager;
using Constants;
using Cysharp.Threading.Tasks;
using Generator;
using Zenject;

namespace Gameplay.Levels
{
    public class LevelManager : ISaveLoad, IInitializable, IDisposable
    {
        public event Action<ITileMap> LevelStarted;
        
        private IRegistry<ISaveLoad> _saveLoadRegistry;
        private LevelList _levelList;
        private TileMapGenerator _tileMapGenerator;
        private MapVisualizer _mapVisualizer;
        private ISaveSystem _saveSystem;

        public Level CurrentLevel => _levelList[_levelIndex];
        
        private int _levelIndex;

        [Inject]
        private void Construct(LevelList levelList,
            TileMapGenerator tileMapGenerator,
            MapVisualizer mapVisualizer,
            ISaveSystem saveSystem,
            IRegistry<ISaveLoad> registry)
        {
            _levelList = levelList;
            _tileMapGenerator = tileMapGenerator;
            _mapVisualizer = mapVisualizer;
            _saveSystem = saveSystem;
            _saveLoadRegistry = registry;
        }
        
        public void Initialize() => _saveLoadRegistry.Register(this);

        public void Save() => _saveSystem.Save(SaveKeys.LevelIndexInt, _levelIndex);

        public async UniTask Load()
            => _levelIndex = await _saveSystem.Load(SaveKeys.LevelIndexInt, 0);

        public void StartLevel()
        {
            var map = _tileMapGenerator.GenerateGrid(CurrentLevel.GeneratorConfig);
            _mapVisualizer.CreateTiles(map);
            LevelStarted?.Invoke(map);
        }

        public void Dispose() => _saveLoadRegistry.Unregister(this);
    }
}
