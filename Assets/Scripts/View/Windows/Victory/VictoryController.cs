using Gameplay.Levels;
using Playgama.Modules.Advertisement;
using Zenject;

namespace UI
{
    public class VictoryController : IInitializable
    {
        private readonly VictoryWindow _window;
        private readonly LevelManager _levelManager;
        private readonly SDK _sdk;

        public VictoryController(VictoryWindow window,
            LevelManager levelManager,
            SDK sdk)
        {
            _window = window;
            _levelManager = levelManager;
            _sdk = sdk;
        }

        public void Initialize() => _window.Setup(StartAfterAd);

        private void StartAfterAd()
        {
            _sdk.ShowFullScreenAd();
            _sdk.InterstitialStateChanged += OnInterstitialStateChanged;

            void OnInterstitialStateChanged(InterstitialState state)
            {
                if (state is InterstitialState.Closed or InterstitialState.Failed)
                {
                    _sdk.InterstitialStateChanged -= OnInterstitialStateChanged;
                    _window.Close();
                    _levelManager.StartLevel();
                }
            }
        }
    }
}
