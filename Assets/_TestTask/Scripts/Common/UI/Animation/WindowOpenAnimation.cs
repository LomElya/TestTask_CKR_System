using DG.Tweening;
using UnityEngine;

namespace TestTask_CKR_System.Common.UI.Animation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public  class WindowOpenAnimation : MonoBehaviour
    {
        private const float MinimumDuration = 0.0001f;

        [Header("Animation")]
        [SerializeField]
        private WindowOpenAnimationType _animationType =
            WindowOpenAnimationType.ScaleAndFade;
        [SerializeField, Min(0f)] private float _duration = 0.3f;
        [SerializeField, Min(0f)] private float _delay;
        [SerializeField] private Ease _ease = Ease.OutCubic;
        [SerializeField] private Ease _popEase = Ease.OutBack;
        [SerializeField] private bool _useUnscaledTime = true;
        [SerializeField] private bool _disableInteractionDuringAnimation = true;

        [Header("Start values")]
        [SerializeField, Range(0f, 1f)] private float _startScale = 0.8f;
        [SerializeField, Min(0f)] private float _horizontalOffset = 600f;
        [SerializeField, Min(0f)] private float _verticalOffset = 400f;
        [SerializeField, Range(0f, 360f)] private float _rotationAngle = 15f;
        [SerializeField, Range(0f, 180f)] private float _flipAngle = 90f;

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Sequence _sequence;
        private Vector2 _targetPosition;
        private Vector3 _targetScale;
        private Quaternion _targetRotation;
        private float _targetAlpha;
        private bool _targetInteractable;
        private bool _targetBlocksRaycasts;
        private bool _initialized;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            CaptureTargetState();
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
                return;

            CaptureTargetState();
            Play();
        }

        private void OnDisable()
        {
            KillAnimation();
            RestoreTargetState();
        }

        private void OnDestroy()
        {
            KillAnimation();
        }

        public void Play()
        {
            if (!_initialized)
                return;

            KillAnimation();
            RestoreTargetState();

            var strategy = WindowOpenAnimationStrategyFactory.Create(_animationType);
            if (!strategy.IsAnimated)
                return;

            if (_disableInteractionDuringAnimation)
            {
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }

            var context = CreateAnimationContext();
            strategy.Prepare(context);

            _sequence = DOTween.Sequence()
                .SetUpdate(_useUnscaledTime)
                .SetDelay(_delay);

            strategy.AddTweens(_sequence, context);
            _sequence.OnComplete(CompleteAnimation);
        }

        public void CaptureTargetState()
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            _targetPosition = _rectTransform.anchoredPosition;
            _targetScale = _rectTransform.localScale;
            _targetRotation = _rectTransform.localRotation;
            _targetAlpha = _canvasGroup.alpha;
            _targetInteractable = _canvasGroup.interactable;
            _targetBlocksRaycasts = _canvasGroup.blocksRaycasts;
            _initialized = true;
        }

        private WindowOpenAnimationContext CreateAnimationContext()
        {
            return new WindowOpenAnimationContext(
                _rectTransform,
                _canvasGroup,
                _targetPosition,
                _targetScale,
                _targetRotation,
                _targetAlpha,
                Mathf.Max(MinimumDuration, _duration),
                _ease,
                _popEase,
                _startScale,
                _horizontalOffset,
                _verticalOffset,
                _rotationAngle,
                _flipAngle);
        }

        private void CompleteAnimation()
        {
            _sequence = null;
            RestoreTargetState();
        }

        private void RestoreTargetState()
        {
            if (!_initialized)
                return;

            _rectTransform.anchoredPosition = _targetPosition;
            _rectTransform.localScale = _targetScale;
            _rectTransform.localRotation = _targetRotation;
            _canvasGroup.alpha = _targetAlpha;
            _canvasGroup.interactable = _targetInteractable;
            _canvasGroup.blocksRaycasts = _targetBlocksRaycasts;
        }

        private void KillAnimation()
        {
            if (_sequence != null && _sequence.IsActive())
                _sequence.Kill(false);

            _sequence = null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _duration = Mathf.Max(0f, _duration);
            _delay = Mathf.Max(0f, _delay);
            _horizontalOffset = Mathf.Max(0f, _horizontalOffset);
            _verticalOffset = Mathf.Max(0f, _verticalOffset);
        }
#endif
    }

}
