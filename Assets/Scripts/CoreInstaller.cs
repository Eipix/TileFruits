using Commons.Systems.AudioManager;
using Commons.Systems.PauseManager;
using Commons.Systems.Save;
using Gameplay;
using Gameplay.Levels;
using Gameplay.Tray;
using Generator;
using Generator.Provider;
using Input;
using NaughtyAttributes;
using Presenters__Controllers;
using UI.Tray;
using UnityEngine;
using View;
using View.Effects;
using WebGLCommons.Scripts;
using Zenject;

namespace DefaultNamespace
{
    public class CoreInstaller : MonoInstaller
    {
        [SerializeField, Min(1)] private int _tilePoolInitialSize = 50;
        
        [SerializeField, BoxGroup("Tray")] private TileTraySettings _tileTraySettings;
        [SerializeField, BoxGroup("Tray")] private TileTrayView _tileTrayView;
        [SerializeField, BoxGroup("Tray")] private TileTrayItem _tileTrayItemPrefab;
        [SerializeField, BoxGroup("Tray")] private RectTransform _tileTrayItemPoolParent;
        
        [SerializeField] private MapVisualizer _mapVisualizer;
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private AudioManager _audioManagerPrefab;
        [SerializeField] private MatchEffect _matchEffectPrefab;

        public override void InstallBindings()
        {
            Container.Bind(typeof(Camera), typeof(CameraZoomController))
                .FromComponentOn(Camera.main!.gameObject)
                .AsSingle();
            
            BindSystems();
            BindPools();
            BindPresenters();
            
            Container.BindInterfacesAndSelfTo<GameplayBootstrap>().AsSingle().NonLazy();
            
            Container.Bind<PlayerInput>().AsSingle()
                .OnInstantiated<PlayerInput>((_, instance) => instance.Enable())
                .NonLazy();

            Container.Bind<ITileMapProvider>().To<TileMapProvider>().AsSingle();

            Container.Bind<TileMapProvider>()
                .FromMethod(ctx => (TileMapProvider)ctx.Container.Resolve<ITileMapProvider>())
                .WhenInjectedInto<TileMapGenerator>();
            
            Container.Bind<TileClickDetector>().AsSingle().NonLazy();
            
            Container.Bind<TileMapGenerator>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<MapVisualizer>()
                .FromInstance(_mapVisualizer)
                .AsSingle()
                .NonLazy();

            Container.Bind<TileTraySettings>().FromInstance(_tileTraySettings);
            
            Container.BindInterfacesAndSelfTo<TileTray>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<TileTrayView>().FromInstance(_tileTrayView).NonLazy();
        }

        public void BindPresenters()
        {
            Container.BindInterfacesAndSelfTo<LevelManager>().AsSingle().NonLazy();
            Container.Bind<GameManager>().AsSingle().NonLazy();

            Container.Bind<GameplayController>().AsSingle().NonLazy();
            Container.Bind<TileTrayPresenter>().AsSingle().NonLazy();
        }

        public void BindSystems()
        {
            Container.Bind<ISaveSystem>().To<SDKSaveSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<SDK>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PauseManager>().AsSingle().NonLazy();
            
            Container.Bind<AudioManager>().FromComponentInNewPrefab(_audioManagerPrefab)
                .AsSingle().NonLazy();
        }

        public void BindPools()
        {
            Container.BindMemoryPool<MatchEffect, MatchEffect.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_matchEffectPrefab)
                .UnderTransformGroup("MatchEffect Pool")
                .AsCached();
            
            Container.BindMemoryPool<Tile, Tile.Pool>()
                .WithInitialSize(_tilePoolInitialSize)
                .FromComponentInNewPrefab(_tilePrefab)
                .UnderTransformGroup("Tile Pool")
                .AsCached();
            
            Container.BindMemoryPool<TileTrayItem, TileTrayItem.Pool>()
                .WithInitialSize(_tileTraySettings.Capacity)
                .FromComponentInNewPrefab(_tileTrayItemPrefab)
                .UnderTransform(_tileTrayItemPoolParent)
                .AsCached();
        }
    }
}
