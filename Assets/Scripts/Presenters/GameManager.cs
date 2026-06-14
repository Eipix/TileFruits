using Gameplay.Levels;
using Gameplay.Tray;
using Zenject;

namespace Gameplay
{
    public class GameManager
    {
        private LevelManager _levelManager;
        private TileTray _tileTray;

        [Inject]
        private void Construct(LevelManager levelManager, TileTray tileTray)
        {
            _levelManager = levelManager;
            _tileTray = tileTray;
        }
        
        public void Restart()
        {
            _tileTray.Clear();
            _levelManager.StartLevel();
        }
    }
}
