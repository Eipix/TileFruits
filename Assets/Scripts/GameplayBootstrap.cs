using Commons.Systems.AudioManager;
using Commons.Systems.Save;
using Commons.Systems.SaveManager;
using Constants;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class GameplayBootstrap : IInitializable
{
    private SDK _sdk;
    private AudioManager _audioManager;
    private ISaveSystem _saveSystem;
    private SaveManager _saveManager;
    
    [Inject]
    private void Construct(SDK sdk, AudioManager audioManager,
        ISaveSystem saveSystem, SaveManager saveManager)
    {
        _sdk = sdk;
        _audioManager = audioManager;
        _saveSystem = saveSystem;
        _saveManager = saveManager;
    }

    public async void Initialize()
    {
        await LoadData();
        _sdk.Setup();
        Debug.Log("Initialized Gameplay Bootstrap");
    }

    private async UniTask LoadData()
    {
        await _saveManager.Load();

        var (muteSounds, muteMusic) = await UniTask.WhenAll(
            _saveSystem.Load(SaveKeys.MuteSound, false),
            _saveSystem.Load(SaveKeys.MuteMusic, false)
        );
        
        _audioManager.MuteSounds = muteSounds;
        _audioManager.MuteMusic = muteMusic;
    }
}
