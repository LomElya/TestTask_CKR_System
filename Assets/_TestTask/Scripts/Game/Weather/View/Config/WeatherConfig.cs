using TestTask_CKR_System.Game.Weather.View;
using UnityEngine;

namespace TestTask_CKR_System.Game.Weather.Config
{
    [CreateAssetMenu(fileName = "WeatherConfig", menuName = "TestTask CKR-System/WeatherConfig")]
    public class WeatherConfig : ScriptableObject, IWeatherConfig
    {
        [SerializeField, Min(1)] private int _maximumForecastDays = 7;
        [SerializeField, Min(1)] private int _weatherPollingIntervalSeconds = 5;

        [Header("Period Pool")]
        [SerializeField] private WeatherPeriodView _periodPrefab;
        [SerializeField] private int _initialPeriodPoolSize = 2;

        [Header("Day Pool")]
        [SerializeField] private WeatherDayView _weatherDayPrefab;
        [SerializeField] private int _initialDayPoolSize = 7;

        public int MaximumForecastDays => _maximumForecastDays;
        public int WeatherPollingIntervalSeconds => _weatherPollingIntervalSeconds;
        public WeatherPeriodView PeriodPrefab => _periodPrefab;
        public int InitialPeriodPoolSize => _initialPeriodPoolSize;
        public WeatherDayView DayPrefab => _weatherDayPrefab;
        public int InitialDayPoolSize => _initialDayPoolSize;
    }
}