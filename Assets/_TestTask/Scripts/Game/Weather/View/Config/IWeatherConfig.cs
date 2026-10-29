using TestTask_CKR_System.Game.Weather.View;

namespace TestTask_CKR_System.Game.Weather.Config
{
    public interface IWeatherConfig
    {
        int MaximumForecastDays { get; }
        int WeatherPollingIntervalSeconds { get; }
        WeatherPeriodView PeriodPrefab { get; }
        int InitialPeriodPoolSize { get; }
        WeatherDayView DayPrefab { get; }
        int InitialDayPoolSize { get; }
    }
}