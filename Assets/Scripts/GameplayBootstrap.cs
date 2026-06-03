using System;
using Commons.Systems.AudioManager;
using Commons.Systems.Save;
using Commons.Systems.SaveManager;
using Constants;
using Cysharp.Threading.Tasks;
using Gameplay.Levels;
using UnityEngine;
using View.Windows.Collection;
using Zenject;

public class GameplayBootstrap : IInitializable
{
    private SDK _sdk;
    private AudioManager _audioManager;
    private ISaveSystem _saveSystem;
    private SaveManager _saveManager;
    private LevelManager _levelManager;

    [Inject]
    private void Construct(SDK sdk, AudioManager audioManager,
        ISaveSystem saveSystem, SaveManager saveManager,
        LevelManager levelManager)
    {
        _sdk = sdk;
        _audioManager = audioManager;
        _saveSystem = saveSystem;
        _saveManager = saveManager;
        _levelManager = levelManager;
    }

    public async void Initialize()
    {
        try
        {
            await LoadData();
            _sdk.Setup();
        
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
}
