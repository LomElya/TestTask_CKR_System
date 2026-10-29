using TMPro;
using UnityEngine;

namespace TestTask_CKR_System.Game.Weather.View
{
    public class WeatherStateView : MonoBehaviour
    {
        [SerializeField] private GameObject _loader;
        [SerializeField] private GameObject _content;
        [SerializeField] private TMP_Text _errorText;

        public void SetLoading(bool isLoading)
        {
            _loader.SetActive(isLoading);
        }

        public void ShowContent()
        {
            _errorText.gameObject.SetActive(false);
            _content.SetActive(true);
        }

        public void ShowError(string message)
        {
            _loader.SetActive(false);
            _content.SetActive(false);
            _errorText.text = message;
            _errorText.gameObject.SetActive(true);
        }
    }
}