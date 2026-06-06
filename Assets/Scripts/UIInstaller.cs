using _Commons.Scripts.Effects.Shakers;
using _Commons.Scripts.UI;
using Commons;
using Gameplay;
using NaughtyAttributes;
using Presenters__Controllers;
using UI.Settings;
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
            Container.BindInterfacesAndSelfTo<TileShaker>()
                .AsSingle()
                .WithArguments(_tileShakerConfig)
                .NonLazy();
            
            /*Container.BindInterfacesAndSelfTo<TileTrayAnimations>()
                .AsSingle()
                .WithArguments(_collectAnimationConfig)
                .NonLazy();*/
        }
    }
}
