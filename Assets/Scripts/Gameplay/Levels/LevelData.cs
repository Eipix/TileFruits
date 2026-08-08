using System;
using OdinSerializer;

namespace Gameplay.Levels
{
    [Serializable]
    public readonly struct LevelData
    {
        [OdinSerialize, NonSerialized] public readonly int LevelIndex;
        [OdinSerialize, NonSerialized] public readonly Difficulty Difficulty;
        [OdinSerialize, NonSerialized] public readonly string LevelID;
        
        public LevelData(int levelIndex, Difficulty difficulty, string levelID) =>
            (LevelIndex, Difficulty, LevelID) = (levelIndex, difficulty, levelID);
    }
}
