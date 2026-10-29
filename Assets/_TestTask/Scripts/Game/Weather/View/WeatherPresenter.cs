using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TestTask_CKR_System.Common;
using TestTask_CKR_System.Core.Networking;
using TestTask_CKR_System.Game.Weather.View;
using UnityEngine;
using Zenject;

namespace TestTask_CKR_System.Game.Weather
{
    public class WeatherPresenter : ITabController, ITickable, IDisposable, IInitializable
    {
        public AppTab Tab => AppTab.Weather;

        private readonly IRequestQueue _requestQueue;
        private readonly IWeatherService _weatherService;
        private readonly WeatherView _view;
        private readonly string _ownerId = $"weather-{Guid.NewGuid():N}";
        private readonly string _imageOwnerId = $"weather-images-{Guid.NewGuid():N}";

        private bool _isActive;
        private int _imageRequestVersion;
        private int _activationVersion;
        private WeatherSnapshot _snapshot;
        private int _selectedDayIndex;
        private TemperatureDisplayUnit _displayUnit = TemperatureDisplayUnit.Celsius;
        private ITimer _timer = new CutomTimer();

        public WeatherPresenter(
            IRequestQueue requestQueue,
            IWeatherService weatherService,
            WeatherView view)
        {
            _requestQueue = requestQueue;
            _weatherService = weatherService;
            _view = view;
        }

        public void Initialize()
        {
            _view.UnitSelected += SelectUnit;
            _view.DaySelected += SelectDay;

            _timer.SetTime(_weatherService.WeatherPollingIntervalSeconds);
            _timer.TimeFinished += TimeFinished;
        }

        private void TimeFinished()
        {
            RequestWeatherAsync(_activationVersion);
        }

        public void Activate()
        {
            _timer.Start(_weatherService.WeatherPollingIntervalSeconds);
            _isActive = true;
            _activationVersion++;
            RequestWeatherAsync(_activationVersion);
        }

        public void Deactivate()
        {
            _timer.Stop();
            _isActive = false;
            _activationVersion++;
            _imageRequestVersion++;
            _requestQueue.CancelOwner(_ownerId);
            _requestQueue.CancelOwner(_imageOwnerId);
        }

        public void Tick()
        {
            _timer.Tick(Time.deltaTime);
        }

        public void Dispose()
        {
            _timer.Dispose();

            _isActive = false;
            _activationVersion++;
            _imageRequestVersion++;
            _requestQueue.CancelOwner(_ownerId);
            _requestQueue.CancelOwner(_imageOwnerId);

            if (_view != null)
            {
                _view.UnitSelected -= SelectUnit;
                _view.DaySelected -= SelectDay;
            }

            if (_timer != null)
            {
                _timer.TimeFinished -= TimeFinished;
            }
        }

        private async void RequestWeatherAsync(int activationVersion)
        {
            if (_snapshot == null)
                _view.SetLoading(true);

            var handle = _requestQueue.Enqueue(
                _ownerId,
                token => _weatherService.GetForecastAsync(token));

            try
            {
                var weather = await handle.Completion;
                if (_isActive && activationVersion == _activationVersion)
                {
                    _snapshot = weather;
                    _selectedDayIndex = Mathf.Clamp(
                        _selectedDayIndex,
                        0,
                        weather.Days.Count - 1);
                    Render();
                    RequestImagesAsync(weather, activationVersion);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Cancel Weather");
            }
            catch (Exception exception)
            {
                if (_isActive && activationVersion == _activationVersion)
                    _view.ShowError($"Не удалось загрузить погоду: {exception.Message}");
            }
            finally
            {
                if (_isActive && activationVersion == _activationVersion)
                {
                    _view.SetLoading(_snapshot == null);
                }
            }
        }

        private async void RequestImagesAsync(
                 WeatherSnapshot snapshot,
                 int activationVersion)
        {
            _imageRequestVersion++;
            var imageRequestVersion = _imageRequestVersion;
            _requestQueue.CancelOwner(_imageOwnerId);

            if (_selectedDayIndex < 0 || _selectedDayIndex >= snapshot.Days.Count)
                return;

            var selectedDay = snapshot.Days[_selectedDayIndex];
            var periods = selectedDay.Periods
                .Where(period =>
                    !string.IsNullOrWhiteSpace(period.IconUrl) &&
                    (period.IconBytes == null || period.IconBytes.Length == 0))
                .ToArray();
            if (periods.Length == 0)
                return;

            var handle = _requestQueue.Enqueue(
                _imageOwnerId,
                token => LoadImagesAsync(
                    periods,
                    snapshot,
                    selectedDay,
                    activationVersion,
                    imageRequestVersion,
                    token));

            try
            {
                await handle.Completion;
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async UniTask<bool> LoadImagesAsync(
           WeatherPeriodSnapshot[] visiblePeriods,
           WeatherSnapshot snapshot,
           WeatherDaySnapshot selectedDay,
           int activationVersion,
           int imageRequestVersion,
           CancellationToken cancellationToken)
        {
            var groups = visiblePeriods
                .GroupBy(period => period.IconUrl, StringComparer.Ordinal);

            var tasks = groups.Select(group => LoadImageAsync(
                group.Key,
                group.ToArray(),
                snapshot,
                selectedDay,
                activationVersion,
                imageRequestVersion,
                cancellationToken));

            await UniTask.WhenAll(tasks);

            for (int i = 0; i < visiblePeriods.Length; i++)
            {
                _view.SetIcon(i, visiblePeriods[i].IconBytes);
            }

            return true;
        }

        private async UniTask LoadImageAsync(
          string url,
          WeatherPeriodSnapshot[] periods,
          WeatherSnapshot snapshot,
          WeatherDaySnapshot selectedDay,
          int activationVersion,
          int imageRequestVersion,
          CancellationToken cancellationToken)
        {
            try
            {
                var bytes = await _weatherService.GetImageAsync(url, cancellationToken);
                if (!IsCurrentImageRequest(
                        snapshot,
                        selectedDay,
                        activationVersion,
                        imageRequestVersion))
                    return;

                for (int i = 0; i < periods.Length; i++)
                {
                    periods[i].SetIconBytes(bytes);
                    _view.SetIcon(i, periods[i].IconBytes);
                }

                // Render();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {

            }
        }

        private bool IsCurrentImageRequest(
                 WeatherSnapshot snapshot,
                 WeatherDaySnapshot selectedDay,
                 int activationVersion,
                 int imageRequestVersion)
        {
            if (!_isActive ||
                activationVersion != _activationVersion ||
                imageRequestVersion != _imageRequestVersion ||
                !ReferenceEquals(snapshot, _snapshot))
                return false;


            if (_selectedDayIndex < 0 || _selectedDayIndex >= snapshot.Days.Count)
                return false;

            return true;
        }

        private void SelectUnit(TemperatureDisplayUnit unit)
        {
            _displayUnit = unit;
            _view.SetTemperatureUnit(unit);
        }

        private void SelectDay(int dayIndex)
        {
            if (_snapshot == null || dayIndex < 0 || dayIndex >= _snapshot.Days.Count)
                return;

            _selectedDayIndex = dayIndex;
            _view.ChangeSelectedDay(_snapshot, _selectedDayIndex, _displayUnit);
            RequestImagesAsync(_snapshot, _activationVersion);
            // Render();
        }

        private void Render()
        {
            if (_snapshot == null || _snapshot.Days.Count == 0)
                return;

            _view.ShowWeather(_snapshot, _selectedDayIndex, _displayUnit);
        }
    }
}