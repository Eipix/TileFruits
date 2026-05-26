using System;

namespace Input.Levels
{
    public class LevelManager
    {
        public event Action LevelStarted;
        public event Action LevelFinished;

        public void StartLevel()
        {
            
        }

        public bool IsLevelCompleted()
        {
            return false;
        }
    }
}
