using System;
using Gameplay;
using Playgama.Modules.Advertisement;
using Zenject;

namespace UI
{
    public class DefeatController : IInitializable
    {
        private readonly DefeatWindow _window;
        private readonly GameManager _gameManager;
        private readonly SDK _sdk;

        public DefeatController(DefeatWindow window,
            GameManager gameManager,
            SDK sdk)
        {
            _window = window;
            _gameManager = gameManager;
            _sdk = sdk;
        }

        public void Initialize()
        {
            _window.Setup(RestartAfterAd);
        }

        private void RestartAfterAd()
        {
            _sdk.ShowFullScreenAd();
            _sdk.InterstitialStateChanged += OnInterstitialStateChanged;

            void OnInterstitialStateChanged(InterstitialState state)
            {
                if(state is InterstitialState.Closed or InterstitialState.Failed)
                {
                    _sdk.InterstitialStateChanged -= OnInterstitialStateChanged;
                    _window.Close();
                    _gameManager.Restart();
                }
            }
        }
    }
}
