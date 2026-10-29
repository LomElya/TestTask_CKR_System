using TMPro;
using UnityEngine;

namespace TestTask_CKR_System.Game.Breeds
{
    public class BreedListView : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private GameObject _loader;
        [SerializeField] private TMP_Text _errorText;

        public RectTransform Content => _content;

        public void SetListLoading(bool isLoading)
        {
            if (_loader == null) return;
            _loader.SetActive(isLoading);
        }

        public void ClearError()
        {
            _errorText?.gameObject.SetActive(false);
        }

        public void ShowError(string message)
        {
            if (_errorText == null) return;
            _errorText.text = message;
            _errorText.gameObject.SetActive(true);
        }
    }
}