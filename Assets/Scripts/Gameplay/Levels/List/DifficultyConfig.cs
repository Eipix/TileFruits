using System;
using System.Collections.Generic;
using _Commons.Scripts.EnumerationStrategies;
using NaughtyAttributes;
using UnityEngine;

namespace Gameplay.Levels
{
    [Serializable]
    public class DifficultyConfig
    {
        [field: SerializeField, Min(1f), HideIf(nameof(HideLevelsForNextDifficulty)), AllowNesting]
        public int LevelsForNextDifficulty { get; private set; }
        
        [field: SerializeField] public EnumerationMode Mode { get; private set; } = EnumerationMode.Shuffle;
        [field: SerializeField, ReadOnly] public Difficulty Difficulty { get; private set; }
        [field: SerializeField] public List<Level> Levels { get; private set; }

        [field: SerializeField, HideInInspector]
        public bool HideLevelsForNextDifficulty { get; set; }
        
        public DifficultyConfig(Difficulty difficulty) => Difficulty = difficulty;

        public EnumerationStrategy<Level> GetStrategy() => Mode.GetStrategy(Levels);
    }
}
