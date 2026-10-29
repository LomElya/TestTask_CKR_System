using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TestTask_CKR_System.Game.Weather.View
{
    public class WeatherPeriodView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _temperature;
        [SerializeField] private RawImage _icon;
        [SerializeField] private GameObject _loader;

        private const string TimeFormat = "HH:mm";
        private Texture2D _texture;
        private WeatherPeriodSnapshot _period;

        public void Show(WeatherPeriodSnapshot period, TemperatureDisplayUnit unit)
        {
            ClearTexture();

            _period = period;

            _title.text = $"{period.Part.GetPartName()} · {GetTime(period)}";
            SetTemperatureUnit(unit);
            SetIcon(period.IconBytes);
        }

        public void SetIcon(byte[] bytes)
        {
            ClearTexture();

            if (bytes != null && bytes.Length > 0)
            {
                _texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (!_texture.LoadImage(bytes))
                {
                    Destroy(_texture);
                    _texture = null;
                }
            }

            ChangeTexture(_texture);
        }

        public void SetTemperatureUnit(TemperatureDisplayUnit unit)
        {
            if (_period == null) return;
            _temperature.text = $"{_period.GetTemperature(unit)}{unit.GetTemperatureString()}";
        }

        private string GetTime(WeatherPeriodSnapshot period) =>
            $"{period.Time.ToString(TimeFormat)}-{period.EndTime.ToString(TimeFormat)}";

        private void ResetState()
        {
            ClearTexture();
            ChangeTexture(null);
        }

        private void ClearTexture()
        {
            if (_texture == null)
                return;

            Destroy(_texture);
            _texture = null;
        }

        private void OnDestroy()
        {
            ClearTexture();
        }

        private void ChangeTexture(Texture2D texture)
        {
            _texture = texture;
            _icon.texture = texture;
            bool active = _texture != null;
            _icon.gameObject.SetActive(active);
            _loader.SetActive(!active);
        }

        public class Pool : MonoMemoryPool<WeatherPeriodView>
        {
            protected override void Reinitialize(WeatherPeriodView item)
            {
                item.transform.SetAsLastSibling();
            }

            protected override void OnDespawned(WeatherPeriodView item)
            {
                item.ResetState();
                base.OnDespawned(item);
            }
        }
    }
}