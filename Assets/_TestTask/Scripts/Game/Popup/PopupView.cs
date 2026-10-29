using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TestTask_CKR_System.Game.Popup
{
    public class PopupView : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private GameObject _root;
        [SerializeField] private RectTransform _popup;

        [Header("Content")]
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Button _closeButton;

        public event System.Action CloseRequested;

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(OnCloseClicked);
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(OnCloseClicked);
        }

        public void Show(PopupDetails details)
        {
            Debug.Log("Show");
            _title.text = details.Name;
            _description.text = string.IsNullOrWhiteSpace(details.Description)
                ? "Описание отсутствует."
                : details.Description;
            _root.SetActive(true);

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_popup);
        }

        public void Hide()
        {
            _root.SetActive(false);
        }

        private void OnCloseClicked()
        {
            Hide();
            CloseRequested?.Invoke();
        }
    }
}
