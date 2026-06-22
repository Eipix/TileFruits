using System;
using System.Linq;
using _Commons.Scripts.EnumerationStrategies;
using Commons.Systems;
using Commons.Systems.Save;
using Commons.Systems.SaveManager;
using Constants;
using Cysharp.Threading.Tasks;
using Gameplay.Tray;
using Generator;
using UnityEngine;
using Zenject;

namespace Gameplay.Levels
{
    public class LevelManager : ISaveLoad, IInitializable, IDisposable
    {
        public event Action LevelStarted;
        public event Action<LevelResult> LevelFinished;
        
        private LevelList _levelList;
        private TileMapGenerator _tileMapGenerator;
        private MapVisualizer _mapVisualizer;
        private ISaveSystem _saveSystem;
        private IRegistry<ISaveLoad> _saveLoadRegistry;
        
        private ITileMap _tileMap;
        private DifficultyConfig _difficultyConfig;
        private EnumerationStrategy<Level> _enumerationStrategy;
        private TileTray _tileTray;
        private LevelData _levelData;

        private DifficultyConfig CurrentDifficultyConfig => _levelList.ConfigByDifficulty[_levelData.Difficulty];
        public Level CurrentLevel { get; private set; }
        public int LevelIndex => _levelData.LevelIndex;
        
        [Inject]
        private void Construct(
            LevelList levelList,
            TileMapGenerator tileMapGenerator,
            MapVisualizer mapVisualizer,
            ISaveSystem saveSystem,
            IRegistry<ISaveLoad> registry,
            TileTray tileTray)
        {
            _levelList = levelList;
            _tileMapGenerator = tileMapGenerator;
            _mapVisualizer = mapVisualizer;
            _saveSystem = saveSystem;
            _saveLoadRegistry = registry;
            _tileTray = tileTray;
        }
        
        public void Initialize() => _saveLoadRegistry.Register(this);

        public void Save() => _saveSystem.Save(SaveKeys.LevelData, _levelData);

        public async UniTask Load()
        {
            _levelData = await _saveSystem.Load<LevelData>(SaveKeys.LevelData);
            _difficultyConfig = CurrentDifficultyConfig;
            _enumerationStrategy = _difficultyConfig.GetStrategy();
            
            CurrentLevel = _difficultyConfig.Levels
                .FirstOrDefault(level => level.Id == _levelData.LevelID)
                           ?? _enumerationStrategy.Next();
        }

        public void StartLevel()
        {
            _tileMap = _tileMapGenerator.GenerateGrid(CurrentLevel.GeneratorConfig);
            _mapVisualizer.SpawnTiles(_tileMap);

            SubscribeOnLevelResult();
            LevelStarted?.Invoke();
        }

        public void StartNextLevel()
        {
            UnsubscribeFromLevelResult();
            SetNextLevel();
            StartLevel();
        }

        private void OnVictoryIfTileZero(Vector3Int _)
        {
            if (_tileMap.Count is 0)
            {
                UnsubscribeFromLevelResult();
                SetNextLevel();
                LevelFinished?.Invoke(LevelResult.Victory);
            }
        }

        private void SetNextLevel()
        {
            CurrentLevel = _enumerationStrategy.Next();
            int nextLevelIndex = _levelData.LevelIndex + 1;
            var difficulty = GetDifficultyByLevel(nextLevelIndex);

            if (_levelData.Difficulty != difficulty)
                _enumerationStrategy = _levelList.ConfigByDifficulty[difficulty].GetStrategy();
            
            _levelData = new(nextLevelIndex, difficulty, CurrentLevel.Id);
            Save();
        }

        private void OnDefeat()
        {
            UnsubscribeFromLevelResult();
            LevelFinished?.Invoke(LevelResult.Defeat);
        }
        
        private Difficulty GetDifficultyByLevel(int levelIndex)
        {
            int levelNumber = levelIndex + 1;
            int cumulativeLevels = 0;
        
            foreach (var config in _levelList.DifficultyConfigs)
            {
                cumulativeLevels += config.LevelsForNextDifficulty;

                if (levelNumber <= cumulativeLevels)
                    return config.Difficulty;
            }

            return _levelList.DifficultiesEnum[^1];
        }

        public void Dispose()
        {
            _saveLoadRegistry.Unregister(this);
            _tileMap.TileTaken -= OnVictoryIfTileZero;
            LevelStarted = null;
            LevelFinished = null;
        }

        private void SubscribeOnLevelResult()
        {
            _tileMap.TileTaken += OnVictoryIfTileZero;
            _tileTray.Filled += OnDefeat;
        }

        private void UnsubscribeFromLevelResult()
        {
            _tileMap.TileTaken -= OnVictoryIfTileZero;
            _tileTray.Filled -= OnDefeat;
        }
    }
}
