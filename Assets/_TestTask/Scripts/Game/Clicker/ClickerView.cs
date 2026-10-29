using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TestTask_CKR_System.Game.Clicker
{

    public class ClickerView : MonoBehaviour
    {
        public event System.Action CollectRequested;

        [SerializeField] private Button _collectButton;
        [SerializeField] private TMP_Text _currencyText;
        [SerializeField] private TMP_Text _energyText;
        [SerializeField] private ClickFeedbackView _feedback;
        [SerializeField] private RectTransform _currencyFxRoot;

        public Transform CurrencyFxRoot => _currencyFxRoot;

        private void OnEnable() => _collectButton.onClick.AddListener(OnCollectClicked);
        private void OnDisable() => _collectButton.onClick.RemoveListener(OnCollectClicked);

        private void OnCollectClicked() => CollectRequested?.Invoke();

        public void Render(ClickerState state, bool canCollect)
        {
            _currencyText.text = state.Currency.ToString();
            _energyText.text = $"{state.Energy}/{state.MaximumEnergy}";
            _collectButton.interactable = canCollect;
        }

        public void PlayCollectFeedback(int reward)
        {
            _feedback.Play(reward);
        }
    }
}