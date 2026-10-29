using TestTask_CKR_System.Game.Clicker.Animation;
using TMPro;
using UnityEngine;
using Zenject;

namespace TestTask_CKR_System.Game.Clicker
{
    public class CurrencyFlyView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _rewardText;

        private IClickerAnimationPlayer _animationPlayer;
        private Pool _pool;
        private IClickerAnimationHandle _animation;

        [Inject]
        public void Construct(IClickerAnimationPlayer animationPlayer)
        {
            _animationPlayer = animationPlayer;
        }

        private void Play(
            Vector3 worldPosition,
            int reward,
            float duration,
            float distance,
            Pool pool)
        {
            _pool = pool;
            _rectTransform.SetAsLastSibling();
            _rectTransform.position = worldPosition;
            _rectTransform.localScale = Vector3.one;
            _canvasGroup.alpha = 1f;
            _rewardText.SetText("+{0}", reward);
            var startPosition = _rectTransform.anchoredPosition;
            _animation = _animationPlayer.PlayCurrencyFly(
                _rectTransform,
                _canvasGroup,
                startPosition,
                distance,
                duration,
                CompleteAnimation);
        }

        private void ResetState()
        {
            _animation?.Dispose();
            _animation = null;
            _pool = null;
            _canvasGroup.alpha = 0f;
        }

        private void CompleteAnimation()
        {
            var pool = _pool;
            _animation = null;
            pool?.Despawn(this);
        }

        private void OnDestroy()
        {
            _animation?.Dispose();
            _animation = null;
        }

        public sealed class Pool :
            MonoMemoryPool<Vector3, int, float, float, CurrencyFlyView>
        {
            protected override void Reinitialize(
                Vector3 worldPosition,
                int reward,
                float duration,
                float distance,
                CurrencyFlyView item)
            {
                item.Play(worldPosition, reward, duration, distance, this);
            }

            protected override void OnDespawned(CurrencyFlyView item)
            {
                item.ResetState();
                base.OnDespawned(item);
            }
        }
    }
}