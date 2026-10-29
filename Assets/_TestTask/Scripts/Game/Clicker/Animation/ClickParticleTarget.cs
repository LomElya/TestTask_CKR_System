using UnityEngine;

namespace TestTask_CKR_System.Game.Clicker.Animation
{
    public readonly struct ClickParticleTarget
    {
        public ClickParticleTarget(
            RectTransform transform,
            CanvasGroup canvasGroup,
            Vector2 direction)
        {
            Transform = transform;
            CanvasGroup = canvasGroup;
            Direction = direction;
        }

        public RectTransform Transform { get; }
        public CanvasGroup CanvasGroup { get; }
        public Vector2 Direction { get; }
    }
}