using Playgama;
using System.Collections;
using System.IO;
using UnityEngine;
using Playgama.Modules.Platform;
using Playgama.Modules.Advertisement;
using System;
using Commons.Coroutines;
using Commons.Systems;
using Commons.Systems.AudioManager;
using Commons.Systems.Save;
using Playgama.Modules.Leaderboards;
using Zenject;

public class SDK
{
    private SaveSystem _saveSystem;
    private AudioManager _audioManager;
    
    public bool IsLoad { get; private set; }

    [Inject]
    private void Construct(ISaveSystem saveSystem, AudioManager audioManager)
    {
        _saveSystem  = (SaveSystem)saveSystem;
        _audioManager = audioManager;
    }

    private void Start() => Bridge.advertisement.interstitialStateChanged += OnInterstitialStateChanged;

    public IEnumerator InitRoutine(Action onInit = null)
    {
        //TODO setup language
        //Language.Instance.ChangeLanguage(Bridge.platform.language);
        Load();
        GameReady();
        yield return new WaitUntil(() => IsLoad);
        ShowBanner();
        onInit?.Invoke();
    }

    public void Save()
        => Bridge.storage.Set(SaveSystem.SaveFile, _saveSystem.JsonData);

    public void Load()
    {
#if UNITY_EDITOR
        IsLoad = true;
#else
        Bridge.storage.Get(SaveSerial.SaveFile, OnLoadComplete);
#endif
    }

    public void OnLoadComplete(bool success, string data)
    {
        if (success && !string.IsNullOrEmpty(data))
            File.WriteAllText(_saveSystem.Path, data);

        IsLoad = true;
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
                _audioManager.MuteMusic();
                _audioManager.MuteSounds();
                break;
            case InterstitialState.Closed:
            case InterstitialState.Failed:
                _audioManager.UnmuteMusic();
                _audioManager.UnmuteSounds();
                break;
            default:
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
