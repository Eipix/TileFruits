using Playgama;
using Playgama.Modules.Platform;
using Playgama.Modules.Advertisement;
using Playgama.Modules.Leaderboards;
using System;
using UnityEngine;
using Zenject;

public class SDK : IInitializable, IDisposable
{
    public event Action<InterstitialState> InterstitialStateChanged;
    
    public string Language => Bridge.platform.language;

    public void Initialize()
        => Bridge.advertisement.interstitialStateChanged += OnInterstitialStateChanged;

    public void Dispose()
    {
        if (Bridge.advertisement != null)
            Bridge.advertisement.interstitialStateChanged -= OnInterstitialStateChanged;
    }

    public void Setup()
    {
        #if !UNITY_EDITOR
        ShowBanner();
#endif
        GameReady();
    }

    public void SetToLeaderBoard(int value, Action<bool> onComplete = null)
    {
        if (Bridge.leaderboards.type is LeaderboardType.NotAvailable)
        {
            onComplete?.Invoke(false);
            return;
        }

        // TODO setup leaderboard
        
        Bridge.leaderboards.SetScore("leaderboardName", value, onComplete);
    }

    public void ShowFullScreenAd() => Bridge.advertisement.ShowInterstitial();

    public void ShowBanner() => Bridge.advertisement.ShowBanner();

    private void OnInterstitialStateChanged(InterstitialState state)
    {
        switch (state)
        {
            case InterstitialState.Loading:
            case InterstitialState.Opened:
                AudioListener.pause = true;
                break;
            case InterstitialState.Closed:
            case InterstitialState.Failed:
                AudioListener.pause = false;
                break;
        }
        InterstitialStateChanged?.Invoke(state);
    }

    private void GameReady()
    {
        switch (Bridge.platform.id)
        {
            case "crazy_games":
                Bridge.platform.SendMessage(PlatformMessage.LevelStarted);
                break;
            default:
                Bridge.platform.SendMessage(PlatformMessage.GameReady);
                break;
        }
    }
}
