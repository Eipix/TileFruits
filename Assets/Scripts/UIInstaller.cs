using _Commons.Scripts.UI;
using Commons;
using Presenters__Controllers;
using UI.Settings;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private Hud _hud;
        [SerializeField] private SettingsWindow _settingsWindow;
        [SerializeField] private VictoryWindow _victoryWindow;
        [SerializeField] private DefeatWindow _defeatWindow;
        
        public override void InstallBindings()
        {
            Container.Bind<Hud>().FromInstance(_hud);
            Container.BindInterfacesAndSelfTo<HudController>().AsSingle().NonLazy();
            
            Container.Bind(typeof(Window), typeof(SettingsWindow))
                .FromInstance(_settingsWindow).AsCached();
            
            Container.BindInterfacesAndSelfTo<SettingsController>().AsSingle().NonLazy();
            
            Container.Bind(typeof(Window), typeof(VictoryWindow))
                .FromInstance(_victoryWindow).AsCached();
            
            Container.BindInterfacesAndSelfTo<VictoryController>().AsSingle().NonLazy();
            
            Container.Bind(typeof(Window), typeof(DefeatWindow))
                .FromInstance(_defeatWindow).AsCached();

            Container.BindInterfacesAndSelfTo<DefeatController>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<UIManager>().AsSingle().NonLazy();
        }
    }
}
