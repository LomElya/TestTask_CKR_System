using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TestTask_CKR_System.Audio
{
    public class SoundClickUI : SoundClickBase, IPointerClickHandler
    {
        private bool _hasButton;
        private bool _hasRectTransform;

        protected void Awake()
        {
            _hasButton = TryGetComponent(out Button _);
            _hasRectTransform = TryGetComponent(out RectTransform _);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_hasButton || _hasRectTransform)
                Play();
        }
    }
}