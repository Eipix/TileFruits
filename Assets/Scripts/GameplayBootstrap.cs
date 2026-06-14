using System;
using Commons.Systems.AudioManager;
using Commons.Systems.Save;
using Commons.Systems.SaveManager;
using Constants;
using Cysharp.Threading.Tasks;
using Gameplay;
using Gameplay.Levels;
using Presenters__Controllers;
using UnityEngine;
using View.Animations;
using View.Windows.Collection;
using Zenject;

public class GameplayBootstrap : IInitializable, IDisposable
{
    private SDK _sdk;
    private AudioManager _audioManager;
    private ISaveSystem _saveSystem;
    private SaveManager _saveManager;
    private LevelManager _levelManager;
    private GameplayController _gameplayController;
    private TileTrayPresenter _trayPresenter;
    private TileClickDetector _tileClickDetector;

    [Inject]
    private void Construct(SDK sdk, AudioManager audioManager,
        ISaveSystem saveSystem, SaveManager saveManager,
        LevelManager levelManager,
        GameplayController gameplayController,
        TileTrayPresenter trayPresenter,
        TileClickDetector tileClickDetector)
    {
        _sdk = sdk;
        _audioManager = audioManager;
        _saveSystem = saveSystem;
        _saveManager = saveManager;
        _levelManager = levelManager;
        _gameplayController = gameplayController;
        _trayPresenter = trayPresenter;
        _tileClickDetector = tileClickDetector;
    }

    public async void Initialize()
    {
        try
        {
            await LoadData();
            _sdk.Setup();
        
            _tileClickDetector.Initialize();
            _trayPresenter.Initialize();
            _gameplayController.Initialize();
            _levelManager.StartLevel();
            Debug.Log("Initialized Gameplay Bootstrap");
        }
        catch (Exception e)
        {
            Debug.LogError("Initialize error: " + e);
        }
    }

    private async UniTask LoadData()
    {
        var (muteSounds, muteMusic) = await UniTask.WhenAll(
            _saveSystem.Load(SaveKeys.MuteSound_Bool, false),
            _saveSystem.Load(SaveKeys.MuteMusic_Bool, false)
        );
        
        _audioManager.MuteSounds = muteSounds;
        _audioManager.MuteMusic = muteMusic;
        
        await _saveManager.Load();
    }

    public void Dispose()
    {
        _tileClickDetector.Dispose();
        _trayPresenter.Dispose();
        _gameplayController.Dispose();
    }
}
