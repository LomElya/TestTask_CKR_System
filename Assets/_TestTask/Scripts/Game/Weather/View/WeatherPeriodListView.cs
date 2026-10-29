using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace TestTask_CKR_System.Game.Weather.View
{
    public class WeatherPeriodListView : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;

        private readonly List<WeatherPeriodView> _activeViews = new();
        private WeatherPeriodView.Pool _pool;

        public Transform Content => _content;

        [Inject]
        public void Construct(WeatherPeriodView.Pool pool)
        {
            _pool = pool;
        }

        public void Show(
            IReadOnlyList<WeatherPeriodSnapshot> periods,
            TemperatureDisplayUnit unit)
        {
            SetViewCount(periods.Count);
            for (var index = 0; index < periods.Count; index++)
                _activeViews[index].Show(periods[index], unit);
        }

        public void SetTemperatureUnit(TemperatureDisplayUnit unit)
        {
            for (var index = 0; index < _activeViews.Count; index++)
                _activeViews[index].SetTemperatureUnit(unit);
        }

        public void SetIcon(int index, byte[] bytes)
        {
            if (index < 0 || index + 1 > _activeViews.Count) return;
            _activeViews[index].SetIcon(bytes);
        }

        private void SetViewCount(int count)
        {
            if (count <= 0) return;

            for (var index = _activeViews.Count; index < count; index++)
                _activeViews.Add(_pool.Spawn());

            for (var index = _activeViews.Count - 1; index >= count; index--)
            {
                var view = _activeViews[index];
                _activeViews.RemoveAt(index);
                _pool.Despawn(view);
            }
        }
    }
}