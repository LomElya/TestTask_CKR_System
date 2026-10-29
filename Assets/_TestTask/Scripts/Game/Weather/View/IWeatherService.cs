using System.Threading;
using Cysharp.Threading.Tasks;

namespace TestTask_CKR_System.Game.Weather
{
    public interface IWeatherService
    {
        int WeatherPollingIntervalSeconds { get; }
        UniTask<WeatherSnapshot> GetForecastAsync(CancellationToken cancellationToken);
        UniTask<byte[]> GetImageAsync(string url, CancellationToken cancellationToken);
    }
}