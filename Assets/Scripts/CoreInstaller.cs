using Commons.Systems;
using Commons.Systems.AudioManager;
using Commons.Systems.PauseManager;
using Commons.Systems.Save;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class CoreInstaller : MonoInstaller
    {
        [SerializeField] private AudioManager _audioManagerPrefab;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PauseManager>().AsSingle().NonLazy();
            Container.Bind<ISaveSystem>().To<SaveSystem>().AsSingle().NonLazy();
            Container.Bind<AudioManager>().FromComponentInNewPrefab(_audioManagerPrefab)
                .AsSingle().NonLazy();
        }
    }
}
