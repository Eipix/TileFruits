using System;
using Assets.SimpleLocalization.Scripts;
using Audio;
using Commons.Systems.AudioManager;
using Commons.Systems.Save;
using Constants;
using Cysharp.Threading.Tasks;
using Gameplay;
using Gameplay.Levels;
using Presenters__Controllers;
using UnityEngine;
using Zenject;

public class GameplayBootstrap : IInitializable, IDisposable
{
    private SDK _sdk;
    private AudioManager _audioManager;
    private ISaveSystem _saveSystem;
    private LevelManager _levelManager;
    private GameplayController _gameplayController;
    private TileTrayPresenter _trayPresenter;
    private TileClickDetector _tileClickDetector;
    private AudioPresenter _audioPresenter;

    [Inject]
    private void Construct(SDK sdk, AudioManager audioManager,
        ISaveSystem saveSystem,
        LevelManager levelManager,
        GameplayController gameplayController,
        TileTrayPresenter trayPresenter,
        TileClickDetector tileClickDetector,
        AudioPresenter audioPresenter)
    {
        _sdk = sdk;
        _audioManager = audioManager;
        _saveSystem = saveSystem;
        _levelManager = levelManager;
        _gameplayController = gameplayController;
        _trayPresenter = trayPresenter;
        _tileClickDetector = tileClickDetector;
        _audioPresenter = audioPresenter;
    }

    public async void Initialize()
    {
        try
        {
            await LoadData();
            LocalizationManager.Language = _sdk.Language;
            
            _tileClickDetector.Initialize();
            _trayPresenter.Initialize();
            _gameplayController.Initialize();
            _levelManager.Initialize();
            
            _levelManager.StartLevel();
            _audioPresenter.Initialize();
            
            _sdk.Setup();
        }
        catch (Exception e)
        {
            Debug.LogError("Initialize error: " + e);
        }
    }

    private async UniTask LoadData()
    {
        await _saveSystem.LoadAsync();
            
        _audioManager.MuteSounds = _saveSystem.Get(SaveKeys.MuteSound_Bool, false);
        _audioManager.MuteMusic = _saveSystem.Get(SaveKeys.MuteMusic_Bool, false);
    }

    public void Dispose()
    {
        _tileClickDetector?.Dispose();
        _trayPresenter?.Dispose();
        _gameplayController?.Dispose();
        _audioPresenter?.Dispose();
    }
}
