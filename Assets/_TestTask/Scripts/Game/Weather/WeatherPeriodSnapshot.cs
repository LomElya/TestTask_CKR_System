using System;

namespace TestTask_CKR_System.Game.Weather
{
    public class WeatherPeriodSnapshot
    {
        public WeatherPeriodSnapshot(
            WeatherDayPart part,
            string name,
            DateTimeOffset time,
            DateTimeOffset endTime,
            int temperature,
            string temperatureUnit,
            string temperatureTrend,
            string shortForecast,
            string detailedForecast,
            string windSpeed,
            string windDirection,
            int precipitationPercent,
            string iconUrl)
        {
            Part = part;
            Name = name;
            Time = time;
            EndTime = endTime;
            Temperature = temperature;
            TemperatureUnit = temperatureUnit;
            TemperatureTrend = temperatureTrend;
            ShortForecast = shortForecast;
            DetailedForecast = detailedForecast;
            WindSpeed = windSpeed;
            WindDirection = windDirection;
            PrecipitationPercent = precipitationPercent;
            IconUrl = iconUrl;
        }

        public WeatherDayPart Part { get; }
        public string Name { get; }
        public DateTimeOffset Time { get; }
        public DateTimeOffset EndTime { get; }
        public int Temperature { get; }
        public string TemperatureUnit { get; }
        public string TemperatureTrend { get; }
        public string ShortForecast { get; }
        public string DetailedForecast { get; }
        public string WindSpeed { get; }
        public string WindDirection { get; }
        public int PrecipitationPercent { get; }
        public string IconUrl { get; }
        public byte[] IconBytes { get; private set; }

        internal void SetIconBytes(byte[] iconBytes)
        {
            IconBytes = iconBytes;
        }

        public int GetTemperature(TemperatureDisplayUnit displayUnit)
        {
            if (displayUnit == TemperatureDisplayUnit.Celsius)
                return (int)Math.Round((Temperature - 32f) * 5f / 9f, MidpointRounding.AwayFromZero);

            if (displayUnit == TemperatureDisplayUnit.Fahrenheit)
                return (int)Math.Round(Temperature * 9f / 5f + 32f, MidpointRounding.AwayFromZero);

            return Temperature;
        }
    }
}