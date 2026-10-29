using TestTask_CKR_System.Game.Weather.Config;
using TestTask_CKR_System.Game.Weather.View;
using UnityEngine;
using Zenject;

namespace TestTask_CKR_System.Game.Weather.Installer
{
    public class WeatherInstaller : MonoInstaller
    {
        [SerializeField] private WeatherConfig _config;
        [SerializeField] private WeatherView _view;

        public override void InstallBindings()
        {
            Container.Bind<IWeatherConfig>()
                .FromInstance(_config)
                .AsSingle();

            Container.BindMemoryPool<WeatherPeriodView, WeatherPeriodView.Pool>()
                .WithInitialSize(_config.InitialPeriodPoolSize)
                .FromComponentInNewPrefab(_config.PeriodPrefab)
                .UnderTransform(_view.PeriodsRoot);

            Container.BindMemoryPool<WeatherDayView, WeatherDayView.Pool>()
                .WithInitialSize(_config.InitialDayPoolSize)
                .FromComponentInNewPrefab(_config.DayPrefab)
                .UnderTransform(_view.DayRoot);

            Container.Bind<WeatherView>()
                .FromInstance(_view)
                .AsSingle();

            Container.Bind<IWeatherService>()
                .To<WeatherService>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<WeatherPresenter>()
                .AsSingle()
                .NonLazy();
        }
    }
}