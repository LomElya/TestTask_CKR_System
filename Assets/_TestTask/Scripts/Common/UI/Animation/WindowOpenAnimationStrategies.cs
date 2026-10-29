using DG.Tweening;
using UnityEngine;

namespace TestTask_CKR_System.Common.UI.Animation
{
    internal interface IWindowOpenAnimationStrategy
    {
        bool IsAnimated { get; }
        void Prepare(WindowOpenAnimationContext context);
        void AddTweens(Sequence sequence, WindowOpenAnimationContext context);
    }

    internal readonly struct WindowOpenAnimationContext
    {
        public WindowOpenAnimationContext(
            RectTransform rectTransform,
            CanvasGroup canvasGroup,
            Vector2 targetPosition,
            Vector3 targetScale,
            Quaternion targetRotation,
            float targetAlpha,
            float duration,
            Ease ease,
            Ease popEase,
            float startScale,
            float horizontalOffset,
            float verticalOffset,
            float rotationAngle,
            float flipAngle)
        {
            RectTransform = rectTransform;
            CanvasGroup = canvasGroup;
            TargetPosition = targetPosition;
            TargetScale = targetScale;
            TargetRotation = targetRotation;
            TargetAlpha = targetAlpha;
            Duration = duration;
            Ease = ease;
            PopEase = popEase;
            StartScale = startScale;
            HorizontalOffset = horizontalOffset;
            VerticalOffset = verticalOffset;
            RotationAngle = rotationAngle;
            FlipAngle = flipAngle;
        }

        public RectTransform RectTransform { get; }
        public CanvasGroup CanvasGroup { get; }
        public Vector2 TargetPosition { get; }
        public Vector3 TargetScale { get; }
        public Quaternion TargetRotation { get; }
        public float TargetAlpha { get; }
        public float Duration { get; }
        public Ease Ease { get; }
        public Ease PopEase { get; }
        public float StartScale { get; }
        public float HorizontalOffset { get; }
        public float VerticalOffset { get; }
        public float RotationAngle { get; }
        public float FlipAngle { get; }
    }

    internal static class WindowOpenAnimationStrategyFactory
    {
        private static readonly IWindowOpenAnimationStrategy None =
            new NoneWindowOpenAnimationStrategy();
        private static readonly IWindowOpenAnimationStrategy Fade =
            new FadeWindowOpenAnimationStrategy();
        private static readonly IWindowOpenAnimationStrategy Scale =
            new ScaleWindowOpenAnimationStrategy(false, false);
        private static readonly IWindowOpenAnimationStrategy ScaleAndFade =
            new ScaleWindowOpenAnimationStrategy(true, false);
        private static readonly IWindowOpenAnimationStrategy Pop =
            new ScaleWindowOpenAnimationStrategy(false, true);
        private static readonly IWindowOpenAnimationStrategy SlideFromLeft =
            new SlideWindowOpenAnimationStrategy(Vector2.left, false);
        private static readonly IWindowOpenAnimationStrategy SlideFromRight =
            new SlideWindowOpenAnimationStrategy(Vector2.right, false);
        private static readonly IWindowOpenAnimationStrategy SlideFromTop =
            new SlideWindowOpenAnimationStrategy(Vector2.up, false);
        private static readonly IWindowOpenAnimationStrategy SlideFromBottom =
            new SlideWindowOpenAnimationStrategy(Vector2.down, false);
        private static readonly IWindowOpenAnimationStrategy SlideFromLeftAndFade =
            new SlideWindowOpenAnimationStrategy(Vector2.left, true);
        private static readonly IWindowOpenAnimationStrategy SlideFromRightAndFade =
            new SlideWindowOpenAnimationStrategy(Vector2.right, true);
        private static readonly IWindowOpenAnimationStrategy SlideFromTopAndFade =
            new SlideWindowOpenAnimationStrategy(Vector2.up, true);
        private static readonly IWindowOpenAnimationStrategy SlideFromBottomAndFade =
            new SlideWindowOpenAnimationStrategy(Vector2.down, true);
        private static readonly IWindowOpenAnimationStrategy RotateClockwise =
            new RotateWindowOpenAnimationStrategy(Vector3.back, false);
        private static readonly IWindowOpenAnimationStrategy RotateCounterClockwise =
            new RotateWindowOpenAnimationStrategy(Vector3.forward, false);
        private static readonly IWindowOpenAnimationStrategy FlipHorizontal =
            new RotateWindowOpenAnimationStrategy(Vector3.up, true);
        private static readonly IWindowOpenAnimationStrategy FlipVertical =
            new RotateWindowOpenAnimationStrategy(Vector3.right, true);

        public static IWindowOpenAnimationStrategy Create(WindowOpenAnimationType type)
        {
            switch (type)
            {
                case WindowOpenAnimationType.Fade:
                    return Fade;
                case WindowOpenAnimationType.Scale:
                    return Scale;
                case WindowOpenAnimationType.ScaleAndFade:
                    return ScaleAndFade;
                case WindowOpenAnimationType.Pop:
                    return Pop;
                case WindowOpenAnimationType.SlideFromLeft:
                    return SlideFromLeft;
                case WindowOpenAnimationType.SlideFromRight:
                    return SlideFromRight;
                case WindowOpenAnimationType.SlideFromTop:
                    return SlideFromTop;
                case WindowOpenAnimationType.SlideFromBottom:
                    return SlideFromBottom;
                case WindowOpenAnimationType.SlideFromLeftAndFade:
                    return SlideFromLeftAndFade;
                case WindowOpenAnimationType.SlideFromRightAndFade:
                    return SlideFromRightAndFade;
                case WindowOpenAnimationType.SlideFromTopAndFade:
                    return SlideFromTopAndFade;
                case WindowOpenAnimationType.SlideFromBottomAndFade:
                    return SlideFromBottomAndFade;
                case WindowOpenAnimationType.RotateClockwise:
                    return RotateClockwise;
                case WindowOpenAnimationType.RotateCounterClockwise:
                    return RotateCounterClockwise;
                case WindowOpenAnimationType.FlipHorizontal:
                    return FlipHorizontal;
                case WindowOpenAnimationType.FlipVertical:
                    return FlipVertical;
                default:
                    return None;
            }
        }
    }

    internal sealed class NoneWindowOpenAnimationStrategy : IWindowOpenAnimationStrategy
    {
        public bool IsAnimated => false;
        public void Prepare(WindowOpenAnimationContext context) { }
        public void AddTweens(Sequence sequence, WindowOpenAnimationContext context) { }
    }

    internal sealed class FadeWindowOpenAnimationStrategy : IWindowOpenAnimationStrategy
    {
        public bool IsAnimated => true;

        public void Prepare(WindowOpenAnimationContext context)
        {
            context.CanvasGroup.alpha = 0f;
        }

        public void AddTweens(Sequence sequence, WindowOpenAnimationContext context)
        {
            sequence.Insert(
                0f,
                context.CanvasGroup
                    .DOFade(context.TargetAlpha, context.Duration)
                    .SetEase(context.Ease));
        }
    }

    internal sealed class ScaleWindowOpenAnimationStrategy : IWindowOpenAnimationStrategy
    {
        private readonly bool _useFade;
        private readonly bool _usePopEase;

        public ScaleWindowOpenAnimationStrategy(bool useFade, bool usePopEase)
        {
            _useFade = useFade;
            _usePopEase = usePopEase;
        }

        public bool IsAnimated => true;

        public void Prepare(WindowOpenAnimationContext context)
        {
            context.RectTransform.localScale = context.TargetScale * context.StartScale;
            if (_useFade)
                context.CanvasGroup.alpha = 0f;
        }

        public void AddTweens(Sequence sequence, WindowOpenAnimationContext context)
        {
            var ease = _usePopEase ? context.PopEase : context.Ease;
            sequence.Insert(
                0f,
                context.RectTransform
                    .DOScale(context.TargetScale, context.Duration)
                    .SetEase(ease));

            if (_useFade)
            {
                sequence.Insert(
                    0f,
                    context.CanvasGroup
                        .DOFade(context.TargetAlpha, context.Duration)
                        .SetEase(context.Ease));
            }
        }
    }

    internal sealed class SlideWindowOpenAnimationStrategy : IWindowOpenAnimationStrategy
    {
        private readonly Vector2 _direction;
        private readonly bool _useFade;

        public SlideWindowOpenAnimationStrategy(Vector2 direction, bool useFade)
        {
            _direction = direction;
            _useFade = useFade;
        }

        public bool IsAnimated => true;

        public void Prepare(WindowOpenAnimationContext context)
        {
            var offset = new Vector2(
                _direction.x * context.HorizontalOffset,
                _direction.y * context.VerticalOffset);
            context.RectTransform.anchoredPosition = context.TargetPosition + offset;

            if (_useFade)
                context.CanvasGroup.alpha = 0f;
        }

        public void AddTweens(Sequence sequence, WindowOpenAnimationContext context)
        {
            sequence.Insert(
                0f,
                context.RectTransform
                    .DOAnchorPos(context.TargetPosition, context.Duration)
                    .SetEase(context.Ease));

            if (_useFade)
            {
                sequence.Insert(
                    0f,
                    context.CanvasGroup
                        .DOFade(context.TargetAlpha, context.Duration)
                        .SetEase(context.Ease));
            }
        }
    }

    internal sealed class RotateWindowOpenAnimationStrategy : IWindowOpenAnimationStrategy
    {
        private readonly Vector3 _axis;
        private readonly bool _useFlipAngle;

        public RotateWindowOpenAnimationStrategy(Vector3 axis, bool useFlipAngle)
        {
            _axis = axis;
            _useFlipAngle = useFlipAngle;
        }

        public bool IsAnimated => true;

        public void Prepare(WindowOpenAnimationContext context)
        {
            var angle = _useFlipAngle ? context.FlipAngle : context.RotationAngle;
            context.RectTransform.localRotation =
                context.TargetRotation * Quaternion.AngleAxis(angle, _axis);
        }

        public void AddTweens(Sequence sequence, WindowOpenAnimationContext context)
        {
            sequence.Insert(
                0f,
                context.RectTransform
                    .DOLocalRotateQuaternion(context.TargetRotation, context.Duration)
                    .SetEase(context.Ease));
        }
    }
}