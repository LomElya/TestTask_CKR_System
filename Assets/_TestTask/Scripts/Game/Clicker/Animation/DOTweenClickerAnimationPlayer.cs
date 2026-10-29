using System;
using DG.Tweening;
using UnityEngine;

namespace TestTask_CKR_System.Game.Clicker.Animation
{
    public class DOTweenClickerAnimationPlayer : IClickerAnimationPlayer
    {
        private const float MinimumDuration = 0.0001f;

        public IClickerAnimationHandle PlayButtonPress(
            RectTransform target,
            Vector3 initialScale,
            float pressedScale,
            float duration)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var halfDuration = Mathf.Max(MinimumDuration, duration) * 0.5f;
            var sequence = DOTween.Sequence()
                .SetUpdate(true)
                .Append(target.DOScale(initialScale * pressedScale, halfDuration)
                    .SetEase(Ease.Linear))
                .Append(target.DOScale(initialScale, halfDuration)
                    .SetEase(Ease.Linear))
                .OnComplete(() => target.localScale = initialScale);

            return new DotweenAnimationHandle(sequence);
        }

        public IClickerAnimationHandle PlayCurrencyFly(
            RectTransform target,
            CanvasGroup canvasGroup,
            Vector2 startPosition,
            float distance,
            float duration,
            Action onCompleted)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (canvasGroup == null)
                throw new ArgumentNullException(nameof(canvasGroup));

            var effectiveDuration = Mathf.Max(MinimumDuration, duration);
            var sequence = DOTween.Sequence()
                .SetUpdate(true)
                .Join(target.DOAnchorPos(
                        startPosition + Vector2.up * distance,
                        effectiveDuration)
                    .SetEase(Ease.Linear))
                .Join(canvasGroup.DOFade(0f, effectiveDuration)
                    .SetEase(Ease.Linear))
                .OnComplete(() => onCompleted?.Invoke());

            return new DotweenAnimationHandle(sequence);
        }

        public IClickerAnimationHandle PlayParticleBurst(
            ClickParticleTarget[] particles,
            float distance,
            float duration)
        {
            if (particles == null)
                throw new ArgumentNullException(nameof(particles));

            var effectiveDuration = Mathf.Max(MinimumDuration, duration);
            var sequence = DOTween.Sequence().SetUpdate(true);

            foreach (var particle in particles)
            {
                particle.Transform.anchoredPosition = Vector2.zero;
                particle.Transform.localScale = Vector3.one * 0.55f;
                particle.CanvasGroup.alpha = 1f;

                sequence.Insert(
                    0f,
                    particle.Transform
                        .DOAnchorPos(particle.Direction * distance, effectiveDuration)
                        .SetEase(Ease.OutCubic));
                sequence.Insert(
                    0f,
                    particle.CanvasGroup
                        .DOFade(0f, effectiveDuration)
                        .SetEase(Ease.InQuad));
                sequence.Insert(
                    0f,
                    particle.Transform
                        .DOScale(1.15f, effectiveDuration * 0.3f)
                        .SetEase(Ease.OutBack));
                sequence.Insert(
                    effectiveDuration * 0.3f,
                    particle.Transform
                        .DOScale(0.25f, effectiveDuration * 0.7f)
                        .SetEase(Ease.InQuad));
            }

            sequence.OnComplete(() =>
            {
                foreach (var particle in particles)
                    particle.CanvasGroup.alpha = 0f;
            });

            return new DotweenAnimationHandle(sequence);
        }

        private sealed class DotweenAnimationHandle : IClickerAnimationHandle
        {
            private Tween _tween;

            public DotweenAnimationHandle(Tween tween)
            {
                _tween = tween;
            }

            public void Dispose()
            {
                if (_tween != null && _tween.IsActive())
                    _tween.Kill(false);

                _tween = null;
            }
        }
    }
}