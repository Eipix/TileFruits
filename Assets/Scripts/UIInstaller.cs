using _Commons.Scripts.Effects.Shakers;
using _Commons.Scripts.UI;
using Commons;
using Gameplay;
using NaughtyAttributes;
using UI.Settings;
using UI.Tray;
using UnityEngine;
using View.Animations;
using View.Windows.Collection;
using Zenject;

namespace UI
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField, Foldout("Animations")]
        private CollectAnimationConfig _collectAnimationConfig;
        
        [SerializeField, Foldout("Animations")]
        private ShakerConfig _tileShakerConfig;
        
        [SerializeField, Foldout("Animations")]
        private ShakerConfig _collectionShakerConfig;
        
        [SerializeField, Foldout("Animations")]
        private HideAnimationConfig _hideAnimationConfig;
        
        [SerializeField] private CollectionItem _collectionItemPrefab;
        
        [SerializeField] private Hud _hud;
        [SerializeField] private SettingsWindow _settingsWindow;
        [SerializeField] private VictoryWindow _victoryWindow;
        [SerializeField] private DefeatWindow _defeatWindow;
        [SerializeField] private CollectionWindow _collectionWindow;
        
        public override void InstallBindings()
        {
            BindFactories();
            
            Container.Bind<Hud>().FromInstance(_hud);
            Container.BindInterfacesAndSelfTo<HudController>().AsSingle().NonLazy();
            
            BindWindow<SettingsWindow, SettingsController>(_settingsWindow);
            BindWindow<VictoryWindow, VictoryController>(_victoryWindow);
            BindWindow<DefeatWindow, DefeatController>(_defeatWindow);
            BindWindow<CollectionWindow, CollectionController>(_collectionWindow);
            
            Container.BindInterfacesAndSelfTo<UIManager>().AsSingle().NonLazy();
            
            BindAnimations();
        }

        private void BindFactories()
        {
            Container.BindFactory<TileConfig, RectTransform, CollectionItem, CollectionItem.Factory>()
                .FromComponentInNewPrefab(_collectionItemPrefab)
                .AsCached();
        }

        private void BindWindow<TWindow, TController>(TWindow windowPrefab) where TWindow : Window
        {
            Container.Bind(typeof(Window), typeof(TWindow))
                .FromInstance(windowPrefab).AsCached();
            
            Container.BindInterfacesAndSelfTo<TController>().AsSingle().NonLazy();
        }

        private void BindAnimations()
        {
            Container.Bind<ShakerConfig>()
                .FromInstance(_collectionShakerConfig)
                .AsSingle()
                .WhenInjectedInto<CollectionItemShaker>();
            
            Container.Bind<TileShaker>()
                .AsSingle()
                .WithArguments(_tileShakerConfig)
                .NonLazy();

            Container.Bind<CollectAnimationConfig>()
                .FromInstance(_collectAnimationConfig)
                .AsSingle()
                .WhenInjectedInto<TileTrayItem>();
            
            Container.Bind<HideAnimationConfig>()
                .FromInstance(_hideAnimationConfig)
                .AsSingle()
                .WhenInjectedInto<TileTrayItem>();
        }
    }
}
