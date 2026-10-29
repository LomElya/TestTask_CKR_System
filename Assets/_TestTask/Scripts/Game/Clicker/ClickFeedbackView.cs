using TestTask_CKR_System.Game.Clicker.Animation;
using TestTask_CKR_System.Game.Clicker.Config;
using UnityEngine;
using Zenject;

namespace TestTask_CKR_System.Game.Clicker
{
    public class ClickFeedbackView : MonoBehaviour
    {
        [SerializeField] private RectTransform _buttonTransform;
        [SerializeField] private ParticleSystem _particles;

        private IClickerBalanceConfig _config;
        private CurrencyFlyView.Pool _currencyPool;
        private IClickerAnimationPlayer _animationPlayer;
        private IClickerAnimationHandle _buttonAnimation;
        private Vector3 _initialScale;

        [Inject]
        public void Construct(
            IClickerBalanceConfig config,
            CurrencyFlyView.Pool currencyPool,
            IClickerAnimationPlayer animationPlayer)
        {
            _config = config;
            _currencyPool = currencyPool;
            _animationPlayer = animationPlayer;
        }

        private void Awake()
        {
            _initialScale = _buttonTransform.localScale;
        }

        public void Play(int reward)
        {
            StopButtonAnimation();
            _buttonAnimation = _animationPlayer.PlayButtonPress(
                 _buttonTransform,
                 _initialScale,
                 _config.PressedScale,
                 _config.ButtonAnimationDuration);

            if (_particles != null)
                _particles.Play();
            var localSpawnPosition = new Vector3(
                Random.Range(-80f, 80f),
                _buttonTransform.rect.height * 0.48f,
                0f);
            _currencyPool.Spawn(
                _buttonTransform.TransformPoint(localSpawnPosition),
                reward,
                _config.CurrencyFlightDuration,
                _config.CurrencyFlightDistance);
        }

        private void OnDisable()
        {
            StopButtonAnimation();
        }

        private void StopButtonAnimation()
        {
            _buttonAnimation?.Dispose();
            _buttonAnimation = null;

            if (_buttonTransform != null)
                _buttonTransform.localScale = _initialScale;
        }
    }
}