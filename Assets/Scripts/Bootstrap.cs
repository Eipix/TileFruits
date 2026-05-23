using System.Collections;
using Commons.Systems.AudioManager;
using Commons.Systems.Save;
using Constants;
using UnityEngine;
using Zenject;

public class Bootstrap : MonoBehaviour
{
    private SDK _sdk;
    private AudioManager _audioManager;
    private ISaveSystem _saveSystem;
    
    [Inject]
    private void Construct(SDK sdk, AudioManager audioManager, ISaveSystem saveSystem)
    {
        _sdk = sdk;
        _audioManager = audioManager;
        _saveSystem = saveSystem;
    }

    private IEnumerator Start()
    {
        yield return _sdk.InitRoutine();
        _audioManager.MuteSounds = _saveSystem.Load(SaveKeys.MuteSound, false);
        _audioManager.MuteMusic = _saveSystem.Load(SaveKeys.MuteMusic, false);
    }
}
