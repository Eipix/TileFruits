using Commons.Systems.AudioManager;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Audio
{
    [CreateAssetMenu(fileName = "Audio/GameAudios")]
    public class AudioConfig : ScriptableObjectInstaller
    {
        [field: SerializeField] public AudioClip Theme { get; private set; }
        
        [field: Space(20), Header("Level")]
        [field: SerializeField] public SoundSettings LevelCompleted { get; private set; }
        [field: SerializeField] public SoundSettings LevelFailed { get; private set; }
        [field: SerializeField] public SoundSettings LevelStarted { get; private set; }
        
        [field: Space(20), Header("Tiles")]
        [field: SerializeField] public SoundSettings TileMatches { get; private set; }
        [field: SerializeField] public SoundSettings TileBlocked { get; private set; }
        [field: SerializeField] public SoundSettings TileMoveToTray { get; private set; }

        public override void InstallBindings()
            => Container.BindInstance(this).AsSingle().NonLazy();
    }
}
