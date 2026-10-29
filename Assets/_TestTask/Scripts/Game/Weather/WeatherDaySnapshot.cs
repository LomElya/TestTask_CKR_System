using System;
using System.Collections.Generic;

namespace TestTask_CKR_System.Game.Weather
{
    public struct WeatherDaySnapshot
    {
        public WeatherDaySnapshot(
            DateTime date,
            IReadOnlyList<WeatherPeriodSnapshot> periods)
        {
            Date = date;
            Periods = periods ?? throw new ArgumentNullException(nameof(periods));
        }

        public DateTime Date { get; }
        public IReadOnlyList<WeatherPeriodSnapshot> Periods { get; }

        public WeatherPeriodSnapshot Find(WeatherDayPart part)
        {
            foreach (var period in Periods)
            {
                if (period.Part == part)
                    return period;
            }

            return null;
        }
    }
}