namespace TestTask_CKR_System.Game.Weather
{
    public static class WeatherExtensions
    {
        public const string DateLittleFormat = "dd.MM";
        public const string DateFullFormat = "dd.MM.yyyy";

        public static string GetTemperatureString(this TemperatureDisplayUnit unit) =>
            unit == TemperatureDisplayUnit.Celsius ? "°C" : "°F";

        public static string GetPartName(this WeatherDayPart part) =>
             part switch
             {
                 WeatherDayPart.Day => "День",
                 WeatherDayPart.Night => "Ночь",
                 _ => string.Empty
             };

        public static string GetLittleDateString(this System.DateTime date) => GetDateString(date, DateLittleFormat);
        public static string GetFullDateString(this System.DateTime date) => GetDateString(date, DateFullFormat);
        public static string GetDateString(this System.DateTime date, string dateFormat) => date.ToString(dateFormat);

    }
}