using Commons.Systems.AudioManager;
using Commons.Systems.PauseManager;
using Commons.Systems.Save;
using Commons.Systems.SaveManager;
using Gameplay;
using Generator;
using Input;
using Input.Levels;
using UnityEngine;
using WebGLCommons.Scripts;
using Zenject;

namespace DefaultNamespace
{
    public class CoreInstaller : MonoInstaller
    {
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private AudioManager _audioManagerPrefab;

        public override void InstallBindings()
        {
            Container.Bind<ISaveSystem>().To<SDKSaveSystem>().AsSingle();
            Container.Bind<SaveManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SDK>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PauseManager>().AsSingle().NonLazy();
            
            Container.Bind<AudioManager>().FromComponentInNewPrefab(_audioManagerPrefab)
                .AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<GameplayBootstrap>().AsSingle().NonLazy();
            
            Container.Bind<PlayerInput>().AsSingle()
                .OnInstantiated<PlayerInput>((_, instance) => instance.Enable())
                .NonLazy();
            
            Container.BindInterfacesAndSelfTo<TileClickDetector>().AsSingle().NonLazy();
            
            Container.Bind<TileMapGenerator>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<TileFactory>()
                .AsSingle()
                .WithArguments(_tilePrefab)
                .NonLazy();

            Container.Bind<LevelManager>().AsSingle().NonLazy();
            Container.Bind<GameManager>().AsSingle().NonLazy();
        }
    }
}
