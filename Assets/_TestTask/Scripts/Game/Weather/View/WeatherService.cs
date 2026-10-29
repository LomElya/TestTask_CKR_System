using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TestTask_CKR_System.Core.Networking;
using TestTask_CKR_System.Core.Networking.Enum;
using TestTask_CKR_System.Core.Networking.Interfaces;
using TestTask_CKR_System.Game.Weather.Config;
using UnityEngine;

namespace TestTask_CKR_System.Game.Weather
{
    public class WeatherService : IWeatherService
    {
        private readonly IHttpClient _httpClient;
        private readonly IWeatherConfig _config;
        private readonly IRemoteImageService _imageService;

        public int WeatherPollingIntervalSeconds => _config.WeatherPollingIntervalSeconds;

        public WeatherService(IHttpClient httpClient, IWeatherConfig config, IRemoteImageService remoteImageService)
        {
            _httpClient = httpClient;
            _config = config;
            _imageService = remoteImageService;
        }

        public async UniTask<WeatherSnapshot> GetForecastAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.Get<WeatherResponse>(
                EndpointType.WeatherForecast,
                cancellationToken: cancellationToken);
            if (response?.properties?.periods == null || response.properties.periods.Length == 0)
                throw new Exception("Weather response contains no forecast periods.");

            var parsedPeriods = ParsePeriods(response.properties.periods);
            if (parsedPeriods.Count == 0)
                throw new Exception("Weather response contains no valid forecast dates.");

            var days = new List<WeatherDaySnapshot>();
            foreach (var dateGroup in parsedPeriods
                         .GroupBy(item => item.Time.Date)
                         .OrderBy(group => group.Key)
                         .Take(_config.MaximumForecastDays))
            {
                var periods = dateGroup
                    .OrderBy(item => item.Time)
                    .GroupBy(item => item.Source.isDaytime)
                    .Select(group => CreatePeriod(group.First()))
                    .ToList();

                days.Add(new WeatherDaySnapshot(dateGroup.Key, periods));
            }

            return new WeatherSnapshot(days);
        }

        public async UniTask<byte[]> GetImageAsync(string url, CancellationToken cancellationToken)
        {
            return await _imageService.GetAsync(url, cancellationToken);
        }

        private static List<ParsedPeriod> ParsePeriods(IEnumerable<WeatherPeriod> periods)
        {
            var result = new List<ParsedPeriod>();
            foreach (var period in periods)
            {
                if (period == null ||
                    !DateTimeOffset.TryParse(
                        period.startTime,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind,
                        out var time) ||
                    !DateTimeOffset.TryParse(
                        period.endTime,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind,
                        out var endTime))
                    continue;

                result.Add(new ParsedPeriod(time, endTime, period));
            }

            return result;
        }

        private WeatherPeriodSnapshot CreatePeriod(ParsedPeriod selected)
        {
            var period = selected.Source;
            return new WeatherPeriodSnapshot(
                period.isDaytime ? WeatherDayPart.Day : WeatherDayPart.Night,
                period.name,
                selected.Time,
                selected.EndTime,
                period.temperature,
                period.temperatureUnit,
                period.temperatureTrend,
                period.shortForecast,
                period.detailedForecast,
                period.windSpeed,
                period.windDirection,
                GetPercentage(period.probabilityOfPrecipitation),
                period.icon);
        }

        private int GetPercentage(WeatherValue value)
        {
            return value == null ? 0 : Mathf.Clamp(Mathf.RoundToInt(value.value), 0, 100);
        }

        [Serializable]
        private class WeatherResponse
        {
            public WeatherProperties properties;
        }

        [Serializable]
        private class WeatherProperties
        {
            public WeatherPeriod[] periods;
        }

        [Serializable]
        private class WeatherPeriod
        {
            public string startTime;
            public string endTime;
            public string name;
            public bool isDaytime;
            public int temperature;
            public string temperatureUnit;
            public string temperatureTrend;
            public string icon;
            public string shortForecast;
            public string detailedForecast;
            public string windSpeed;
            public string windDirection;
            public WeatherValue probabilityOfPrecipitation;
        }

        [Serializable]
        private class WeatherValue
        {
            public float value;
        }

        private class ParsedPeriod
        {
            public ParsedPeriod(
                DateTimeOffset time,
                DateTimeOffset endTime,
                WeatherPeriod source)
            {
                Time = time;
                EndTime = endTime;
                Source = source;
            }

            public DateTimeOffset Time { get; }
            public DateTimeOffset EndTime { get; }
            public WeatherPeriod Source { get; }
        }
    }
}