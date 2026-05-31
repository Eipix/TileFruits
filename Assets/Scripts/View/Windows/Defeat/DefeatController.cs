using Gameplay;
using Zenject;

namespace UI
{
    public class DefeatController : IInitializable
    {
        private readonly DefeatWindow _window;
        private readonly GameManager _gameManager;

        public DefeatController(DefeatWindow window, GameManager gameManager)
        {
            _window = window;
            _gameManager = gameManager;
        }

        public void Initialize()
        {
            _window.Setup(_gameManager.Restart);
        }
    }
}
