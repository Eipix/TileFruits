using System;
using System.Linq;
using _Commons.Scripts.EnumerationStrategies;
using Commons.Systems.Save;
using Constants;
using Gameplay.Tray;
using Generator;
using UnityEngine;
using Zenject;

namespace Gameplay.Levels
{
    public class LevelManager : IDisposable
    {
        public event Action LevelStarted;
        public event Action<LevelResult> LevelFinished;
        
        private LevelList _levelList;
        private TileMapGenerator _tileMapGenerator;
        private MapVisualizer _mapVisualizer;
        private ISaveSystem _saveSystem;
        
        private ITileMap _tileMap;
        private DifficultyConfig _difficultyConfig;
        private EnumerationStrategy<Level> _enumerationStrategy;
        private TileTray _tileTray;
        private LevelData _levelData;

        private DifficultyConfig CurrentDifficultyConfig =>
            _levelList.ConfigByDifficulty[_levelData.Difficulty];
        
        public Level CurrentLevel { get; private set; }
        public int LevelIndex => _levelData.LevelIndex;
        
        [Inject]
        private void Construct(
            LevelList levelList,
            TileMapGenerator tileMapGenerator,
            MapVisualizer mapVisualizer,
            ISaveSystem saveSystem,
            TileTray tileTray)
        {
            _levelList = levelList;
            _tileMapGenerator = tileMapGenerator;
            _mapVisualizer = mapVisualizer;
            _saveSystem = saveSystem;
            _tileTray = tileTray;
        }

        public void Initialize()
        {
            _levelData = _saveSystem.Get<LevelData>(SaveKeys.LevelData);
            _difficultyConfig = CurrentDifficultyConfig;
            _enumerationStrategy = _difficultyConfig.GetStrategy();

            int levelDifficultyIndex = GetLevelIndexInDifficulty(_levelData.LevelIndex);
            
            if(levelDifficultyIndex != 0)
                for (int i = 0; i < levelDifficultyIndex + 1; i++)
                    _enumerationStrategy.Next();
            
            Debug.Log($"{nameof(levelDifficultyIndex)}: {levelDifficultyIndex}");
            
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
            int nextLevelIndex = _levelData.LevelIndex + 1;
            var difficulty = GetDifficultyByLevel(nextLevelIndex);

            if (_levelData.Difficulty != difficulty)
                _enumerationStrategy = _levelList.ConfigByDifficulty[difficulty].GetStrategy();
            
            CurrentLevel = _enumerationStrategy.Next();
            _levelData = new(nextLevelIndex, difficulty, CurrentLevel.Id);
            _saveSystem.SetAndSave(SaveKeys.LevelData, _levelData);
        }

        private void OnDefeat()
        {
            UnsubscribeFromLevelResult();
            LevelFinished?.Invoke(LevelResult.Defeat);
        }

        private int GetLevelIndexInDifficulty(int levelIndex)
        {
            int cumulativeLevels = 0;

            foreach (var config in _levelList.DifficultyConfigs)
            {
                if (levelIndex <= cumulativeLevels + config.LevelsForNextDifficulty)
                    return levelIndex - cumulativeLevels;

                cumulativeLevels += config.LevelsForNextDifficulty;
            }
    
            return 0;
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
