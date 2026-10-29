using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TestTask_CKR_System.Game.Weather.View
{
    public class WeatherDayView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private GameObject _indicators;

        private System.Action _action;

        public void Initialize(System.Action action)
        {
            _action = action;
            _button.onClick.AddListener(OnClick);
        }

        public void ResetState()
        {
            if (_action != null)
                _action = null;

            _button.onClick.RemoveListener(OnClick);
        }

        public void SetText(string text) => _text.text = text;
        public void SetActive(bool isActive) => _button.gameObject.SetActive(isActive);

        public void SetSelected(bool isActive)
        {
            _indicators.gameObject.SetActive(isActive);
            _button.interactable = !isActive;
        }

        private void OnClick()
        {
            _action?.Invoke();
        }

        public class Pool : MonoMemoryPool<System.Action, WeatherDayView>
        {
            protected override void Reinitialize(System.Action selectedDay, WeatherDayView item)
            {
                item.Initialize(selectedDay);
                item.transform.SetAsLastSibling();
            }

            protected override void OnDespawned(WeatherDayView item)
            {
                item.ResetState();
                base.OnDespawned(item);
            }
        }
    }

}