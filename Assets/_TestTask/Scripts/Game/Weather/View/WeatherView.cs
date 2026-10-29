using System;
using TMPro;
using UnityEngine;

namespace TestTask_CKR_System.Game.Weather.View
{
    public class WeatherView : MonoBehaviour
    {
        [SerializeField] private WeatherStateView _state;
        [SerializeField] private TemperatureUnitView _temperatureUnits;
        [SerializeField] private WeatherDayTabsView _dayTabs;
        [SerializeField] private TMP_Text _dayTitle;
        [SerializeField] private WeatherPeriodListView _periods;

        public Transform PeriodsRoot => _periods.Content;
        public Transform DayRoot => _dayTabs.Content;

        public event Action<TemperatureDisplayUnit> UnitSelected
        {
            add => _temperatureUnits.Selected += value;
            remove => _temperatureUnits.Selected -= value;
        }

        public event Action<int> DaySelected
        {
            add => _dayTabs.Selected += value;
            remove => _dayTabs.Selected -= value;
        }

        public void SetLoading(bool isLoading)
        {
            _state.SetLoading(isLoading);
        }

        public void ShowWeather(
            WeatherSnapshot snapshot,
            int selectedDayIndex,
            TemperatureDisplayUnit unit)
        {
            WeatherDaySnapshot day = snapshot.Days[selectedDayIndex];
            _dayTabs.Show(snapshot.Days, selectedDayIndex);
            _temperatureUnits.Show(unit);
            _dayTitle.text = day.Date.GetFullDateString();
            _periods.Show(day.Periods, unit);
            _state.ShowContent();
        }

        public void SetIcon(int index, byte[] bytes) => _periods.SetIcon(index, bytes);

        public void SetTemperatureUnit(TemperatureDisplayUnit unit)
        {
            _temperatureUnits.Show(unit);
            _periods.SetTemperatureUnit(unit);
        }

        public void ChangeSelectedDay(WeatherSnapshot snapshot,
            int selectedDayIndex,
            TemperatureDisplayUnit unit)
        {
            WeatherDaySnapshot day = snapshot.Days[selectedDayIndex];
            _dayTabs.ChangeSelectedDay(selectedDayIndex);
            _dayTitle.text = day.Date.GetFullDateString();
            _periods.Show(day.Periods, unit);
        }

        public void ShowError(string message)
        {
            _state.ShowError(message);
        }
    }
}