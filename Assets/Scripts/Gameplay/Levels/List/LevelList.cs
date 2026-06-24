using System;
using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Generator.GenerationStrategies.Implementations;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace Gameplay.Levels
{
    [CreateAssetMenu(menuName = "Levels/Level List")]
    public class LevelList : ScriptableObjectInstaller, ISerializationCallbackReceiver
    {
        private const string InvalidMessage = "Invalid Difficulty Config: Ensure all difficulties contain" +
                                              "levels, there are no duplicate difficulties, and the list is " +
                                              "sorted in ascending order (Easy -> Medium -> Hard).";
        
        private readonly Dictionary<Difficulty, DifficultyConfig> _configByDifficulty = new();
        
        [SerializeField, ValidateInput(nameof(ValidateConfigs), InvalidMessage)]
        private List<DifficultyConfig> _difficultyConfigs;
        
        public Difficulty[] DifficultiesEnum { get; private set; } = (Difficulty[])Enum.GetValues(typeof(Difficulty));
        
        public IReadOnlyList<DifficultyConfig> DifficultyConfigs => _difficultyConfigs;
        public IReadOnlyDictionary<Difficulty, DifficultyConfig> ConfigByDifficulty => _configByDifficulty;

        #region Validation
        
        private void OnValidate()
        {
            if(_difficultyConfigs.Count is 0)
                InitList();

            foreach (var config in _difficultyConfigs)
            {
                var levels = config.Levels;
                
                for (int i = levels.Count - 1; i >= 0; i--)
                {
                    var level = levels[i];

                    if (level == null)
                    {
                        levels.Remove(level);
                        Debug.LogError("Can't add null levels");
                        continue;
                    }
                    
                    var strategy = level.GeneratorConfig.ShapeStrategy;
                    
                    if (strategy is CustomStrategy customConfig
                        && customConfig.IsValidAll(out string error) is false)
                    {
                        levels.Remove(level);
                        Debug.LogError($"Can't add levels with invalid strategies. Invalid level {level.name}\n Error {error}");
                    }
                }
            }
        }

        private void InitList()
        {
            foreach (var mode in DifficultiesEnum)
            {
                DifficultyConfig config = new(mode);
                _difficultyConfigs.Add(config);
            }
                
            _difficultyConfigs[^1].HideLevelsForNextDifficulty = true;
        }

        private bool ValidateConfigs()
        {
            for (int i = 0; i < DifficultiesEnum.Length; i++)
            {
                _difficultyConfigs[i].Difficulty = DifficultiesEnum[i];
                _difficultyConfigs[i].HideLevelsForNextDifficulty = false;
            }
            
            _difficultyConfigs[^1].HideLevelsForNextDifficulty = true;
            
            foreach (var config in _difficultyConfigs)
            {
                if(config.Levels.Count is 0)
                    return false;
            }

            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
            return true;
        }

        #endregion

        #region ISerializationCallbackReceiver
        
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            _configByDifficulty.Clear();

            foreach (var group in _difficultyConfigs)
                _configByDifficulty[group.Difficulty] = group;
        }
        
        #endregion

        public override void InstallBindings()
            => Container.Bind<LevelList>().FromInstance(this).AsSingle();
    }
}
