using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TestTask_CKR_System.Game.Weather.View
{
    public class TemperatureUnitView : MonoBehaviour
    {
        [Serializable]
        private class UnitView : IDisposable
        {
            public event Action<TemperatureDisplayUnit> OnSelected;

            [SerializeField] private TemperatureDisplayUnit _unit;
            [SerializeField] private Button _button;
            [SerializeField] private TMP_Text _textTemperature;
            [SerializeField] private GameObject _indicator;

            public void Initialize()
            {
                _textTemperature.text = _unit.GetTemperatureString();
                _button.onClick.AddListener(OnClick);
            }

            public void SetActive(TemperatureDisplayUnit unit)
            {
                bool isActive = _unit == unit;
                _button.interactable = !isActive;
                _indicator.SetActive(isActive);
            }

            private void OnClick()
            {
                OnSelected?.Invoke(_unit);
            }

            public void Dispose()
            {
                _button.onClick.RemoveListener(OnClick);
            }
        }

        public event Action<TemperatureDisplayUnit> Selected;

        [SerializeField] private UnitView[] _units;

        private void Awake()
        {
            for (int i = 0; i < _units.Length; i++)
            {
                _units[i].Initialize();
                _units[i].OnSelected += OnSelect;
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _units.Length; i++)
            {
                _units[i].Dispose();
                _units[i].OnSelected -= OnSelect;
            }
        }

        public void Show(TemperatureDisplayUnit unit)
        {
            for (int i = 0; i < _units.Length; i++)
                _units[i].SetActive(unit);
        }

        private void OnSelect(TemperatureDisplayUnit unit)
        {
            Selected?.Invoke(unit);
        }
    }
}