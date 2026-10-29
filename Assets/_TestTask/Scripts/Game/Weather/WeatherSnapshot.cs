using System;
using System.Collections.Generic;

namespace TestTask_CKR_System.Game.Weather
{
    public class WeatherSnapshot
    {
        public WeatherSnapshot(IReadOnlyList<WeatherDaySnapshot> days)
        {
            Days = days ?? throw new ArgumentNullException(nameof(days));
        }

        public IReadOnlyList<WeatherDaySnapshot> Days { get; }
    }
}