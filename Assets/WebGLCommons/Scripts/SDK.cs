using Playgama;
using System.Collections;
using Playgama.Modules.Platform;
using Playgama.Modules.Advertisement;
using System;
using Commons.Systems.AudioManager;
using Playgama.Modules.Leaderboards;
using Zenject;

public class SDK : IInitializable
{
    private AudioManager _audioManager;

    [Inject]
    private void Construct(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public void Initialize()
        => Bridge.advertisement.interstitialStateChanged += OnInterstitialStateChanged;

    public void Setup()
    {
        //TODO setup language
        //Language.Instance.ChangeLanguage(Bridge.platform.language);
        GameReady();
        ShowBanner();
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
                _audioManager.MuteMusic = true;
                _audioManager.MuteSounds = true;
                break;
            case InterstitialState.Closed:
            case InterstitialState.Failed:
                _audioManager.MuteMusic = false;
                _audioManager.MuteSounds = false;
                break;
        }
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
