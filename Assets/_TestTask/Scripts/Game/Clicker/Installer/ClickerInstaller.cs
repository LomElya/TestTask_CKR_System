using TestTask_CKR_System.Game.Clicker.Animation;
using TestTask_CKR_System.Game.Clicker.Config;
using UnityEngine;
using Zenject;

namespace TestTask_CKR_System.Game.Clicker.Installer
{
    public class ClickerInstaller : MonoInstaller
    {
        [SerializeField] private ClickerBalanceConfig _config;
        [SerializeField] private ClickerView _clickerView;

        public override void InstallBindings()
        {
            Container.BindInstance(_clickerView).AsSingle();

            Container.Bind<IClickerBalanceConfig>().FromInstance(_config).AsSingle();
            Container.Bind<IClickerAnimationPlayer>().To<DOTweenClickerAnimationPlayer>().AsSingle();
            
            Container.BindMemoryPool<CurrencyFlyView, CurrencyFlyView.Pool>()
                .WithInitialSize(_config.CurrencyFlyViewPoolSize)
                .FromComponentInNewPrefab(_config.CurrencyFlyViewPrefab)
                .UnderTransform(_clickerView.CurrencyFxRoot);

            Container.Bind<IClickerModel>()
                .To<ClickerModel>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<ClickerPresenter>()
                .AsSingle()
                .NonLazy();
        }
    }
}