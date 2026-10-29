using UnityEngine;

namespace TestTask_CKR_System.Game.Clicker.Animation
{

    public interface IClickerAnimationPlayer
    {
        IClickerAnimationHandle PlayButtonPress(
            RectTransform target,
            Vector3 initialScale,
            float pressedScale,
            float duration);

        IClickerAnimationHandle PlayCurrencyFly(
            RectTransform target,
            CanvasGroup canvasGroup,
            Vector2 startPosition,
            float distance,
            float duration,
            System.Action onCompleted);

        IClickerAnimationHandle PlayParticleBurst(
            ClickParticleTarget[] particles,
            float distance,
            float duration);
    }
}