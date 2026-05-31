using Gameplay.Levels;
using Zenject;

namespace UI
{
    public class VictoryController : IInitializable
    {
        private readonly VictoryWindow _window;
        private readonly LevelManager _levelManager;

        public VictoryController(VictoryWindow window, LevelManager levelManager)
        {
            _window = window;
            _levelManager = levelManager;
        }

        public void Initialize()
        {
            _window.Setup(_levelManager.StartLevel);
        }
    }
}
