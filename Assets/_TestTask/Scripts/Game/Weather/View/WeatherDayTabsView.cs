using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace TestTask_CKR_System.Game.Weather.View
{
    public class WeatherDayTabsView : MonoBehaviour
    {
        public event Action<int> Selected;
        [SerializeField] private RectTransform _content;

        private List<WeatherDayView> _days = new();
        private WeatherDayView.Pool _pool;

        public RectTransform Content => _content;

        [Inject]
        private void Construct(WeatherDayView.Pool pool)
        {
            _pool = pool;
        }

        private void OnDestroy()
        {
            if (_pool == null)
                return;

            for (var i = _days.Count - 1; i >= 0; i--)
                _pool.Despawn(_days[i]);

            _days.Clear();
        }

        public void Show(IReadOnlyList<WeatherDaySnapshot> days, int selectedIndex)
        {
            SetViewCount(days.Count);

            for (var i = 0; i < days.Count; i++)
            {
                _days[i].SetText(GetLabel(i, days[i].Date));
                _days[i].SetSelected(selectedIndex == i);
            }
        }

        public void ChangeSelectedDay(int selectedIndex)
        {
            for (var i = 0; i < _days.Count; i++)
                _days[i].SetSelected(selectedIndex == i);
        }

        private void SetViewCount(int count)
        {
            for (var i = _days.Count; i < count; i++)
            {
                int dayIndex = i;
                WeatherDayView view = _pool.Spawn(() => SelectDay(dayIndex));

                _days.Add(view);
            }

            for (var i = _days.Count - 1; i >= count; i--)
            {
                _days.RemoveAt(i);
                _pool.Despawn(_days[i]);
            }
        }

        private void SelectDay(int index)
        {
            Selected?.Invoke(index);
        }

        private string GetLabel(int index, DateTime date)
        {
            if (index == 0)
                return "Сегодня";
            if (index == 1)
                return "Завтра";
            return date.GetLittleDateString();
        }
    }
}